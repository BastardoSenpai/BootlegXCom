using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    private int currentUnitIndex = 0;

    void Start()
    {
        StartTurn();
    }

    void StartTurn()
    {
        if (units.Count == 0) return;

        Unit currentUnit = units[currentUnitIndex];
        // Activate the current unit (e.g., highlight, enable controls)
        Debug.Log($"It's {currentUnit.name}'s turn");
    }

    public void EndTurn()
    {
        currentUnitIndex = (currentUnitIndex + 1) % units.Count;
        StartTurn();
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        if (currentUnitIndex >= units.Count)
        {
            currentUnitIndex = 0;
        }
    }
}