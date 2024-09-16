using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public LayerMask obstacleLayer;
    private GridSystem gridSystem;

    void Start()
    {
        gridSystem = GetComponent<GridSystem>();
    }

    public bool TryAttack(Unit attacker, Unit target)
    {
        if (!attacker.CanAttack(target))
        {
            Debug.Log("Target is out of range or attacker has no action points");
            return false;
        }

        if (!LineOfSight.HasLineOfSight(attacker.transform.position, target.transform.position, obstacleLayer, gridSystem))
        {
            Debug.Log("No line of sight to target");
            return false;
        }

        float hitChance = LineOfSight.CalculateHitChance(attacker, target, gridSystem);

        if (Random.value <= hitChance)
        {
            int damage = CalculateDamage(attacker, target);
            target.TakeDamage(damage);
            Debug.Log($"{attacker.unitName} hit {target.unitName} for {damage} damage!");
            attacker.Attack(target);
            return true;
        }
        else
        {
            Debug.Log($"{attacker.unitName} missed {target.unitName}!");
            attacker.Attack(target);
            return false;
        }
    }

    private int CalculateDamage(Unit attacker, Unit target)
    {
        float baseDamage = attacker.damage;
        float critChance = 0.1f; // 10% base crit chance

        // Increase crit chance for Snipers
        if (attacker.type == UnitType.Sniper)
        {
            critChance += 0.1f;
        }

        // Increase damage for Heavy units
        if (attacker.type == UnitType.Heavy)
        {
            baseDamage *= 1.2f;
        }

        // Apply cover damage reduction
        Cell targetCell = gridSystem.GetCellAtPosition(target.transform.position);
        if (targetCell != null)
        {
            switch (targetCell.CoverType)
            {
                case CoverType.Half:
                    baseDamage *= 0.75f;
                    break;
                case CoverType.Full:
                    baseDamage *= 0.5f;
                    break;
            }
        }

        // Critical hit
        if (Random.value <= critChance)
        {
            baseDamage *= 1.5f;
            Debug.Log("Critical hit!");
        }

        return Mathf.RoundToInt(baseDamage);
    }
}