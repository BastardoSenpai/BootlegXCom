using UnityEngine;
using System.Collections.Generic;

public enum EnvironmentalObjectType
{
    Hazard,
    Interactive
}

public class EnvironmentalObject : MonoBehaviour
{
    public string objectName;
    public EnvironmentalObjectType objectType;
    public int damage; // For hazards
    public int healAmount; // For healing objects
    public bool destroyOnUse = false;
    public int usesRemaining = 1;
    public List<Cell> affectedCells = new List<Cell>();

    private GridSystem gridSystem;

    void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        InitializeAffectedCells();
    }

    void InitializeAffectedCells()
    {
        // Add the cell this object is on
        Cell currentCell = gridSystem.GetCellAtPosition(transform.position);
        if (currentCell != null)
        {
            affectedCells.Add(currentCell);
        }

        // For area-effect objects, add surrounding cells
        if (objectName == "Acid Pool" || objectName == "Explosive Barrel")
        {
            List<Cell> neighbors = gridSystem.GetNeighbors(currentCell);
            affectedCells.AddRange(neighbors);
        }
    }

    public void Interact(Unit unit)
    {
        switch (objectName)
        {
            case "Acid Pool":
                ApplyAcidDamage(unit);
                break;
            case "Laser Grid":
                ApplyLaserDamage(unit);
                break;
            case "Healing Station":
                HealUnit(unit);
                break;
            case "Explosive Barrel":
                Explode();
                break;
            case "Cover Generator":
                GenerateCover();
                break;
            default:
                Debug.LogWarning($"No interaction defined for {objectName}");
                break;
        }

        usesRemaining--;
        if (destroyOnUse && usesRemaining <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ApplyAcidDamage(Unit unit)
    {
        unit.TakeDamage(damage);
        // Apply a damage-over-time effect
        unit.gameObject.AddComponent<AcidEffect>().Initialize(2, 3); // 2 damage per turn for 3 turns
        Debug.Log($"{unit.unitName} stepped in acid and took {damage} damage!");
    }

    private void ApplyLaserDamage(Unit unit)
    {
        unit.TakeDamage(damage);
        Debug.Log($"{unit.unitName} was hit by a laser grid and took {damage} damage!");
    }

    private void HealUnit(Unit unit)
    {
        unit.Heal(healAmount);
        Debug.Log($"{unit.unitName} used a healing station and recovered {healAmount} health!");
    }

    private void Explode()
    {
        foreach (Cell cell in affectedCells)
        {
            Unit unit = FindUnitInCell(cell);
            if (unit != null)
            {
                unit.TakeDamage(damage);
            }
            // Destroy cover in affected cells
            gridSystem.DestroyObjectAtCell(cell);
        }
        Debug.Log("Explosive barrel detonated!");
    }

    private void GenerateCover()
    {
        foreach (Cell cell in affectedCells)
        {
            if (cell.CoverType == CoverType.None)
            {
                cell.CoverType = CoverType.Half;
                // Instantiate a cover visual at the cell position
                // You'll need to implement this based on your visual setup
                Debug.Log($"Generated half cover at {cell.WorldPosition}");
            }
        }
    }

    private Unit FindUnitInCell(Cell cell)
    {
        // This method should be implemented in coordination with your Unit management system
        // For now, we'll use a simple physics check
        Collider[] colliders = Physics.OverlapSphere(cell.WorldPosition, 0.5f);
        foreach (Collider collider in colliders)
        {
            Unit unit = collider.GetComponent<Unit>();
            if (unit != null)
            {
                return unit;
            }
        }
        return null;
    }
}

public class AcidEffect : MonoBehaviour
{
    private int damagePerTurn;
    private int turnsRemaining;

    public void Initialize(int damage, int duration)
    {
        damagePerTurn = damage;
        turnsRemaining = duration;
    }

    public void ApplyEffect()
    {
        if (turnsRemaining > 0)
        {
            Unit unit = GetComponent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(damagePerTurn);
                Debug.Log($"{unit.unitName} took {damagePerTurn} acid damage!");
            }
            turnsRemaining--;
        }

        if (turnsRemaining <= 0)
        {
            Destroy(this);
        }
    }
}