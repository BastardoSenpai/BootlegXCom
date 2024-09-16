using UnityEngine;
using System.Collections.Generic;

public class BossEnemy : Unit
{
    public string bossName;
    public List<Ability> specialAbilities = new List<Ability>();
    public int phase = 1;
    public int maxPhases = 3;

    private int initialHealth;

    protected override void Start()
    {
        base.Start();
        initialHealth = maxHealth;
        InitializeSpecialAbilities();
    }

    private void InitializeSpecialAbilities()
    {
        // Example special abilities
        specialAbilities.Add(new OffensiveAbility
        {
            name = "Area Attack",
            description = "Attacks all enemies in a 3x3 area",
            actionPointCost = 2,
            cooldown = 3,
            damageBonus = 5,
            accuracyBonus = -10
        });

        specialAbilities.Add(new SupportAbility
        {
            name = "Summon Minions",
            description = "Summons 2 minions to aid in battle",
            actionPointCost = 3,
            cooldown = 4
        });

        specialAbilities.Add(new OffensiveAbility
        {
            name = "Rage Mode",
            description = "Doubles damage but halves defense for 2 turns",
            actionPointCost = 1,
            cooldown = 5
        });
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        CheckPhaseTransition();
    }

    private void CheckPhaseTransition()
    {
        float healthPercentage = (float)currentHealth / initialHealth;
        int newPhase = Mathf.Clamp(maxPhases - Mathf.FloorToInt(healthPercentage * maxPhases) + 1, 1, maxPhases);

        if (newPhase > phase)
        {
            TransitionToNewPhase(newPhase);
        }
    }

    private void TransitionToNewPhase(int newPhase)
    {
        phase = newPhase;
        Debug.Log($"{bossName} has entered phase {phase}!");

        // Modify boss stats or behavior based on the new phase
        switch (phase)
        {
            case 2:
                accuracy += 10;
                movementRange += 1;
                break;
            case 3:
                damage += 5;
                actionPoints += 1;
                maxActionPoints += 1;
                break;
        }

        // Unlock new abilities or modify existing ones
        if (phase == 2)
        {
            specialAbilities.Add(new OffensiveAbility
            {
                name = "Devastating Strike",
                description = "A powerful attack with a high chance to stun",
                actionPointCost = 2,
                cooldown = 3,
                damageBonus = 10
            });
        }

        // Heal the boss a bit when transitioning phases
        Heal(maxHealth / 10);
    }

    public override void StartTurn()
    {
        base.StartTurn();
        
        // Reset cooldowns for special abilities
        foreach (var ability in specialAbilities)
        {
            if (ability.currentCooldown > 0)
            {
                ability.currentCooldown--;
            }
        }
    }

    public void UseSpecialAbility(Ability ability, List<Unit> targets)
    {
        if (actionPoints >= ability.actionPointCost && ability.currentCooldown <= 0)
        {
            switch (ability.name)
            {
                case "Area Attack":
                    PerformAreaAttack(targets);
                    break;
                case "Summon Minions":
                    SummonMinions();
                    break;
                case "Rage Mode":
                    ActivateRageMode();
                    break;
                case "Devastating Strike":
                    PerformDevastatingStrike(targets[0]);
                    break;
            }

            actionPoints -= ability.actionPointCost;
            ability.currentCooldown = ability.cooldown;
        }
    }

    private void PerformAreaAttack(List<Unit> targets)
    {
        foreach (var target in targets)
        {
            int damage = Mathf.RoundToInt(this.damage * 0.7f);
            target.TakeDamage(damage);
        }
        Debug.Log($"{bossName} performed an Area Attack!");
    }

    private void SummonMinions()
    {
        // This should be implemented in coordination with the MissionManager or EnemyManager
        Debug.Log($"{bossName} summoned minions!");
    }

    private void ActivateRageMode()
    {
        damage *= 2;
        // Implement a way to halve defense for 2 turns
        Debug.Log($"{bossName} activated Rage Mode!");
    }

    private void PerformDevastatingStrike(Unit target)
    {
        int damage = Mathf.RoundToInt(this.damage * 1.5f);
        target.TakeDamage(damage);
        // Implement a way to potentially stun the target
        Debug.Log($"{bossName} performed a Devastating Strike on {target.unitName}!");
    }
}