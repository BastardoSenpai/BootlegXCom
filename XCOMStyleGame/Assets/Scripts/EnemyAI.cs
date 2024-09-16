using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum EnemyBehavior { Aggressive, Defensive, Patrol, Guard, Flee }

public class EnemyAI : MonoBehaviour
{
    private GridSystem gridSystem;
    private CombatManager combatManager;
    private TurnManager turnManager;
    private MissionManager missionManager;

    public float aggressiveThreshold = 0.7f;
    public float defensiveThreshold = 0.3f;

    void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        combatManager = FindObjectOfType<CombatManager>();
        turnManager = FindObjectOfType<TurnManager>();
        missionManager = FindObjectOfType<MissionManager>();
    }

    public void PerformTurn(Unit enemyUnit)
    {
        EnemyBehavior behavior = DetermineBehavior(enemyUnit);
        
        while (enemyUnit.HasRemainingActions())
        {
            AIAction bestAction = GetBestAction(enemyUnit, behavior);
            ExecuteAction(enemyUnit, bestAction);
        }
    }

    private EnemyBehavior DetermineBehavior(Unit enemyUnit)
    {
        float healthPercentage = (float)enemyUnit.currentHealth / enemyUnit.maxHealth;
        
        if (healthPercentage <= defensiveThreshold)
        {
            return EnemyBehavior.Defensive;
        }
        else if (healthPercentage >= aggressiveThreshold)
        {
            return EnemyBehavior.Aggressive;
        }
        
        switch (missionManager.currentMissionType)
        {
            case MissionType.Elimination:
            case MissionType.BossEncounter:
                return EnemyBehavior.Aggressive;
            case MissionType.DefendPosition:
            case MissionType.HackTerminal:
                return EnemyBehavior.Guard;
            case MissionType.Extraction:
            case MissionType.VIPRescue:
                return EnemyBehavior.Patrol;
            default:
                return Random.value > 0.5f ? EnemyBehavior.Aggressive : EnemyBehavior.Defensive;
        }
    }

    private AIAction GetBestAction(Unit enemyUnit, EnemyBehavior behavior)
    {
        List<AIAction> possibleActions = new List<AIAction>();
        List<Unit> playerUnits = turnManager.playerUnits;

        // Check for attack opportunities
        foreach (Unit target in playerUnits)
        {
            if (enemyUnit.CanAttack(target))
            {
                float hitChance = combatManager.CalculateHitChance(enemyUnit, target);
                float score = hitChance;
                
                if (behavior == EnemyBehavior.Aggressive)
                {
                    score *= 1.5f;
                }
                else if (behavior == EnemyBehavior.Defensive)
                {
                    score *= 0.5f;
                }
                
                possibleActions.Add(new AIAction { type = AIActionType.Attack, target = target, score = score });
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
                    float score = 0.8f;
                    if (behavior == EnemyBehavior.Aggressive && ability is OffensiveAbility)
                    {
                        score *= 1.5f;
                    }
                    else if (behavior == EnemyBehavior.Defensive && ability is SupportAbility)
                    {
                        score *= 1.5f;
                    }
                    possibleActions.Add(new AIAction { type = AIActionType.UseAbility, ability = ability, target = bestTarget, score = score });
                }
            }
        }

        // Check for movement opportunities
        Cell bestMoveCell = GetBestMovePosition(enemyUnit, playerUnits, behavior);
        if (bestMoveCell != null)
        {
            float score = 0.5f;
            if (behavior == EnemyBehavior.Patrol || behavior == EnemyBehavior.Guard)
            {
                score *= 1.5f;
            }
            possibleActions.Add(new AIAction { type = AIActionType.Move, targetCell = bestMoveCell, score = score });
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
        if (ability is OffensiveAbility)
        {
            return playerUnits.OrderBy(u => u.currentHealth).FirstOrDefault();
        }
        else if (ability is SupportAbility)
        {
            return turnManager.enemyUnits
                .Where(u => u != enemyUnit && u.currentHealth < u.maxHealth)
                .OrderBy(u => u.currentHealth)
                .FirstOrDefault();
        }
        return null;
    }

    private Cell GetBestMovePosition(Unit enemyUnit, List<Unit> playerUnits, EnemyBehavior behavior)
    {
        List<Cell> reachableCells = gridSystem.GetCellsInRange(enemyUnit.transform.position, enemyUnit.movementRange);
        
        return reachableCells
            .OrderByDescending(cell => EvaluateCellScore(cell, enemyUnit, playerUnits, behavior))
            .FirstOrDefault();
    }

    private float EvaluateCellScore(Cell cell, Unit enemyUnit, List<Unit> playerUnits, EnemyBehavior behavior)
    {
        float score = 0f;

        // Prefer cells with cover
        CoverType coverType = gridSystem.GetCoverTypeAtPosition(cell.WorldPosition);
        score += coverType == CoverType.Full ? 2f : (coverType == CoverType.Half ? 1f : 0f);

        Unit nearestPlayer = playerUnits.OrderBy(u => Vector3.Distance(cell.WorldPosition, u.transform.position)).FirstOrDefault();
        if (nearestPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(cell.WorldPosition, nearestPlayer.transform.position);
            
            switch (behavior)
            {
                case EnemyBehavior.Aggressive:
                    score += Mathf.Clamp(10f - distanceToPlayer, 0f, 5f);
                    break;
                case EnemyBehavior.Defensive:
                    score += Mathf.Clamp(distanceToPlayer, 0f, 5f);
                    break;
                case EnemyBehavior.Patrol:
                    score += Mathf.Clamp(5f - Mathf.Abs(5f - distanceToPlayer), 0f, 5f);
                    break;
                case EnemyBehavior.Guard:
                    Cell objectiveCell = GetObjectiveCell();
                    if (objectiveCell != null)
                    {
                        float distanceToObjective = Vector3.Distance(cell.WorldPosition, objectiveCell.WorldPosition);
                        score += Mathf.Clamp(10f - distanceToObjective, 0f, 5f);
                    }
                    break;
                case EnemyBehavior.Flee:
                    score += Mathf.Clamp(distanceToPlayer, 0f, 10f);
                    break;
            }
        }

        return score;
    }

    private Cell GetObjectiveCell()
    {
        switch (missionManager.currentMissionType)
        {
            case MissionType.DefendPosition:
                return missionManager.defensePosition;
            case MissionType.HackTerminal:
                return missionManager.terminalLocation;
            default:
                return null;
        }
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