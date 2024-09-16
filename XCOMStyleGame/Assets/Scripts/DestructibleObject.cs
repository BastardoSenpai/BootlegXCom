using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public int health = 100;
    private GridSystem gridSystem;
    private Cell associatedCell;

    public void Initialize(GridSystem grid, Cell cell)
    {
        gridSystem = grid;
        associatedCell = cell;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        gridSystem.DestroyObjectAtCell(associatedCell);
        Destroy(gameObject);
    }
}