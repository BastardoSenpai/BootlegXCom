using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int maxHealth;
    public int currentHealth;
    public int movementRange;
    public int attackRange;
    public int accuracy;
    public int actionPoints;
    public int maxActionPoints;

    public SoldierClass soldierClass;
    public Weapon equippedWeapon;
    public List<Weapon> inventory = new List<Weapon>();

    private Cell currentCell;
    public bool hasMoved = false;
    public bool hasAttacked = false;

    private CharacterProgression characterProgression;

    void Start()
    {
        characterProgression = GetComponent<CharacterProgression>();
        if (characterProgression == null)
        {
            characterProgression = gameObject.AddComponent<CharacterProgression>();
        }
        InitializeUnit();
    }

    void InitializeUnit()
    {
        currentHealth = maxHealth;
        actionPoints = maxActionPoints;
        UpdateStatsBasedOnClassAndWeapon();
    }

    void UpdateStatsBasedOnClassAndWeapon()
    {
        if (soldierClass != null)
        {
            // Update stats based on soldier class
            switch (soldierClass.classType)
            {
                case SoldierClassType.Assault:
                    movementRange += 2;
                    break;
                case SoldierClassType.Sniper:
                    accuracy += 10;
                    break;
                case SoldierClassType.Heavy:
                    maxHealth += 20;
                    currentHealth += 20;
                    break;
                case SoldierClassType.Support:
                    maxActionPoints += 1;
                    actionPoints += 1;
                    break;
            }
        }

        if (equippedWeapon != null)
        {
            // Update stats based on equipped weapon
            attackRange = equippedWeapon.range;
            accuracy += Mathf.RoundToInt(equippedWeapon.accuracyModifier * 100);
        }
    }

    public void SetPosition(Cell cell)
    {
        if (currentCell != null)
        {
            currentCell.IsOccupied = false;
        }

        currentCell = cell;
        currentCell.IsOccupied = true;
        transform.position = cell.WorldPosition + Vector3.up * 0.5f;
    }

    public bool CanMoveTo(Cell targetCell)
    {
        return !hasMoved && actionPoints >= 1 && Vector3.Distance(currentCell.WorldPosition, targetCell.WorldPosition) <= movementRange;
    }

    public void Move(Cell targetCell)
    {
        if (CanMoveTo(targetCell))
        {
            SetPosition(targetCell);
            hasMoved = true;
            actionPoints--;
        }
    }

    public bool CanAttack(Unit target)
    {
        return !hasAttacked && actionPoints >= 1 && Vector3.Distance(transform.position, target.transform.position) <= attackRange;
    }

    public void Attack(Unit target)
    {
        if (CanAttack(target))
        {
            hasAttacked = true;
            actionPoints--;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{unitName} has been defeated!");
        Destroy(gameObject);
    }

    public void StartTurn()
    {
        hasMoved = false;
        hasAttacked = false;
        actionPoints = maxActionPoints;
        
        // Reset ability cooldowns
        if (soldierClass != null)
        {
            foreach (var ability in soldierClass.abilities)
            {
                if (ability.currentCooldown > 0)
                {
                    ability.currentCooldown--;
                }
            }
        }
    }

    public void EndTurn()
    {
        hasMoved = true;
        hasAttacked = true;
        actionPoints = 0;
    }

    public bool HasRemainingActions()
    {
        return actionPoints > 0;
    }

    public void UseAbility(Ability ability, Unit target)
    {
        if (actionPoints >= ability.actionPointCost && ability.currentCooldown <= 0)
        {
            ability.Use(this, target);
            actionPoints -= ability.actionPointCost;
            ability.currentCooldown = ability.cooldown;
        }
        else
        {
            Debug.Log("Cannot use ability: Not enough action points or ability is on cooldown.");
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (inventory.Contains(weapon))
        {
            equippedWeapon = weapon;
            UpdateStatsBasedOnClassAndWeapon();
        }
        else
        {
            Debug.Log("Cannot equip weapon: Weapon not in inventory.");
        }
    }

    public void AddWeaponToInventory(Weapon weapon)
    {
        if (!inventory.Contains(weapon))
        {
            inventory.Add(weapon);
        }
    }
}