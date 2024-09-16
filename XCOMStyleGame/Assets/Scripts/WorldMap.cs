using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class MissionSite
{
    public string name;
    public Vector2 position;
    public MissionType missionType;
    public int difficulty;
    public bool isAvailable = true;
    public string description;
}

public class WorldMap : MonoBehaviour
{
    public List<MissionSite> missionSites = new List<MissionSite>();
    public int maxActiveMissions = 3;
    public float newMissionChance = 0.3f;

    private CampaignManager campaignManager;

    void Start()
    {
        campaignManager = CampaignManager.Instance;
        GenerateInitialMissions();
    }

    void GenerateInitialMissions()
    {
        for (int i = 0; i < maxActiveMissions; i++)
        {
            GenerateNewMission();
        }
    }

    public void UpdateMissions()
    {
        // Remove completed missions
        missionSites.RemoveAll(m => !m.isAvailable);

        // Generate new missions
        while (missionSites.Count(m => m.isAvailable) < maxActiveMissions)
        {
            if (Random.value < newMissionChance)
            {
                GenerateNewMission();
            }
        }
    }

    void GenerateNewMission()
    {
        MissionSite newMission = new MissionSite
        {
            name = GenerateMissionName(),
            position = new Vector2(Random.Range(-180f, 180f), Random.Range(-90f, 90f)),
            missionType = (MissionType)Random.Range(0, System.Enum.GetValues(typeof(MissionType)).Length),
            difficulty = Random.Range(1, 6),
            description = GenerateMissionDescription()
        };

        missionSites.Add(newMission);
    }

    string GenerateMissionName()
    {
        string[] adjectives = { "Hidden", "Desperate", "Covert", "Crucial", "Dangerous" };
        string[] nouns = { "Strike", "Rescue", "Assault", "Defense", "Infiltration" };

        return $"Operation {adjectives[Random.Range(0, adjectives.Length)]} {nouns[Random.Range(0, nouns.Length)]}";
    }

    string GenerateMissionDescription()
    {
        // Generate a more detailed description based on mission type and difficulty
        return "A critical mission that requires immediate attention.";
    }

    public void SelectMission(MissionSite mission)
    {
        if (mission.isAvailable)
        {
            mission.isAvailable = false;
            campaignManager.StartMission(mission);
        }
    }

    public List<MissionSite> GetAvailableMissions()
    {
        return missionSites.Where(m => m.isAvailable).ToList();
    }
}