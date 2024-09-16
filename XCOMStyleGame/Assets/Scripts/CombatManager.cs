using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public LayerMask obstacleLayer;

    public bool TryAttack(Unit attacker, Unit target)
    {
        if (!attacker.CanAttack(target))
        {
            Debug.Log("Target is out of range");
            return false;
        }

        if (!LineOfSight.HasLineOfSight(attacker.transform.position, target.transform.position, obstacleLayer))
        {
            Debug.Log("No line of sight to target");
            return false;
        }

        float hitChance = LineOfSight.CalculateHitChance(attacker.transform.position, target.transform.position, 0.7f);
        
        if (Random.value <= hitChance)
        {
            int damage = Random.Range(10, 20); // Simple damage calculation
            target.TakeDamage(damage);
            Debug.Log($"{attacker.name} hit {target.name} for {damage} damage!");
            return true;
        }
        else
        {
            Debug.Log($"{attacker.name} missed {target.name}!");
            return false;
        }
    }
}