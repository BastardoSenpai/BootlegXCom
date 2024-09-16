using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public LayerMask obstacleLayer;
    private GridSystem gridSystem;
    private DifficultyManager difficultyManager;

    void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        difficultyManager = DifficultyManager.Instance;
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
        }

        attacker.Attack(target);
        return isHit;
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

        // Apply difficulty modifier
        if (attacker.CompareTag("Player"))
        {
            finalHitChance *= difficultyManager.GetPlayerAccuracyMultiplier();
        }
        else
        {
            finalHitChance *= difficultyManager.GetEnemyAccuracyMultiplier();
        }

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
        int baseDamage = attacker.GetDamage();
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

        // Apply cover damage reduction
        float coverDamageReduction = 1f - (GetCoverModifier(target) - 0.5f) * 2f;
        int finalDamage = Mathf.RoundToInt(baseDamage * coverDamageReduction);

        return Mathf.Max(1, finalDamage); // Ensure at least 1 damage is dealt
    }
}