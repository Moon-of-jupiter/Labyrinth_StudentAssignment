using System.Collections.Generic;
using UnityEngine;

public class GridCharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Grid grid;
    public float moveDelay = 0.5f;

    [Header("Character Setup")]
    [SerializeField] private GameObject charPrefab;

    [Header("Wall Detection")]
    [SerializeField] private JsonLoader jsonLoader;

    [Header("Path Following")]
    public bool manualInputEnabled = false;

    // Start at bottom-left
    private int currentGridX = 0;
    private int currentGridZ = 0;
    private float lastMoveTime;

    private GameObject spawnedCharacter;

    private List<Vector2Int> currentPath;
    private int pathIndex = 0;
    private bool isFollowingPath = false;
    private bool pathCompleted = false;

    private IMapData mapAdapter;

    private void Start()
    {
        if (grid == null) grid = FindAnyObjectByType<Grid>();
        if (jsonLoader == null) jsonLoader = FindAnyObjectByType<JsonLoader>();

        var mapData = jsonLoader.GetMapData();
        if (mapData != null)
            mapAdapter = new MapDataAdapter(mapData, jsonLoader.GetMinX(), jsonLoader.GetMinY());

        MoveToCurrentCell();
    }

    private void Update()
    {
        if (Time.time - lastMoveTime < moveDelay) return;

        if (manualInputEnabled)
        {
            HandleInput();
        }
        else if (isFollowingPath && !pathCompleted)
        {
            FollowPath();
        }

    }


    public void SetPath(List<Vector2Int> newPath)
    {
        if (newPath == null || newPath.Count == 0)
        {
            Debug.LogWarning("Invalid path provided!");
            return;
        }

        currentPath = new List<Vector2Int>(newPath);
        pathIndex = 0;
        isFollowingPath = true;
        pathCompleted = false;

        int totalMoves = currentPath.Count - 1; // Actual moves (excluding start position)
        Debug.Log($"Character received path with {currentPath.Count} positions, requiring {totalMoves} moves");

        // Start from the first position in the path
        if (currentPath.Count > 0)
        {
            SetGridPosition(currentPath[0].x, currentPath[0].y);
            pathIndex = 1; // Next move will be to index 1
        }
    }

    public event System.Action OnPathCompleted;

    private void FollowPath()
    {
        if (pathIndex >= currentPath.Count)
        {
            isFollowingPath = false;
            pathCompleted = true;
            int totalMoves = currentPath.Count - 1;
            Debug.Log($"Path completed! Made {totalMoves} moves through {currentPath.Count} positions.");
            OnPathCompleted?.Invoke();
            return;
        }

        Vector2Int nextPosition = currentPath[pathIndex];

        if (TryMove(nextPosition.x, nextPosition.y))
        {
            int currentMove = pathIndex; // Current move number (1-based)
            int totalMoves = currentPath.Count - 1; // Total moves needed
            pathIndex++;
            Debug.Log($"Move {currentMove}/{totalMoves} completed - Reached ({nextPosition.x}, {nextPosition.y})");
        }
        else
        {
            Debug.LogError($"Path blocked at ({nextPosition.x}, {nextPosition.y})!");
            isFollowingPath = false;
        }
    }

    private void HandleInput()
    {
        int newGridX = currentGridX;
        int newGridZ = currentGridZ;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            newGridZ++;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            newGridZ--;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            newGridX--;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            newGridX++;
        else
            return;

        TryMove(newGridX, newGridZ);
    }


    public bool TryMove(int targetX, int targetZ)
    {
        if (Time.time - lastMoveTime < moveDelay)
            return false;

        if (!grid.IsValidGridPosition(targetX, targetZ))
            return false;

        // Check for wall collision
        if (IsMovementBlocked(currentGridX, currentGridZ, targetX, targetZ))
            return false;

        FaceDirection(new Vector2Int(currentGridX, currentGridZ), new Vector2Int(targetX, targetZ));

        currentGridX = targetX;
        currentGridZ = targetZ;

        MoveToCurrentCell();
        lastMoveTime = Time.time;

        return true;
    }

    private void MoveToCurrentCell()
    {
        if (spawnedCharacter == null) return;

        Vector3 cellCenter = grid.GetCellCenter(currentGridX, currentGridZ);
        spawnedCharacter.transform.position = cellCenter;
    }

    private bool IsMovementBlocked(int fromX, int fromZ, int toX, int toZ)
    {
        if (mapAdapter == null) return false;

        return PathfindingAlgorithm.IsMovementBlocked(
            new Vector2Int(fromX, fromZ),
            new Vector2Int(toX, toZ),
            mapAdapter
        );
    }

    public Vector2Int CurrentGridPosition => new Vector2Int(currentGridX, currentGridZ);

    public void SetGridPosition(int gridX, int gridZ)
    {
        if (grid.IsValidGridPosition(gridX, gridZ))
        {
            currentGridX = gridX;
            currentGridZ = gridZ;
            MoveToCurrentCell();
        }
    }

    public Grid GetGrid()
    {
        return grid;
    }

    // Manual control toggle (for testing)
    [ContextMenu("Toggle Manual Input")]
    public void ToggleManualControl()
    {
        manualInputEnabled = !manualInputEnabled;
        if (manualInputEnabled)
        {
            isFollowingPath = false;
            Debug.Log("Manual control enabled (WASD or Arrow Keys)");
        }
        else
        {
            Debug.Log("Manual control disabled");
        }
    }

    public void InitializeCharacterAtStart()
    {
        Vector2Int startGridPos = jsonLoader.GetStartGridPosition();
        currentGridX = startGridPos.x;
        currentGridZ = startGridPos.y;

        Vector3 worldPos = grid.GetCellCenter(currentGridX, currentGridZ);
        spawnedCharacter = Instantiate(charPrefab, worldPos, Quaternion.identity);
        spawnedCharacter.transform.SetParent(transform);
    }

    // Public getter for UI to show progress
    public int GetCurrentMoveNumber()
    {
        if (!isFollowingPath || currentPath == null || currentPath.Count == 0)
            return 0;

        return pathIndex; // Current move number (1-based, 0 means at start)
    }

    public int GetTotalMoves()
    {
        if (currentPath == null || currentPath.Count == 0)
            return 0;

        return currentPath.Count - 1; // Total moves excluding start position
    }

    public bool IsFollowingPath()
    {
        return isFollowingPath;
    }

    public bool IsPathCompleted()
    {
        return pathCompleted;
    }

    private void FaceDirection(Vector2Int from, Vector2Int to)
    {
        Vector3 fromPos = grid.GetCellCenter(from.x, from.y);
        Vector3 toPos = grid.GetCellCenter(to.x, to.y);

        Vector3 direction = (toPos - fromPos).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            spawnedCharacter.transform.rotation = targetRotation;
        }
    }
}