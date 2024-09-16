using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    private GridSystem gridSystem;
    private CombatManager combatManager;
    private TurnManager turnManager;
    private MissionManager missionManager;

    void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        combatManager = FindObjectOfType<CombatManager>();
        turnManager = FindObjectOfType<TurnManager>();
        missionManager = FindObjectOfType<MissionManager>();
    }

    public void PerformTurn(Unit enemyUnit)
    {
        List<Unit> playerUnits = turnManager.playerUnits;
        
        while (enemyUnit.HasRemainingActions())
        {
            AIAction bestAction = GetBestAction(enemyUnit, playerUnits);
            ExecuteAction(enemyUnit, bestAction);
        }
    }

    private AIAction GetBestAction(Unit enemyUnit, List<Unit> playerUnits)
    {
        List<AIAction> possibleActions = new List<AIAction>();

        // Check for attack opportunities
        foreach (Unit target in playerUnits)
        {
            if (enemyUnit.CanAttack(target))
            {
                float hitChance = combatManager.CalculateHitChance(enemyUnit, target);
                possibleActions.Add(new AIAction { type = AIActionType.Attack, target = target, score = hitChance });
            }
        }

        // Check for ability use opportunities
        foreach (Ability ability in enemyUnit.soldierClass.abilities)
        {
            if (ability.currentCooldown <= 0 && enemyUnit.actionPoints >= ability.actionPointCost)
            {
                Unit bestTarget = GetBestAbilityTarget(enemyUnit, ability, playerUnits);
                if (bestTarget != null)
                {
                    possibleActions.Add(new AIAction { type = AIActionType.UseAbility, ability = ability, target = bestTarget, score = 0.8f });
                }
            }
        }

        // Check for movement opportunities
        Cell bestMoveCell = GetBestMovePosition(enemyUnit, playerUnits);
        if (bestMoveCell != null)
        {
            possibleActions.Add(new AIAction { type = AIActionType.Move, targetCell = bestMoveCell, score = 0.5f });
        }

        // Choose the action with the highest score
        return possibleActions.OrderByDescending(a => a.score).FirstOrDefault();
    }

    private void ExecuteAction(Unit enemyUnit, AIAction action)
    {
        switch (action.type)
        {
            case AIActionType.Attack:
                combatManager.TryAttack(enemyUnit, action.target);
                break;
            case AIActionType.UseAbility:
                enemyUnit.UseAbility(action.ability, action.target);
                break;
            case AIActionType.Move:
                enemyUnit.Move(action.targetCell);
                break;
        }
    }

    private Unit GetBestAbilityTarget(Unit enemyUnit, Ability ability, List<Unit> playerUnits)
    {
        // Implement logic to determine the best target for the ability
        // This could involve checking range, potential damage, or other factors
        return playerUnits.OrderBy(u => Vector3.Distance(enemyUnit.transform.position, u.transform.position)).FirstOrDefault();
    }

    private Cell GetBestMovePosition(Unit enemyUnit, List<Unit> playerUnits)
    {
        List<Cell> reachableCells = gridSystem.GetCellsInRange(enemyUnit.transform.position, enemyUnit.movementRange);
        
        return reachableCells
            .OrderByDescending(cell => EvaluateCellScore(cell, enemyUnit, playerUnits))
            .FirstOrDefault();
    }

    private float EvaluateCellScore(Cell cell, Unit enemyUnit, List<Unit> playerUnits)
    {
        float score = 0f;

        // Prefer cells with cover
        CoverType coverType = gridSystem.GetCoverTypeAtPosition(cell.WorldPosition);
        score += coverType == CoverType.Full ? 2f : (coverType == CoverType.Half ? 1f : 0f);

        // Prefer cells closer to player units, but not too close
        Unit nearestPlayer = playerUnits.OrderBy(u => Vector3.Distance(cell.WorldPosition, u.transform.position)).FirstOrDefault();
        if (nearestPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(cell.WorldPosition, nearestPlayer.transform.position);
            score += Mathf.Clamp(10f - distanceToPlayer, 0f, 5f);
        }

        // Consider mission objectives
        if (missionManager.currentMissionType == MissionType.DefendPosition)
        {
            float distanceToDefensePosition = Vector3.Distance(cell.WorldPosition, missionManager.defensePosition.WorldPosition);
            score += 10f / (distanceToDefensePosition + 1f);
        }

        return score;
    }
}

public enum AIActionType { Attack, UseAbility, Move }

public class AIAction
{
    public AIActionType type;
    public Unit target;
    public Ability ability;
    public Cell targetCell;
    public float score;
}