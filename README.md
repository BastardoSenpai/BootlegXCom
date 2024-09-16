# Created by Claude Sonnet 3.5 & ClaudeDev

## To set up and test the expanded game in Unity:

Open Unity and create a new 3D project.
Ensure all scripts are up to date in your Unity project.
Set up the Unity scene, including creating materials, configuring the camera, and adding components to the GameManager GameObject.

Create the following scenes if you haven't already:
- MainMenu
- CampaignMenu
- CharacterCustomization
- WorldMap
- BaseManagement
- ResearchLab
- MissionScene

### In the MainMenu scene:
Add UI elements for starting a new campaign, loading a saved game, and selecting difficulty level.
Attach the DifficultyManager script to an empty GameObject.

### In the CampaignMenu scene:
Update the UI to display soldier information, including their customized appearance.
Add buttons to access the CharacterCustomization scene for each soldier.

### Create a new CharacterCustomization scene:
Implement UI elements for customizing soldier appearance (hair, face, armor colors, etc.).
Use the SoldierCustomization script to manage customization options.

### In the WorldMap scene:
Update the mission selection UI to display the new varied mission types and objectives.

### In the BaseManagement scene:
Create UI elements for managing facilities, resources, and base expansion.
Attach the BaseManager script to handle base management logic.

### Create a new ResearchLab scene:
Implement UI for the tech tree and research projects.
Attach the ResearchManager script to handle research logic.

### In the MissionScene:
Update the MapGenerator to include environmental hazards and interactive objects.
Implement the new mission types and objectives in the MissionManager.
Add support for boss encounters using the BossEnemy script.

### Update prefabs:
Modify the Unit prefab to include new renderer components for customization.
Create prefabs for environmental hazards and interactive objects.
Create prefabs for boss enemies.

### Create ScriptableObjects for:
Different mission types with specific objectives and parameters.
Research projects and tech tree nodes.
Base facilities with their respective effects and requirements.

### Update the GameManager prefab:
Assign references to the new systems (ResearchManager, BaseManager, etc.).

## To test the expanded game:

Start from the MainMenu scene and begin a new campaign, selecting a difficulty level.

In the CampaignMenu, customize your soldiers' appearances.

In the WorldMap, observe the variety of available missions, including the new types.

Visit the BaseManagement scene to construct and upgrade facilities.

Use the ResearchLab to progress through the tech tree and unlock new technologies.

Select a mission and enter the MissionScene:

Verify that the map layout includes environmental hazards and interactive objects.

Test different mission types and objectives, including the new ones.

Observe how the difficulty settings affect gameplay.

If applicable, engage in a boss fight and test the unique mechanics.

Complete the mission and return to the WorldMap.
