using UnityEngine;

public enum UnitType { Soldier, Sniper, Heavy }

public class Unit : MonoBehaviour
{
    public UnitType type;
    public string unitName;
    public int maxHealth;
    public int currentHealth;
    public int movementRange;
    public int attackRange;
    public int accuracy;
    public int damage;
    public int actionPoints;
    public int maxActionPoints;

    private Cell currentCell;
    public bool hasMoved = false;
    public bool hasAttacked = false;

    void Start()
    {
        SetUnitTypeAttributes();
        currentHealth = maxHealth;
        actionPoints = maxActionPoints;
    }

    void SetUnitTypeAttributes()
    {
        switch (type)
        {
            case UnitType.Soldier:
                maxHealth = 100;
                movementRange = 5;
                attackRange = 5;
                accuracy = 75;
                damage = 20;
                maxActionPoints = 2;
                break;
            case UnitType.Sniper:
                maxHealth = 80;
                movementRange = 4;
                attackRange = 10;
                accuracy = 90;
                damage = 30;
                maxActionPoints = 2;
                break;
            case UnitType.Heavy:
                maxHealth = 150;
                movementRange = 3;
                attackRange = 4;
                accuracy = 65;
                damage = 40;
                maxActionPoints = 1;
                break;
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
}