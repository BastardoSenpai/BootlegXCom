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
}