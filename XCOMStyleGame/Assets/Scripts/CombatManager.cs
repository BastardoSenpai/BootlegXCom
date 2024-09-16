using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public LayerMask obstacleLayer;
    private GridSystem gridSystem;

    void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();
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

        float hitChance = CalculateHitChance(attacker, target);
        bool isHit = Random.value <= hitChance;

        if (isHit)
        {
            int damage = CalculateDamage(attacker, target);
            target.TakeDamage(damage);
            Debug.Log($"{attacker.unitName} hit {target.unitName} for {damage} damage!");
        }
        else
        {
            Debug.Log($"{attacker.unitName} missed {target.unitName}!");
            HandleMissedShot(attacker, target);
        }

        attacker.Attack(target);
        return isHit;
    }

    private void HandleMissedShot(Unit attacker, Unit target)
    {
        Vector3 shotDirection = (target.transform.position - attacker.transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(attacker.transform.position, shotDirection, out hit, attacker.attackRange, obstacleLayer))
        {
            DestructibleObject destructible = hit.collider.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                int damage = CalculateDamage(attacker, null) / 2; // Reduced damage for missed shots
                destructible.TakeDamage(damage);
                Debug.Log($"{attacker.unitName}'s missed shot hit destructible object for {damage} damage!");
            }
        }
    }

    public float CalculateHitChance(Unit attacker, Unit target)
    {
        float baseHitChance = attacker.accuracy / 100f;
        float weaponAccuracy = attacker.equippedWeapon.GetAccuracy() / 100f;
        float distance = Vector3.Distance(attacker.transform.position, target.transform.position);
        float distanceModifier = Mathf.Clamp01(1f - (distance / attacker.attackRange));

        // Apply cover modifier
        float coverModifier = GetCoverModifier(target);

        // Calculate angle modifier
        Vector3 toTarget = target.transform.position - attacker.transform.position;
        float angle = Vector3.Angle(attacker.transform.forward, toTarget);
        float angleModifier = Mathf.Clamp01(1f - (angle / 90f));

        // Apply modifiers
        float finalHitChance = (baseHitChance + weaponAccuracy) * distanceModifier * coverModifier * angleModifier;

        return Mathf.Clamp01(finalHitChance);
    }

    private float GetCoverModifier(Unit target)
    {
        CoverType coverType = target.GetCurrentCoverType();
        switch (coverType)
        {
            case CoverType.Half:
                return 0.7f;
            case CoverType.Full:
                return 0.5f;
            default:
                return 1f;
        }
    }

    private int CalculateDamage(Unit attacker, Unit target)
    {
        int baseDamage = attacker.equippedWeapon.GetDamage();
        float critChance = attacker.equippedWeapon.GetCriticalChance() / 100f;

        // Apply class-specific bonuses
        if (attacker.soldierClass.classType == SoldierClassType.Heavy)
        {
            baseDamage = Mathf.RoundToInt(baseDamage * 1.2f);
        }

        // Check for critical hit
        if (Random.value <= critChance)
        {
            baseDamage = Mathf.RoundToInt(baseDamage * 1.5f);
            Debug.Log("Critical hit!");
        }

        // Apply cover damage reduction if target is not null
        if (target != null)
        {
            float coverDamageReduction = 1f - (GetCoverModifier(target) - 0.5f) * 2f;
            baseDamage = Mathf.RoundToInt(baseDamage * coverDamageReduction);
        }

        return Mathf.Max(1, baseDamage); // Ensure at least 1 damage is dealt
    }
}