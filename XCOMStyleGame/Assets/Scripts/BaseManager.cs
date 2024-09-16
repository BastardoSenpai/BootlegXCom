using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BaseManager : MonoBehaviour
{
    public CampaignManager campaignManager;
    
    [System.Serializable]
    public class Research
    {
        public string name;
        public string description;
        public int cost;
        public int turnsToComplete;
        public bool isCompleted;
        public List<string> requirements;
    }

    [System.Serializable]
    public class Facility
    {
        public string name;
        public string description;
        public int cost;
        public int capacity;
        public int count;
    }

    [System.Serializable]
    public class Item
    {
        public string name;
        public string description;
        public int cost;
        public int count;
    }

    public List<Research> availableResearch = new List<Research>();
    public List<Facility> facilities = new List<Facility>();
    public List<Item> inventory = new List<Item>();

    private Research currentResearch;
    private int remainingResearchTurns;

    void Start()
    {
        campaignManager = CampaignManager.Instance;
        InitializeBaseData();
    }

    void InitializeBaseData()
    {
        // Initialize available research
        availableResearch = new List<Research>
        {
            new Research { name = "Advanced Weapons", description = "Unlock better weapons", cost = 100, turnsToComplete = 5, requirements = new List<string>() },
            new Research { name = "Improved Armor", description = "Enhance soldier protection", cost = 150, turnsToComplete = 6, requirements = new List<string>() },
            new Research { name = "Psionic Abilities", description = "Unlock mental powers", cost = 200, turnsToComplete = 8, requirements = new List<string> { "Advanced Weapons" } }
        };

        // Initialize facilities
        facilities = new List<Facility>
        {
            new Facility { name = "Laboratory", description = "Speeds up research", cost = 200, capacity = 2, count = 1 },
            new Facility { name = "Workshop", description = "Reduces item costs", cost = 150, capacity = 2, count = 1 },
            new Facility { name = "Power Generator", description = "Provides power for the base", cost = 100, capacity = 0, count = 1 }
        };

        // Initialize inventory
        inventory = new List<Item>
        {
            new Item { name = "Medkit", description = "Heals soldiers", cost = 50, count = 2 },
            new Item { name = "Grenade", description = "Explosive device", cost = 75, count = 3 }
        };
    }

    public void StartResearch(string researchName)
    {
        Research research = availableResearch.Find(r => r.name == researchName && !r.isCompleted);
        if (research != null && campaignManager.currentCampaign.resources >= research.cost)
        {
            campaignManager.currentCampaign.resources -= research.cost;
            currentResearch = research;
            remainingResearchTurns = research.turnsToComplete;
            Debug.Log($"Started research on {researchName}");
        }
        else
        {
            Debug.Log("Cannot start research: Not available or insufficient resources");
        }
    }

    public void ProgressResearch()
    {
        if (currentResearch != null)
        {
            remainingResearchTurns--;
            if (remainingResearchTurns <= 0)
            {
                CompleteResearch();
            }
        }
    }

    private void CompleteResearch()
    {
        currentResearch.isCompleted = true;
        Debug.Log($"Research completed: {currentResearch.name}");
        // Apply research effects (e.g., unlock new items, improve stats)
        currentResearch = null;
    }

    public void BuildFacility(string facilityName)
    {
        Facility facility = facilities.Find(f => f.name == facilityName);
        if (facility != null && campaignManager.currentCampaign.resources >= facility.cost)
        {
            campaignManager.currentCampaign.resources -= facility.cost;
            facility.count++;
            Debug.Log($"Built new {facilityName}");
        }
        else
        {
            Debug.Log("Cannot build facility: Not available or insufficient resources");
        }
    }

    public void CraftItem(string itemName)
    {
        Item item = inventory.Find(i => i.name == itemName);
        if (item != null && campaignManager.currentCampaign.resources >= item.cost)
        {
            campaignManager.currentCampaign.resources -= item.cost;
            item.count++;
            Debug.Log($"Crafted new {itemName}");
        }
        else
        {
            Debug.Log("Cannot craft item: Not available or insufficient resources");
        }
    }

    public int GetResearchSpeedBonus()
    {
        Facility lab = facilities.Find(f => f.name == "Laboratory");
        return lab != null ? lab.count * lab.capacity : 0;
    }

    public float GetCraftingCostReduction()
    {
        Facility workshop = facilities.Find(f => f.name == "Workshop");
        return workshop != null ? workshop.count * 0.05f : 0f; // 5% reduction per workshop
    }

    public int GetTotalPower()
    {
        Facility powerGen = facilities.Find(f => f.name == "Power Generator");
        return powerGen != null ? powerGen.count * 5 : 0; // 5 power per generator
    }
}