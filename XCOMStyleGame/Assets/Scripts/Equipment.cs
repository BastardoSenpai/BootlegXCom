using UnityEngine;

public enum EquipmentType
{
    Armor,
    Accessory,
    Consumable
}

[CreateAssetMenu(fileName = "NewEquipment", menuName = "XCOM/Equipment")]
public class Equipment : ScriptableObject
{
    public string equipmentName;
    public EquipmentType type;
    public string description;

    // Armor specific properties
    public int armorBonus;
    public int mobilityModifier;

    // Accessory specific properties
    public int healthBonus;
    public int critChanceBonus;
    public int dodgeBonus;

    // Consumable specific properties
    public bool isOneTimeUse;
    public int usesPerMission = 1;

    public virtual void ApplyEffect(Unit unit)
    {
        switch (type)
        {
            case EquipmentType.Armor:
                unit.maxHealth += armorBonus;
                unit.movementRange += mobilityModifier;
                break;
            case EquipmentType.Accessory:
                unit.maxHealth += healthBonus;
                // Crit chance and dodge might need to be implemented in the Unit class
                break;
            case EquipmentType.Consumable:
                // Consumable effects will be handled when used
                break;
        }
    }

    public virtual void RemoveEffect(Unit unit)
    {
        switch (type)
        {
            case EquipmentType.Armor:
                unit.maxHealth -= armorBonus;
                unit.movementRange -= mobilityModifier;
                break;
            case EquipmentType.Accessory:
                unit.maxHealth -= healthBonus;
                // Remove crit chance and dodge bonuses
                break;
            case EquipmentType.Consumable:
                // No need to remove effects for consumables
                break;
        }
    }
}

[CreateAssetMenu(fileName = "NewMedkit", menuName = "XCOM/Equipment/Medkit")]
public class Medkit : Equipment
{
    public int healAmount;

    public override void ApplyEffect(Unit unit)
    {
        base.ApplyEffect(unit);
        unit.Heal(healAmount);
    }
}

[CreateAssetMenu(fileName = "NewGrenade", menuName = "XCOM/Equipment/Grenade")]
public class Grenade : Equipment
{
    public int damage;
    public float radius;

    public void Explode(Vector3 position, GridSystem gridSystem, CombatManager combatManager)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (var hitCollider in hitColliders)
        {
            Unit hitUnit = hitCollider.GetComponent<Unit>();
            if (hitUnit != null)
            {
                hitUnit.TakeDamage(damage);
            }

            DestructibleObject destructible = hitCollider.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                destructible.TakeDamage(damage);
            }
        }

        // Destroy cover in the affected area
        Cell[] affectedCells = gridSystem.GetCellsInRadius(position, radius);
        foreach (var cell in affectedCells)
        {
            if (cell.CoverType != CoverType.None)
            {
                gridSystem.DestroyObjectAtCell(cell);
            }
        }
    }
}