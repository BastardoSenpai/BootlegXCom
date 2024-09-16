using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public static bool HasLineOfSight(Vector3 start, Vector3 end, LayerMask obstacleLayer)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, distance, obstacleLayer))
        {
            return false; // Line of sight is blocked
        }

        return true; // Clear line of sight
    }

    public static float CalculateHitChance(Vector3 attackerPosition, Vector3 targetPosition, float baseHitChance)
    {
        Vector3 toTarget = targetPosition - attackerPosition;
        float distance = toTarget.magnitude;
        float angle = Vector3.Angle(attackerPosition.forward, toTarget);

        // Adjust hit chance based on distance and angle
        float distanceModifier = Mathf.Clamp01(1f - (distance / 10f)); // Assume max effective range is 10 units
        float angleModifier = Mathf.Clamp01(1f - (angle / 90f)); // Assume 90 degrees is the max angle for shooting

        return baseHitChance * distanceModifier * angleModifier;
    }
}