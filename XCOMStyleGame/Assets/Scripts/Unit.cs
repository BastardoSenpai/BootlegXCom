using UnityEngine;

public class Unit : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int movementRange = 5;
    public int attackRange = 2;

    private Cell currentCell;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetPosition(Cell cell)
    {
        if (currentCell != null)
        {
            currentCell.IsOccupied = false;
        }

        currentCell = cell;
        currentCell.IsOccupied = true;
        transform.position = cell.WorldPosition;
    }

    public bool CanMoveTo(Cell targetCell)
    {
        // Implement pathfinding logic here
        // For simplicity, we'll just check if the target cell is within movement range
        return Vector3.Distance(currentCell.WorldPosition, targetCell.WorldPosition) <= movementRange;
    }

    public bool CanAttack(Unit target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
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
        // Implement death logic (e.g., remove from game, play animation)
        Destroy(gameObject);
    }
}