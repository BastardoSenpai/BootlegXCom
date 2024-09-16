using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    public TMP_Text currentUnitText;
    public TMP_Text actionPointsText;
    public TMP_Text missionInfoText;
    public TMP_Text selectedUnitInfoText;
    public Button endTurnButton;
    public Button attackButton;
    public Button moveButton;
    public GameObject gameModePanel;
    public TMP_Text gameModeText;
    public GameObject abilityPanel;
    public GameObject abilityButtonPrefab;
    public GameObject skillTreePanel;
    public GameObject skillTreeNodePrefab;

    private GameManager gameManager;
    private TurnManager turnManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        turnManager = FindObjectOfType<TurnManager>();

        endTurnButton.onClick.AddListener(EndTurn);
        attackButton.onClick.AddListener(SetAttackMode);
        moveButton.onClick.AddListener(SetMoveMode);

        turnManager.OnTurnChange += UpdateUI;
    }

    void UpdateUI(Unit currentUnit)
    {
        currentUnitText.text = $"Current Unit: {currentUnit.unitName} ({currentUnit.soldierClass.className})";
        actionPointsText.text = $"Action Points: {currentUnit.actionPoints}";

        bool isPlayerTurn = turnManager.IsPlayerTurn();
        endTurnButton.gameObject.SetActive(isPlayerTurn);
        attackButton.gameObject.SetActive(isPlayerTurn);
        moveButton.gameObject.SetActive(isPlayerTurn);

        UpdateAbilityPanel(currentUnit);
    }

    public void UpdateSelectedUnitInfo(Unit unit)
    {
        if (unit != null)
        {
            selectedUnitInfoText.text = $"Selected Unit: {unit.unitName}\n" +
                                        $"Class: {unit.soldierClass.className}\n" +
                                        $"Health: {unit.currentHealth}/{unit.maxHealth}\n" +
                                        $"Action Points: {unit.actionPoints}/{unit.maxActionPoints}\n" +
                                        $"Movement Range: {unit.movementRange}\n" +
                                        $"Attack Range: {unit.attackRange}\n" +
                                        $"Accuracy: {unit.accuracy}\n" +
                                        $"Weapon: {unit.equippedWeapon.weaponName}";

            UpdateSkillTreePanel(unit);
        }
        else
        {
            selectedUnitInfoText.text = "No unit selected";
            skillTreePanel.SetActive(false);
        }
    }

    public void UpdateMissionInfo(string missionStatus)
    {
        missionInfoText.text = missionStatus;
    }

    void EndTurn()
    {
        turnManager.EndCurrentTurn();
    }

    void SetAttackMode()
    {
        gameManager.SetGameMode(GameMode.Attack);
    }

    void SetMoveMode()
    {
        gameManager.SetGameMode(GameMode.Move);
    }

    public void UpdateGameMode(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.Normal:
                gameModePanel.SetActive(false);
                break;
            case GameMode.Move:
                gameModePanel.SetActive(true);
                gameModeText.text = "Move Mode: Select a cell to move";
                break;
            case GameMode.Attack:
                gameModePanel.SetActive(true);
                gameModeText.text = "Attack Mode: Select an enemy to attack";
                break;
            case GameMode.UseAbility:
                gameModePanel.SetActive(true);
                gameModeText.text = "Ability Mode: Select a target for the ability";
                break;
        }
    }

    void UpdateAbilityPanel(Unit unit)
    {
        foreach (Transform child in abilityPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (unit.soldierClass != null)
        {
            foreach (Ability ability in unit.soldierClass.abilities)
            {
                GameObject abilityButton = Instantiate(abilityButtonPrefab, abilityPanel.transform);
                abilityButton.GetComponentInChildren<TMP_Text>().text = ability.name;
                abilityButton.GetComponent<Button>().onClick.AddListener(() => gameManager.UseAbility(ability));
            }
        }
    }

    void UpdateSkillTreePanel(Unit unit)
    {
        foreach (Transform child in skillTreePanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (unit.soldierClass != null)
        {
            CreateSkillTreeNodeUI(unit.soldierClass.skillTreeRoot, Vector2.zero);
        }

        skillTreePanel.SetActive(true);
    }

    void CreateSkillTreeNodeUI(SoldierClass.SkillTreeNode node, Vector2 position)
    {
        GameObject nodeObject = Instantiate(skillTreeNodePrefab, skillTreePanel.transform);
        RectTransform rectTransform = nodeObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        TMP_Text nodeText = nodeObject.GetComponentInChildren<TMP_Text>();
        nodeText.text = node.ability.name;

        Button nodeButton = nodeObject.GetComponent<Button>();
        nodeButton.onClick.AddListener(() => gameManager.ImproveAbility(node.ability));

        if (node.isUnlocked)
        {
            nodeButton.interactable = false;
            nodeText.color = Color.green;
        }

        for (int i = 0; i < node.children.Count; i++)
        {
            float xOffset = (i - (node.children.Count - 1) / 2f) * 100f;
            Vector2 childPosition = position + new Vector2(xOffset, -100f);
            CreateSkillTreeNodeUI(node.children[i], childPosition);
        }
    }
}