using UnityEngine;
using System.Collections.Generic;

public class CharacterProgression : MonoBehaviour
{
    public int level = 1;
    public int experience = 0;
    public int skillPoints = 0;

    private Unit unit;
    private SoldierClass soldierClass;

    void Start()
    {
        unit = GetComponent<Unit>();
        soldierClass = unit.soldierClass;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int experienceRequired = level * 100;
        if (experience >= experienceRequired)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        experience -= (level - 1) * 100;
        skillPoints += 2;
        Debug.Log($"{unit.unitName} leveled up to level {level}!");

        // Improve base stats
        unit.maxHealth += 5;
        unit.currentHealth += 5;
        unit.accuracy += 2;
        unit.movementRange += 1;
    }

    public void ImproveAbility(Ability ability)
    {
        if (skillPoints <= 0)
        {
            Debug.Log("No skill points available.");
            return;
        }

        if (soldierClass.UnlockAbility(ability))
        {
            skillPoints--;
            Debug.Log($"{unit.unitName} unlocked the ability: {ability.name}!");
        }
        else
        {
            Debug.Log($"Cannot unlock ability: {ability.name}. Make sure it's in the skill tree and its prerequisites are met.");
        }
    }

    public List<Ability> GetAvailableAbilities()
    {
        List<Ability> availableAbilities = new List<Ability>();
        CheckAvailableAbilitiesRecursive(soldierClass.skillTreeRoot, availableAbilities);
        return availableAbilities;
    }

    private void CheckAvailableAbilitiesRecursive(SoldierClass.SkillTreeNode node, List<Ability> availableAbilities)
    {
        if (!node.isUnlocked && CanUnlockNode(node))
        {
            availableAbilities.Add(node.ability);
        }

        foreach (var child in node.children)
        {
            CheckAvailableAbilitiesRecursive(child, availableAbilities);
        }
    }

    private bool CanUnlockNode(SoldierClass.SkillTreeNode node)
    {
        if (node == soldierClass.skillTreeRoot)
        {
            return true;
        }

        var parent = FindParentNode(soldierClass.skillTreeRoot, node);
        return parent != null && parent.isUnlocked;
    }

    private SoldierClass.SkillTreeNode FindParentNode(SoldierClass.SkillTreeNode current, SoldierClass.SkillTreeNode target)
    {
        foreach (var child in current.children)
        {
            if (child == target)
            {
                return current;
            }

            var result = FindParentNode(child, target);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}