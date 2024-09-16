using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Facility
{
    public string name;
    public string description;
    public int constructionTime;
    public int powerUsage;
    public int resourceCost;
    public List<string> prerequisites = new List<string>();
    public bool isConstructed = false;
}

public class BaseManager : MonoBehaviour
{
    public CampaignManager campaignManager;
    public ResearchManager researchManager;

    public List<Facility> availableFacilities = new List<Facility>();
    public List<Facility> constructedFacilities = new List<Facility>();
    public Facility currentConstruction;

    public int power = 0;
    public int resources = 1000;

    private int currentDay = 0;
    private int remainingConstructionDays = 0;

    public delegate void FacilityConstructedDelegate(Facility facility);
    public event FacilityConstructedDelegate OnFacilityConstructed;

    void Start()
    {
        campaignManager = FindObjectOfType<CampaignManager>();
        researchManager = FindObjectOfType<ResearchManager>();
        InitializeFacilities();
    }

    void InitializeFacilities()
    {
        availableFacilities = new List<Facility>
        {
            new Facility
            {
                name = "Power Generator",
                description = "Generates power for the base.",
                constructionTime = 5,
                powerUsage = -10,
                resourceCost = 100
            },
            new Facility
            {
                name = "Laboratory",
                description = "Increases research speed.",
                constructionTime = 10,
                powerUsage = 3,
                resourceCost = 200,
                prerequisites = new List<string> { "Power Generator" }
            },
            new Facility
            {
                name = "Workshop",
                description = "Increases engineering efficiency.",
                constructionTime = 10,
                powerUsage = 3,
                resourceCost = 200,
                prerequisites = new List<string> { "Power Generator" }
            },
            new Facility
            {
                name = "Barracks",
                description = "Increases soldier capacity.",
                constructionTime = 7,
                powerUsage = 2,
                resourceCost = 150
            },
            new Facility
            {
                name = "Psi Lab",
                description = "Allows training of psionic abilities.",
                constructionTime = 15,
                powerUsage = 5,
                resourceCost = 300,
                prerequisites = new List<string> { "Laboratory", "Psionic Abilities" }
            }
        };
    }

    public void StartConstruction(string facilityName)
    {
        Facility facility = availableFacilities.Find(f => f.name == facilityName);
        if (facility != null && CanConstructFacility(facility))
        {
            currentConstruction = facility;
            remainingConstructionDays = facility.constructionTime;
            resources -= facility.resourceCost;
            availableFacilities.Remove(facility);
            Debug.Log($"Started construction of {facilityName}");
        }
        else
        {
            Debug.Log($"Cannot construct {facilityName}");
        }
    }

    public void AdvanceDay()
    {
        currentDay++;
        if (currentConstruction != null)
        {
            remainingConstructionDays--;
            if (remainingConstructionDays <= 0)
            {
                CompleteCurrentConstruction();
            }
        }

        researchManager.AdvanceDay();
        UpdateResources();
    }

    private void CompleteCurrentConstruction()
    {
        currentConstruction.isConstructed = true;
        constructedFacilities.Add(currentConstruction);
        power += currentConstruction.powerUsage;
        Debug.Log($"Construction completed: {currentConstruction.name}");
        OnFacilityConstructed?.Invoke(currentConstruction);

        // Unlock new facilities
        List<Facility> newlyAvailable = availableFacilities.Where(f => 
            f.prerequisites.All(prereq => constructedFacilities.Any(cf => cf.name == prereq) || 
                                          researchManager.GetUnlockedTechnologies().Contains(prereq))).ToList();

        foreach (var facility in newlyAvailable)
        {
            Debug.Log($"New facility available: {facility.name}");
        }

        currentConstruction = null;
    }

    private bool CanConstructFacility(Facility facility)
    {
        return resources >= facility.resourceCost &&
               power + facility.powerUsage >= 0 &&
               facility.prerequisites.All(prereq => 
                   constructedFacilities.Any(cf => cf.name == prereq) || 
                   researchManager.GetUnlockedTechnologies().Contains(prereq));
    }

    public List<Facility> GetAvailableFacilities()
    {
        return availableFacilities.Where(f => CanConstructFacility(f)).ToList();
    }

    public float GetConstructionProgress()
    {
        if (currentConstruction == null) return 0f;
        return 1f - (float)remainingConstructionDays / currentConstruction.constructionTime;
    }

    private void UpdateResources()
    {
        // Simulate resource generation based on constructed facilities
        int resourceGeneration = 50; // Base resource generation
        resourceGeneration += constructedFacilities.Count(f => f.name == "Workshop") * 20;
        resources += resourceGeneration;
    }

    public int GetResearchSpeedBonus()
    {
        return constructedFacilities.Count(f => f.name == "Laboratory") * 10; // 10% bonus per lab
    }

    public int GetEngineeringEfficiencyBonus()
    {
        return constructedFacilities.Count(f => f.name == "Workshop") * 5; // 5% bonus per workshop
    }

    public int GetSoldierCapacity()
    {
        return 4 + constructedFacilities.Count(f => f.name == "Barracks") * 4; // Base 4 + 4 per barracks
    }

    public bool HasPsiLab()
    {
        return constructedFacilities.Any(f => f.name == "Psi Lab");
    }
}