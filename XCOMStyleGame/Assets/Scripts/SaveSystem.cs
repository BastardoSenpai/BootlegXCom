using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
    private const string SAVE_FOLDER = "/SaveData/";
    private const string CAMPAIGN_SAVE_FILE = "campaign.save";
    private const string MISSION_SAVE_FILE = "mission.save";

    public static void SaveCampaign(CampaignData campaignData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + SAVE_FOLDER + CAMPAIGN_SAVE_FILE;
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, campaignData);
        }
    }

    public static CampaignData LoadCampaign()
    {
        string path = Application.persistentDataPath + SAVE_FOLDER + CAMPAIGN_SAVE_FILE;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return formatter.Deserialize(stream) as CampaignData;
            }
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void SaveMission(MissionSaveData missionData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + SAVE_FOLDER + MISSION_SAVE_FILE;
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, missionData);
        }
    }

    public static MissionSaveData LoadMission()
    {
        string path = Application.persistentDataPath + SAVE_FOLDER + MISSION_SAVE_FILE;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return formatter.Deserialize(stream) as MissionSaveData;
            }
        }
        else
        {
            Debug.LogError("Mission save file not found in " + path);
            return null;
        }
    }

    public static bool MissionSaveExists()
    {
        string path = Application.persistentDataPath + SAVE_FOLDER + MISSION_SAVE_FILE;
        return File.Exists(path);
    }

    public static void DeleteMissionSave()
    {
        string path = Application.persistentDataPath + SAVE_FOLDER + MISSION_SAVE_FILE;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

[System.Serializable]
public class MissionSaveData
{
    public MissionType missionType;
    public int turnsRemaining;
    public List<UnitSaveData> playerUnits;
    public List<UnitSaveData> enemyUnits;
    public List<ObjectiveSaveData> objectives;
    public GridSaveData gridData;

    public MissionSaveData(MissionManager missionManager, TurnManager turnManager, GridSystem gridSystem)
    {
        missionType = missionManager.currentMissionType;
        turnsRemaining = missionManager.turnsRemaining;
        
        playerUnits = turnManager.playerUnits.Select(u => new UnitSaveData(u)).ToList();
        enemyUnits = turnManager.enemyUnits.Select(u => new UnitSaveData(u)).ToList();
        
        objectives = missionManager.objectives.Select(o => new ObjectiveSaveData(o)).ToList();
        
        gridData = new GridSaveData(gridSystem);
    }
}

[System.Serializable]
public class UnitSaveData
{
    public string unitName;
    public Vector3 position;
    public int currentHealth;
    public int actionPoints;
    public bool hasMoved;
    public bool hasAttacked;

    public UnitSaveData(Unit unit)
    {
        unitName = unit.unitName;
        position = unit.transform.position;
        currentHealth = unit.currentHealth;
        actionPoints = unit.actionPoints;
        hasMoved = unit.hasMoved;
        hasAttacked = unit.hasAttacked;
    }
}

[System.Serializable]
public class ObjectiveSaveData
{
    public string description;
    public bool completed;

    public ObjectiveSaveData(MissionManager.MissionObjective objective)
    {
        description = objective.description;
        completed = objective.completed;
    }
}

[System.Serializable]
public class GridSaveData
{
    public List<CellSaveData> cells;

    public GridSaveData(GridSystem gridSystem)
    {
        cells = gridSystem.GetAllCells().Select(c => new CellSaveData(c)).ToList();
    }
}

[System.Serializable]
public class CellSaveData
{
    public Vector3Int gridPosition;
    public CoverType coverType;
    public TerrainType terrainType;
    public bool isOccupied;

    public CellSaveData(Cell cell)
    {
        gridPosition = cell.GridPosition;
        coverType = cell.CoverType;
        terrainType = cell.TerrainType;
        isOccupied = cell.IsOccupied;
    }
}