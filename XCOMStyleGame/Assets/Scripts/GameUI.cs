using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TMP_Text currentUnitText;
    public TMP_Text actionPointsText;
    public Button endTurnButton;
    public Button attackButton;
    public Button moveButton;

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
}