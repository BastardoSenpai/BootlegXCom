using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int baseMaxHealth;
    public int maxHealth;
    public int currentHealth;
    public int baseMovementRange;
    public int movementRange;
    public int baseAttackRange;
    public int attackRange;
    public int baseAccuracy;
    public int accuracy;
    public int actionPoints;
    public int maxActionPoints;

    public SoldierClass soldierClass;
    public Weapon equippedWeapon;
    public List<Weapon> inventory = new List<Weapon>();
    public List<Equipment> equipments = new List<Equipment>();

    private Cell currentCell;
    public bool hasMoved = false;
    public bool hasAttacked = false;

    private CharacterProgression characterProgression;
    private GridSystem gridSystem;
    private DifficultyManager difficultyManager;

    public GameObject fullCoverVisual;
    public GameObject halfCoverVisual;

    // Visual customization
    public SoldierCustomization customization;
    public SkinnedMeshRenderer bodyRenderer;
    public SkinnedMeshRenderer headRenderer;
    public SkinnedMeshRenderer hairRenderer;
    public SkinnedMeshRenderer armorRenderer;
    public GameObject[] accessoryObjects;

    void Start()
    {
        characterProgression = GetComponent<CharacterProgression>();
        if (characterProgression == null)
        {
            characterProgression = gameObject.AddComponent<CharacterProgression>();
        }
        gridSystem = FindObjectOfType<GridSystem>();
        difficultyManager = DifficultyManager.Instance;
        InitializeUnit();
    }

    void InitializeUnit()
    {
        ApplyDifficultySettings();
        currentHealth = maxHealth;
        actionPoints = maxActionPoints;
        UpdateStatsBasedOnClassAndWeapon();
        ApplyCustomization();
    }

    void ApplyDifficultySettings()
    {
        if (CompareTag("Player"))
        {
            maxHealth = Mathf.RoundToInt(baseMaxHealth * difficultyManager.GetPlayerHealthMultiplier());
            accuracy = Mathf.RoundToInt(baseAccuracy * difficultyManager.GetPlayerAccuracyMultiplier());
        }
        else
        {
            maxHealth = Mathf.RoundToInt(baseMaxHealth * difficultyManager.GetEnemyHealthMultiplier());
            accuracy = Mathf.RoundToInt(baseAccuracy * difficultyManager.GetEnemyAccuracyMultiplier());
        }
    }

    public void InitializeFromPersistentSoldier(PersistentSoldier soldier)
    {
        unitName = soldier.name;
        baseMaxHealth = soldier.stats["maxHealth"];
        baseAccuracy = soldier.stats["accuracy"];
        baseMovementRange = soldier.stats["mobility"];
        
        ApplyDifficultySettings();
        
        soldierClass = SoldierClass.GetSoldierClassByType(soldier.classType);

        foreach (string weaponName in soldier.inventory)
        {
            Weapon weapon = GameManager.Instance.availableWeapons.Find(w => w.weaponName == weaponName);
            if (weapon != null)
            {
                inventory.Add(weapon);
            }
        }
        if (inventory.Count > 0)
        {
            equippedWeapon = inventory[0];
        }

        characterProgression.level = soldier.level;
        characterProgression.experience = soldier.experience;
        characterProgression.InitializeFromPersistentSoldier(soldier);

        customization = soldier.customization;
        ApplyCustomization();

        UpdateStatsBasedOnClassAndWeapon();
    }

    void UpdateStatsBasedOnClassAndWeapon()
    {
        // ... (existing code)

        if (CompareTag("Player"))
        {
            maxHealth = Mathf.RoundToInt(maxHealth * difficultyManager.GetPlayerHealthMultiplier());
            accuracy = Mathf.RoundToInt(accuracy * difficultyManager.GetPlayerAccuracyMultiplier());
        }
        else
        {
            maxHealth = Mathf.RoundToInt(maxHealth * difficultyManager.GetEnemyHealthMultiplier());
            accuracy = Mathf.RoundToInt(accuracy * difficultyManager.GetEnemyAccuracyMultiplier());
        }
    }

    public void ApplyCustomization()
    {
        if (customization == null)
        {
            customization = SoldierCustomization.GenerateRandomCustomization();
        }

        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = customization.skinColor;
        }

        if (headRenderer != null)
        {
            headRenderer.material.SetColor("_SkinColor", customization.skinColor);
            headRenderer.material.SetColor("_EyeColor", customization.eyeColor);
            headRenderer.SetBlendShapeWeight(0, customization.faceIndex * 100f); // Assuming blend shapes for faces
        }

        if (hairRenderer != null)
        {
            hairRenderer.material.color = customization.hairColor;
            hairRenderer.gameObject.SetActive(false);
            Transform hairStyle = hairRenderer.transform.GetChild(customization.hairStyleIndex);
            if (hairStyle != null)
            {
                hairStyle.gameObject.SetActive(true);
            }
        }

        if (armorRenderer != null)
        {
            Material armorMaterial = armorRenderer.material;
            armorMaterial.SetColor("_PrimaryColor", customization.armorPrimaryColor);
            armorMaterial.SetColor("_SecondaryColor", customization.armorSecondaryColor);
            armorMaterial.SetFloat("_PatternIndex", customization.armorPatternIndex);
        }

        for (int i = 0; i < accessoryObjects.Length; i++)
        {
            if (i < customization.accessories.Count)
            {
                accessoryObjects[i].SetActive(true);
                accessoryObjects[i].name = customization.accessories[i];
            }
            else
            {
                accessoryObjects[i].SetActive(false);
            }
        }
    }

    // ... (rest of the existing methods)

    public int GetDamage()
    {
        int baseDamage = equippedWeapon != null ? equippedWeapon.GetDamage() : 1;
        float multiplier = CompareTag("Player") ? difficultyManager.GetPlayerDamageMultiplier() : difficultyManager.GetEnemyDamageMultiplier();
        return Mathf.RoundToInt(baseDamage * multiplier);
    }
}