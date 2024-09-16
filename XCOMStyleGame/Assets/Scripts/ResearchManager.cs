using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ResearchProject
{
    public string name;
    public string description;
    public int costInDays;
    public List<string> prerequisites = new List<string>();
    public List<string> unlockedTechnologies = new List<string>();
    public List<string> unlockedItems = new List<string>();
    public bool isCompleted = false;
}

public class ResearchManager : MonoBehaviour
{
    public List<ResearchProject> availableProjects = new List<ResearchProject>();
    public List<ResearchProject> completedProjects = new List<ResearchProject>();
    public ResearchProject currentProject;

    private int currentDay = 0;
    private int remainingDays = 0;

    public delegate void ResearchCompleteDelegate(ResearchProject project);
    public event ResearchCompleteDelegate OnResearchComplete;

    private void Start()
    {
        InitializeResearchProjects();
    }

    private void InitializeResearchProjects()
    {
        // Define your research projects here
        availableProjects = new List<ResearchProject>
        {
            new ResearchProject
            {
                name = "Advanced Armor",
                description = "Improves the protective capabilities of soldier armor.",
                costInDays = 10,
                unlockedTechnologies = new List<string> { "Predator Armor" },
                unlockedItems = new List<string> { "Predator Armor" }
            },
            new ResearchProject
            {
                name = "Laser Weapons",
                description = "Unlocks powerful laser-based weaponry.",
                costInDays = 15,
                prerequisites = new List<string> { "Advanced Armor" },
                unlockedTechnologies = new List<string> { "Laser Rifle", "Laser Pistol" },
                unlockedItems = new List<string> { "Laser Rifle", "Laser Pistol" }
            },
            new ResearchProject
            {
                name = "Alien Materials",
                description = "Studies alien materials to improve our own technology.",
                costInDays = 20,
                unlockedTechnologies = new List<string> { "Alloy Plating" },
                unlockedItems = new List<string> { "Alloy Plating" }
            },
            new ResearchProject
            {
                name = "Psionic Abilities",
                description = "Unlocks the potential of psionic abilities in human soldiers.",
                costInDays = 30,
                prerequisites = new List<string> { "Alien Materials" },
                unlockedTechnologies = new List<string> { "Psi Lab", "Mind Control" },
                unlockedItems = new List<string> { "Psi Amp" }
            }
        };
    }

    public void StartResearch(string projectName)
    {
        ResearchProject project = availableProjects.Find(p => p.name == projectName);
        if (project != null && CanResearchProject(project))
        {
            currentProject = project;
            remainingDays = project.costInDays;
            availableProjects.Remove(project);
            Debug.Log($"Started research on {projectName}");
        }
        else
        {
            Debug.Log($"Cannot start research on {projectName}");
        }
    }

    public void AdvanceDay()
    {
        currentDay++;
        if (currentProject != null)
        {
            remainingDays--;
            if (remainingDays <= 0)
            {
                CompleteCurrentResearch();
            }
        }
    }

    private void CompleteCurrentResearch()
    {
        currentProject.isCompleted = true;
        completedProjects.Add(currentProject);
        Debug.Log($"Research completed: {currentProject.name}");
        OnResearchComplete?.Invoke(currentProject);

        // Unlock new research projects
        List<ResearchProject> newlyAvailable = availableProjects.Where(p => 
            p.prerequisites.All(prereq => completedProjects.Any(cp => cp.name == prereq))).ToList();

        foreach (var project in newlyAvailable)
        {
            Debug.Log($"New research available: {project.name}");
        }

        currentProject = null;
    }

    private bool CanResearchProject(ResearchProject project)
    {
        return project.prerequisites.All(prereq => completedProjects.Any(cp => cp.name == prereq));
    }

    public List<ResearchProject> GetAvailableProjects()
    {
        return availableProjects.Where(p => CanResearchProject(p)).ToList();
    }

    public float GetResearchProgress()
    {
        if (currentProject == null) return 0f;
        return 1f - (float)remainingDays / currentProject.costInDays;
    }

    public List<string> GetUnlockedTechnologies()
    {
        return completedProjects.SelectMany(p => p.unlockedTechnologies).Distinct().ToList();
    }

    public List<string> GetUnlockedItems()
    {
        return completedProjects.SelectMany(p => p.unlockedItems).Distinct().ToList();
    }
}