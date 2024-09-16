using UnityEngine;
using System.Collections.Generic;

public class CharacterProgression : MonoBehaviour
{
    public int level = 1;
    public int experience = 0;
    public int skillPoints = 0;

    private Unit unit;

    [System.Serializable]
    public class Skill
    {
        public string name;
        public int level;
        public int maxLevel;
        public float[] improvements;
    }

    public List<Skill> skills = new List<Skill>
    {
        new Skill { name = "Health", level = 0, maxLevel = 5, improvements = new float[] { 10, 20, 30, 40, 50 } },
        new Skill { name = "Accuracy", level = 0, maxLevel = 5, improvements = new float[] { 5, 10, 15, 20, 25 } },
        new Skill { name = "Damage", level = 0, maxLevel = 5, improvements = new float[] { 2, 4, 6, 8, 10 } },
        new Skill { name = "Movement", level = 0, maxLevel = 3, improvements = new float[] { 1, 2, 3 } }
    };

    void Start()
    {
        unit = GetComponent<Unit>();
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
    }

    public void ImproveSkill(string skillName)
    {
        if (skillPoints <= 0)
        {
            Debug.Log("No skill points available.");
            return;
        }

        Skill skill = skills.Find(s => s.name == skillName);
        if (skill == null || skill.level >= skill.maxLevel)
        {
            Debug.Log($"Cannot improve {skillName} further.");
            return;
        }

        skill.level++;
        skillPoints--;

        ApplySkillImprovement(skill);
        Debug.Log($"{unit.unitName} improved {skillName} to level {skill.level}!");
    }

    private void ApplySkillImprovement(Skill skill)
    {
        float improvement = skill.improvements[skill.level - 1];
        switch (skill.name)
        {
            case "Health":
                unit.maxHealth += Mathf.RoundToInt(improvement);
                unit.currentHealth += Mathf.RoundToInt(improvement);
                break;
            case "Accuracy":
                unit.accuracy += Mathf.RoundToInt(improvement);
                break;
            case "Damage":
                unit.damage += Mathf.RoundToInt(improvement);
                break;
            case "Movement":
                unit.movementRange += Mathf.RoundToInt(improvement);
                break;
        }
    }
}