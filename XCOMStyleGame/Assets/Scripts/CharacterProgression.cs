using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Perk
{
    public string name;
    public string description;
    public bool isUnlocked;
    public List<string> prerequisites = new List<string>();
}

[System.Serializable]
public class Specialization
{
    public string name;
    public List<Perk> perks = new List<Perk>();
}

public class CharacterProgression : MonoBehaviour
{
    public int level = 1;
    public int experience = 0;
    public int skillPoints = 0;

    private Unit unit;
    public List<Specialization> specializations = new List<Specialization>();
    public Specialization currentSpecialization;

    void Start()
    {
        unit = GetComponent<Unit>();
        InitializeSpecializations();
    }

    void InitializeSpecializations()
    {
        // Define specializations and perks based on the unit's class
        switch (unit.soldierClass.classType)
        {
            case SoldierClassType.Assault:
                specializations = new List<Specialization>
                {
                    new Specialization
                    {
                        name = "Close Quarter Combat",
                        perks = new List<Perk>
                        {
                            new Perk { name = "Run & Gun", description = "Move and shoot in the same turn" },
                            new Perk { name = "Close and Personal", description = "Bonus damage at close range", prerequisites = new List<string> { "Run & Gun" } },
                            new Perk { name = "Killer Instinct", description = "Increased critical chance", prerequisites = new List<string> { "Close and Personal" } }
                        }
                    },
                    new Specialization
                    {
                        name = "Tactical Assault",
                        perks = new List<Perk>
                        {
                            new Perk { name = "Tactical Sense", description = "Bonus defense for each enemy in sight" },
                            new Perk { name = "Aggression", description = "Bonus crit chance for each enemy in sight", prerequisites = new List<string> { "Tactical Sense" } },
                            new Perk { name = "Bring 'Em On", description = "Bonus damage based on enemies in sight", prerequisites = new List<string> { "Aggression" } }
                        }
                    }
                };
                break;
            // Add specializations for other classes (Sniper, Heavy, Support) here
        }
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

    public void UnlockPerk(string perkName)
    {
        if (skillPoints <= 0)
        {
            Debug.Log("No skill points available.");
            return;
        }

        Perk perk = currentSpecialization.perks.Find(p => p.name == perkName && !p.isUnlocked);
        if (perk != null && CanUnlockPerk(perk))
        {
            perk.isUnlocked = true;
            skillPoints--;
            ApplyPerkEffects(perk);
            Debug.Log($"{unit.unitName} unlocked the perk: {perk.name}!");
        }
        else
        {
            Debug.Log($"Cannot unlock perk: {perkName}. Make sure it's available and its prerequisites are met.");
        }
    }

    private bool CanUnlockPerk(Perk perk)
    {
        return perk.prerequisites.All(prereq => currentSpecialization.perks.Find(p => p.name == prereq)?.isUnlocked ?? false);
    }

    private void ApplyPerkEffects(Perk perk)
    {
        // Implement perk effects here
        switch (perk.name)
        {
            case "Run & Gun":
                // Allow moving and shooting in the same turn
                break;
            case "Close and Personal":
                // Implement bonus damage at close range
                break;
            // Add cases for other perks
        }
    }

    public void ChooseSpecialization(string specializationName)
    {
        Specialization spec = specializations.Find(s => s.name == specializationName);
        if (spec != null)
        {
            currentSpecialization = spec;
            Debug.Log($"{unit.unitName} chose the {specializationName} specialization!");
        }
        else
        {
            Debug.Log($"Specialization {specializationName} not found.");
        }
    }

    public List<Perk> GetAvailablePerks()
    {
        if (currentSpecialization == null) return new List<Perk>();
        return currentSpecialization.perks.Where(p => !p.isUnlocked && CanUnlockPerk(p)).ToList();
    }
}