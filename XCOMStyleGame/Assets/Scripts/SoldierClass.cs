using UnityEngine;
using System.Collections.Generic;

public enum SoldierClassType { Assault, Sniper, Heavy, Support }

[System.Serializable]
public class Ability
{
    public string name;
    public string description;
    public int actionPointCost;
    public float cooldown;
    public float currentCooldown;

    public virtual void Use(Unit user, Unit target) { }
}

[System.Serializable]
public class OffensiveAbility : Ability
{
    public int damageBonus;
    public float accuracyBonus;

    public override void Use(Unit user, Unit target)
    {
        // Implement offensive ability logic
        int totalDamage = user.equippedWeapon.GetDamage() + damageBonus;
        float totalAccuracy = user.accuracy + accuracyBonus;
        if (Random.value <= totalAccuracy / 100f)
        {
            target.TakeDamage(totalDamage);
            Debug.Log($"{user.unitName} used {name} on {target.unitName} for {totalDamage} damage!");
        }
        else
        {
            Debug.Log($"{user.unitName} missed {target.unitName} with {name}!");
        }
    }
}

[System.Serializable]
public class SupportAbility : Ability
{
    public int healAmount;
    public float buffDuration;

    public override void Use(Unit user, Unit target)
    {
        // Implement support ability logic
        target.Heal(healAmount);
        Debug.Log($"{user.unitName} used {name} on {target.unitName}, healing for {healAmount} HP!");
    }
}

[CreateAssetMenu(fileName = "NewSoldierClass", menuName = "XCOM/Soldier Class")]
public class SoldierClass : ScriptableObject
{
    public SoldierClassType classType;
    public string className;
    public List<Ability> abilities = new List<Ability>();

    [System.Serializable]
    public class SkillTreeNode
    {
        public Ability ability;
        public List<SkillTreeNode> children = new List<SkillTreeNode>();
        public bool isUnlocked = false;
    }

    public SkillTreeNode skillTreeRoot;

    public void UnlockAbility(Ability ability)
    {
        UnlockAbilityRecursive(skillTreeRoot, ability);
    }

    private bool UnlockAbilityRecursive(SkillTreeNode node, Ability ability)
    {
        if (node.ability == ability)
        {
            node.isUnlocked = true;
            return true;
        }

        foreach (var child in node.children)
        {
            if (UnlockAbilityRecursive(child, ability))
            {
                return true;
            }
        }

        return false;
    }

    // Add more class-specific abilities here
    public static List<Ability> GetAssaultAbilities()
    {
        return new List<Ability>
        {
            new OffensiveAbility { name = "Run & Gun", description = "Move and shoot in the same turn", actionPointCost = 2, cooldown = 3, damageBonus = 0, accuracyBonus = -10 },
            new OffensiveAbility { name = "Rapid Fire", description = "Fire twice at reduced accuracy", actionPointCost = 1, cooldown = 2, damageBonus = 0, accuracyBonus = -15 },
            new SupportAbility { name = "Adrenaline Rush", description = "Gain an extra action point", actionPointCost = 0, cooldown = 4, healAmount = 0, buffDuration = 1 }
        };
    }

    public static List<Ability> GetSniperAbilities()
    {
        return new List<Ability>
        {
            new OffensiveAbility { name = "Headshot", description = "Powerful shot with increased crit chance", actionPointCost = 2, cooldown = 3, damageBonus = 5, accuracyBonus = 10 },
            new OffensiveAbility { name = "Squadsight", description = "Shoot at any visible enemy in range", actionPointCost = 1, cooldown = 0, damageBonus = 0, accuracyBonus = 0 },
            new SupportAbility { name = "Steady Hands", description = "Increase accuracy for the next turn", actionPointCost = 1, cooldown = 3, healAmount = 0, buffDuration = 1 }
        };
    }

    public static List<Ability> GetHeavyAbilities()
    {
        return new List<Ability>
        {
            new OffensiveAbility { name = "Suppression", description = "Reduce enemy accuracy and movement", actionPointCost = 1, cooldown = 1, damageBonus = -2, accuracyBonus = 20 },
            new OffensiveAbility { name = "Rocket Launcher", description = "Deal AoE damage", actionPointCost = 2, cooldown = 4, damageBonus = 10, accuracyBonus = -20 },
            new SupportAbility { name = "Hunker Down", description = "Greatly increase defense for one turn", actionPointCost = 1, cooldown = 2, healAmount = 0, buffDuration = 1 }
        };
    }

    public static List<Ability> GetSupportAbilities()
    {
        return new List<Ability>
        {
            new SupportAbility { name = "Medikit", description = "Heal an ally", actionPointCost = 1, cooldown = 1, healAmount = 20, buffDuration = 0 },
            new SupportAbility { name = "Smoke Grenade", description = "Increase defense for nearby allies", actionPointCost = 1, cooldown = 3, healAmount = 0, buffDuration = 2 },
            new OffensiveAbility { name = "Overwatch", description = "React to enemy movement", actionPointCost = 1, cooldown = 0, damageBonus = 0, accuracyBonus = -10 }
        };
    }
}