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
        // Apply class-specific stat modifiers
        if (soldierClass != null)
        {
            maxHealth = Mathf.RoundToInt(baseMaxHealth * soldierClass.healthMultiplier);
            movementRange = Mathf.RoundToInt(baseMovementRange * soldierClass.mobilityMultiplier);
            accuracy = Mathf.RoundToInt(baseAccuracy * soldierClass.accuracyMultiplier);
        }

        // Apply weapon-specific stat modifiers
        if (equippedWeapon != null)
        {
            attackRange = equippedWeapon.range;
            accuracy += equippedWeapon.accuracyModifier;
        }
        else
        {
            attackRange = 1; // Default melee range if no weapon is equipped
        }

        // Apply equipment bonuses
        foreach (Equipment equipment in equipments)
        {
            maxHealth += equipment.healthBonus;
            movementRange += equipment.mobilityBonus;
            accuracy += equipment.accuracyBonus;
        }

        // Apply difficulty settings
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

        // Ensure current health doesn't exceed new max health
        currentHealth = Mathf.Min(currentHealth, maxHealth);
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

    public int GetDamage()
    {
        int baseDamage = equippedWeapon != null ? equippedWeapon.GetDamage() : 1;
        float multiplier = CompareTag("Player") ? difficultyManager.GetPlayerDamageMultiplier() : difficultyManager.GetEnemyDamageMultiplier();
        return Mathf.RoundToInt(baseDamage * multiplier);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Move(Cell targetCell)
    {
        if (CanMove(targetCell))
        {
            currentCell = targetCell;
            transform.position = targetCell.WorldPosition;
            actionPoints -= CalculateMovementCost(targetCell);
            hasMoved = true;
        }
    }

    public bool CanMove(Cell targetCell)
    {
        return !hasMoved &&
               actionPoints >= CalculateMovementCost(targetCell) &&
               gridSystem.IsWithinRange(currentCell, targetCell, movementRange);
    }

    private int CalculateMovementCost(Cell targetCell)
    {
        // Simple implementation: each move costs 1 action point
        // You can make this more complex based on terrain type, etc.
        return 1;
    }

    public void Attack(Unit target)
    {
        if (CanAttack(target))
        {
            int damage = GetDamage();
            bool hit = Random.Range(0f, 100f) < accuracy;
            if (hit)
            {
                target.TakeDamage(damage);
            }
            actionPoints -= 1; // Assume each attack costs 1 action point
            hasAttacked = true;
        }
    }

    public bool CanAttack(Unit target)
    {
        return !hasAttacked &&
               actionPoints > 0 &&
               gridSystem.IsWithinRange(currentCell, gridSystem.GetCellAtPosition(target.transform.position), attackRange);
    }

    public void EndTurn()
    {
        hasMoved = false;
        hasAttacked = false;
        actionPoints = maxActionPoints;
    }

    public void SetPosition(Cell cell)
    {
        currentCell = cell;
        transform.position = cell.WorldPosition;
    }

    private void Die()
    {
        // Handle unit death (e.g., play death animation, remove from game, etc.)
        gameObject.SetActive(false);
        // Notify the TurnManager or other relevant systems about the unit's death
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (inventory.Contains(weapon))
        {
            equippedWeapon = weapon;
            UpdateStatsBasedOnClassAndWeapon();
        }
    }

    public void AddToInventory(Weapon weapon)
    {
        if (!inventory.Contains(weapon))
        {
            inventory.Add(weapon);
        }
    }

    public void RemoveFromInventory(Weapon weapon)
    {
        inventory.Remove(weapon);
        if (equippedWeapon == weapon)
        {
            equippedWeapon = inventory.Count > 0 ? inventory[0] : null;
            UpdateStatsBasedOnClassAndWeapon();
        }
    }

    public void EquipItem(Equipment item)
    {
        if (!equipments.Contains(item))
        {
            equipments.Add(item);
            UpdateStatsBasedOnClassAndWeapon();
        }
    }

    public void UnequipItem(Equipment item)
    {
        if (equipments.Remove(item))
        {
            UpdateStatsBasedOnClassAndWeapon();
        }
    }
}