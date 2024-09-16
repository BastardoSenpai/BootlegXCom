using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        currentUnitText.text = $"Current Unit: {currentUnit.unitName} ({currentUnit.type})";
        actionPointsText.text = $"Action Points: {currentUnit.actionPoints}";

        bool isPlayerTurn = turnManager.IsPlayerTurn();
        endTurnButton.gameObject.SetActive(isPlayerTurn);
        attackButton.gameObject.SetActive(isPlayerTurn);
        moveButton.gameObject.SetActive(isPlayerTurn);
    }

    public void UpdateSelectedUnitInfo(Unit unit)
    {
        if (unit != null)
        {
            selectedUnitInfoText.text = $"Selected Unit: {unit.unitName}\n" +
                                        $"Type: {unit.type}\n" +
                                        $"Health: {unit.currentHealth}/{unit.maxHealth}\n" +
                                        $"Action Points: {unit.actionPoints}/{unit.maxActionPoints}\n" +
                                        $"Movement Range: {unit.movementRange}\n" +
                                        $"Attack Range: {unit.attackRange}\n" +
                                        $"Accuracy: {unit.accuracy}\n" +
                                        $"Damage: {unit.damage}";
        }
        else
        {
            selectedUnitInfoText.text = "No unit selected";
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
        }
    }
}