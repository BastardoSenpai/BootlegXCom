using UnityEngine;

public static class LineOfSight
{
    public static bool HasLineOfSight(Vector3 start, Vector3 end, LayerMask obstacleLayer, GridSystem gridSystem)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, distance, obstacleLayer))
        {
            // Check if the hit object is cover
            Cell hitCell = gridSystem.GetCellAtPosition(hit.point);
            if (hitCell != null && hitCell.CoverType != CoverType.None)
            {
                // If it's cover, check if it's the target's position
                if (Vector3.Distance(hit.point, end) < 0.1f)
                {
                    return true; // The target is in cover, but we can still see it
                }
                return false; // Line of sight is blocked by cover
            }
            return false; // Line of sight is blocked by an obstacle
        }

        return true; // Clear line of sight
    }

    public static float CalculateHitChance(Unit attacker, Unit target, GridSystem gridSystem)
    {
        float baseHitChance = attacker.accuracy / 100f;
        float distance = Vector3.Distance(attacker.transform.position, target.transform.position);
        float distanceModifier = Mathf.Clamp01(1f - (distance / attacker.attackRange));

        // Check if the target is in cover
        Cell targetCell = gridSystem.GetCellAtPosition(target.transform.position);
        float coverModifier = 1f;
        if (targetCell != null)
        {
            switch (targetCell.CoverType)
            {
                case CoverType.Half:
                    coverModifier = 0.5f;
                    break;
                case CoverType.Full:
                    coverModifier = 0.25f;
                    break;
            }
        }

        // Calculate angle modifier
        Vector3 toTarget = target.transform.position - attacker.transform.position;
        float angle = Vector3.Angle(attacker.transform.forward, toTarget);
        float angleModifier = Mathf.Clamp01(1f - (angle / 90f));

        // Apply modifiers
        float finalHitChance = baseHitChance * distanceModifier * coverModifier * angleModifier;

        return Mathf.Clamp01(finalHitChance);
    }
}