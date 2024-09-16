using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CampaignData
{
    public List<PersistentSoldier> soldiers = new List<PersistentSoldier>();
    public int completedMissions = 0;
    public int resources = 1000;
}

[System.Serializable]
public class PersistentSoldier
{
    public string name;
    public SoldierClassType classType;
    public int level;
    public int experience;
    public List<string> unlockedAbilities = new List<string>();
    public Dictionary<string, int> stats = new Dictionary<string, int>();
    public List<string> inventory = new List<string>();
    public SoldierCustomization customization = new SoldierCustomization();
}

[System.Serializable]
public class SoldierCustomization
{
    public Color armorColor = Color.white;
    public int faceIndex = 0;
    public int hairIndex = 0;
    public Color hairColor = Color.black;
}

public class CampaignManager : MonoBehaviour
{
    private static CampaignManager _instance;
    public static CampaignManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CampaignManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("CampaignManager");
                    _instance = go.AddComponent<CampaignManager>();
                }
            }
            return _instance;
        }
    }

    public CampaignData currentCampaign = new CampaignData();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void StartNewCampaign()
    {
        currentCampaign = new CampaignData();
        GenerateInitialSoldiers();
    }

    private void GenerateInitialSoldiers()
    {
        for (int i = 0; i < 4; i++)
        {
            PersistentSoldier soldier = CreateRandomSoldier();
            currentCampaign.soldiers.Add(soldier);
        }
    }

    private PersistentSoldier CreateRandomSoldier()
    {
        PersistentSoldier soldier = new PersistentSoldier
        {
            name = GenerateRandomName(),
            classType = (SoldierClassType)Random.Range(0, System.Enum.GetValues(typeof(SoldierClassType)).Length),
            level = 1,
            experience = 0
        };

        soldier.stats["maxHealth"] = 100;
        soldier.stats["accuracy"] = 70;
        soldier.stats["mobility"] = 5;

        soldier.inventory.Add("AssaultRifle");

        return soldier;
    }

    private string GenerateRandomName()
    {
        string[] firstNames = { "John", "Jane", "Alex", "Sarah", "Mike", "Emma" };
        string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia" };

        return $"{firstNames[Random.Range(0, firstNames.Length)]} {lastNames[Random.Range(0, lastNames.Length)]}";
    }

    public void SaveCampaign()
    {
        string json = JsonUtility.ToJson(currentCampaign);
        PlayerPrefs.SetString("CampaignData", json);
        PlayerPrefs.Save();
    }

    public void LoadCampaign()
    {
        if (PlayerPrefs.HasKey("CampaignData"))
        {
            string json = PlayerPrefs.GetString("CampaignData");
            currentCampaign = JsonUtility.FromJson<CampaignData>(json);
        }
        else
        {
            Debug.LogWarning("No saved campaign found. Starting a new campaign.");
            StartNewCampaign();
        }
    }

    public void UpdateSoldierAfterMission(Unit unit)
    {
        PersistentSoldier persistentSoldier = currentCampaign.soldiers.FirstOrDefault(s => s.name == unit.unitName);
        if (persistentSoldier != null)
        {
            persistentSoldier.level = unit.GetComponent<CharacterProgression>().level;
            persistentSoldier.experience = unit.GetComponent<CharacterProgression>().experience;
            persistentSoldier.unlockedAbilities = unit.soldierClass.abilities.Where(a => a.isUnlocked).Select(a => a.name).ToList();
            persistentSoldier.stats["maxHealth"] = unit.maxHealth;
            persistentSoldier.stats["accuracy"] = unit.accuracy;
            persistentSoldier.stats["mobility"] = unit.movementRange;
        }
    }

    public Unit CreateUnitFromPersistentSoldier(PersistentSoldier persistentSoldier, GameObject unitPrefab)
    {
        Unit unit = Instantiate(unitPrefab).GetComponent<Unit>();
        unit.unitName = persistentSoldier.name;
        unit.soldierClass = GetSoldierClassByType(persistentSoldier.classType);
        unit.maxHealth = persistentSoldier.stats["maxHealth"];
        unit.accuracy = persistentSoldier.stats["accuracy"];
        unit.movementRange = persistentSoldier.stats["mobility"];

        CharacterProgression progression = unit.GetComponent<CharacterProgression>();
        progression.level = persistentSoldier.level;
        progression.experience = persistentSoldier.experience;

        // Apply customization
        ApplySoldierCustomization(unit, persistentSoldier.customization);

        return unit;
    }

    private SoldierClass GetSoldierClassByType(SoldierClassType classType)
    {
        // This method should return the appropriate SoldierClass ScriptableObject based on the class type
        // You'll need to implement this based on how you're storing your SoldierClass ScriptableObjects
        return null;
    }

    private void ApplySoldierCustomization(Unit unit, SoldierCustomization customization)
    {
        // Apply visual customization to the unit
        // This will depend on how you've set up your unit's visual components
        Renderer renderer = unit.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = customization.armorColor;
        }

        // Apply face and hair customization
        // You'll need to implement this based on your character model setup
    }
}