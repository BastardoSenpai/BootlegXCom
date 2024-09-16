using UnityEngine;

public enum DifficultyLevel { Easy, Normal, Hard, Impossible }

[System.Serializable]
public class DifficultySettings
{
    public float playerHealthMultiplier = 1f;
    public float playerAccuracyMultiplier = 1f;
    public float playerDamageMultiplier = 1f;
    public float playerResourceMultiplier = 1f;

    public float enemyHealthMultiplier = 1f;
    public float enemyAccuracyMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
    public float enemyCountMultiplier = 1f;

    public int startingResources = 1000;
    public int maxSquadSize = 4;

    public float missionTimeMultiplier = 1f;
    public float researchTimeMultiplier = 1f;
}

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    public DifficultyLevel currentDifficulty = DifficultyLevel.Normal;

    public DifficultySettings easySettings = new DifficultySettings
    {
        playerHealthMultiplier = 1.2f,
        playerAccuracyMultiplier = 1.2f,
        playerDamageMultiplier = 1.2f,
        playerResourceMultiplier = 1.5f,
        enemyHealthMultiplier = 0.8f,
        enemyAccuracyMultiplier = 0.8f,
        enemyDamageMultiplier = 0.8f,
        enemyCountMultiplier = 0.8f,
        startingResources = 1500,
        maxSquadSize = 6,
        missionTimeMultiplier = 1.2f,
        researchTimeMultiplier = 0.8f
    };

    public DifficultySettings normalSettings = new DifficultySettings();

    public DifficultySettings hardSettings = new DifficultySettings
    {
        playerHealthMultiplier = 0.9f,
        playerAccuracyMultiplier = 0.9f,
        playerDamageMultiplier = 0.9f,
        playerResourceMultiplier = 0.8f,
        enemyHealthMultiplier = 1.2f,
        enemyAccuracyMultiplier = 1.1f,
        enemyDamageMultiplier = 1.1f,
        enemyCountMultiplier = 1.2f,
        startingResources = 800,
        maxSquadSize = 4,
        missionTimeMultiplier = 0.9f,
        researchTimeMultiplier = 1.2f
    };

    public DifficultySettings impossibleSettings = new DifficultySettings
    {
        playerHealthMultiplier = 0.8f,
        playerAccuracyMultiplier = 0.8f,
        playerDamageMultiplier = 0.8f,
        playerResourceMultiplier = 0.5f,
        enemyHealthMultiplier = 1.5f,
        enemyAccuracyMultiplier = 1.2f,
        enemyDamageMultiplier = 1.3f,
        enemyCountMultiplier = 1.5f,
        startingResources = 500,
        maxSquadSize = 4,
        missionTimeMultiplier = 0.8f,
        researchTimeMultiplier = 1.5f
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficulty(DifficultyLevel difficulty)
    {
        currentDifficulty = difficulty;
    }

    public DifficultySettings GetCurrentSettings()
    {
        switch (currentDifficulty)
        {
            case DifficultyLevel.Easy:
                return easySettings;
            case DifficultyLevel.Normal:
                return normalSettings;
            case DifficultyLevel.Hard:
                return hardSettings;
            case DifficultyLevel.Impossible:
                return impossibleSettings;
            default:
                return normalSettings;
        }
    }

    public float GetPlayerHealthMultiplier() => GetCurrentSettings().playerHealthMultiplier;
    public float GetPlayerAccuracyMultiplier() => GetCurrentSettings().playerAccuracyMultiplier;
    public float GetPlayerDamageMultiplier() => GetCurrentSettings().playerDamageMultiplier;
    public float GetPlayerResourceMultiplier() => GetCurrentSettings().playerResourceMultiplier;

    public float GetEnemyHealthMultiplier() => GetCurrentSettings().enemyHealthMultiplier;
    public float GetEnemyAccuracyMultiplier() => GetCurrentSettings().enemyAccuracyMultiplier;
    public float GetEnemyDamageMultiplier() => GetCurrentSettings().enemyDamageMultiplier;
    public float GetEnemyCountMultiplier() => GetCurrentSettings().enemyCountMultiplier;

    public int GetStartingResources() => GetCurrentSettings().startingResources;
    public int GetMaxSquadSize() => GetCurrentSettings().maxSquadSize;

    public float GetMissionTimeMultiplier() => GetCurrentSettings().missionTimeMultiplier;
    public float GetResearchTimeMultiplier() => GetCurrentSettings().researchTimeMultiplier;
}