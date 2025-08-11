using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathfindingUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI pathLengthText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI questInfoText;

    [Header("UI Settings")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color errorColor = Color.red;

    private PathfinderManager pathfindingManager;
    private JsonLoader jsonLoader;

    private void Start()
    {
        // Find required components
        pathfindingManager = FindAnyObjectByType<PathfinderManager>();
        jsonLoader = FindAnyObjectByType<JsonLoader>();

        // Subscribe to character movement events
        GridCharacterMovement character = FindAnyObjectByType<GridCharacterMovement>();
        if (character != null)
        {
            character.OnPathCompleted += OnPathCompleted;
        }

        // Initialize UI
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (jsonLoader != null)
        {
            UpdateQuestInfo();
        }

        UpdateStatus();
    }

    private void UpdateQuestInfo()
    {
        if (questInfoText != null)
        {
            int questIndex = jsonLoader.GetQuestIndex();
            var mapData = jsonLoader.GetMapData();

            if (mapData?.quests != null && questIndex < mapData.quests.Length)
            {
                var quest = mapData.quests[questIndex];
                questInfoText.text = $"Quest {questIndex + 1}: ({quest.from.x}, {quest.from.y}) → ({quest.to.x}, {quest.to.y})";
                questInfoText.color = normalColor;
            }
            else
            {
                questInfoText.text = "No Quest Data";
                questInfoText.color = errorColor;
            }
        }
    }

    private void UpdateStatus()
    {
        if (statusText != null)
        {
            GridCharacterMovement character = FindAnyObjectByType<GridCharacterMovement>();
            if (character != null)
            {
                if (character.manualInputEnabled)
                {
                    statusText.text = "Status: Manual Control (WASD/Arrows)";
                    statusText.color = highlightColor;
                }
                else if (character.IsFollowingPath())
                {
                    int currentMove = character.GetCurrentMoveNumber();
                    int totalMoves = character.GetTotalMoves();
                    statusText.text = $"Status: Following Path ({currentMove}/{totalMoves})";
                    statusText.color = normalColor;
                }
                else if (character.IsPathCompleted())
                {
                    statusText.text = "Status: Path Completed!";
                    statusText.color = Color.green;
                }
                else
                {
                    statusText.text = "Status: Ready";
                    statusText.color = normalColor;
                }
            }
            else
            {
                statusText.text = "Status: No Character Found";
                statusText.color = errorColor;
            }
        }
    }

    public void UpdatePathInfo(float totalCost, int totalMoves, int totalPositions)
    {
        if (costText != null)
        {
            costText.text = $"Path Cost: {totalCost:F1}";
            costText.color = totalCost > totalMoves ? highlightColor : normalColor;
        }

        if (pathLengthText != null)
        {
            pathLengthText.text = $"Moves: {totalMoves} | Positions: {totalPositions}";
            pathLengthText.color = normalColor;
        }
    }

    public void UpdatePathNotFound()
    {
        if (costText != null)
        {
            costText.text = "Path Cost: No Path Found";
            costText.color = errorColor;
        }

        if (pathLengthText != null)
        {
            pathLengthText.text = "Moves: 0 | Positions: 0";
            pathLengthText.color = errorColor;
        }
    }

    private void OnPathCompleted()
    {
        if (statusText != null)
        {
            statusText.text = "Status: Path Completed!";
            statusText.color = Color.green;
        }
    }

    // Manual buttons for testing
    public void OnFindPathButtonClicked()
    {
        if (pathfindingManager != null)
        {
            pathfindingManager.StartPathfinding();
        }
    }

    public void OnToggleManualControlButtonClicked()
    {
        GridCharacterMovement character = FindAnyObjectByType<GridCharacterMovement>();
        if (character != null)
        {
            character.ToggleManualControl();
        }
    }

    public void OnClearPathButtonClicked()
    {
        if (pathfindingManager != null)
        {
            pathfindingManager.ClearPathManual();
        }

        // Reset UI
        if (costText != null) costText.text = "Path Cost: --";
        if (pathLengthText != null) pathLengthText.text = "Moves: -- | Positions: --";
    }
}