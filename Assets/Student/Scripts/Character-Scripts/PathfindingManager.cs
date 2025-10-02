using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderManager : MonoBehaviour
{
    [Header("References")]
    public JsonLoader jsonLoader;
    public GridCharacterMovement characterMovement;
    public PathfindingUI pathfindingUI; // New UI reference

    [Header("Settings")]
    public bool autoStart = true;
    public float startDelay = 2f;

    [Header("Debug Visualization")]
    public bool showDebugPath = true;
    public Color pathColor = Color.yellow;
    public Color jumpPathColor = Color.red;
    public Color ventPathColor = Color.cyan; // New color for vent teleportation
    public float pathNodeSize = 0.3f;

    [Header("Custom Debug Visualization")]
    public Vector3 visual_offset;

    private List<GameObject> debugPathObjects = new List<GameObject>();
    private IMapData mapDataInterface;

    private void Start()
    {
        // Find UI component if not assigned
        if (pathfindingUI == null)
        {
            pathfindingUI = FindAnyObjectByType<PathfindingUI>();
        }

        if (autoStart)
        {
            Invoke(nameof(StartPathfinding), startDelay);
        }
    }

    public void StartPathfinding()
    {
        // Get map data from JsonLoader
        MapData mapData = jsonLoader.GetMapData();
        if (mapData == null)
        {
            Debug.LogError("No map data available!");
            UpdateUIForError();
            return;
        }

        // Create adapter with proper offsets
        mapDataInterface = new MapDataAdapter(mapData, jsonLoader.GetMinX(), jsonLoader.GetMinY());

        // Get current quest positions
        int questIndex = jsonLoader.GetQuestIndex();
        Vector2Int startPos = mapDataInterface.GetQuestStart(questIndex);
        Vector2Int goalPos = mapDataInterface.GetQuestGoal(questIndex);

        Debug.Log($"Starting pathfinding from ({startPos.x}, {startPos.y}) to ({goalPos.x}, {goalPos.y})");

        // Call the student's pathfinding algorithm
        List<Vector2Int> path = PathfindingAlgorithm.FindShortestPath(startPos, goalPos, mapDataInterface);

        if (path != null && path.Count > 0)
        {
            float totalCost = CalculatePathCost(path);
            int totalMoves = path.Count - 1; // Moves = positions - 1
            int totalPositions = path.Count;

            Debug.Log($"Path found with {totalPositions} positions requiring {totalMoves} moves! Total cost: {totalCost:F1}");

            // Update UI with path information
            UpdateUIForSuccessfulPath(totalCost, totalMoves, totalPositions);

            // Visualize the path
            if (showDebugPath)
                VisualizePath(path);

            // Set character to follow the path
            characterMovement.SetPath(path);
        }
        else
        {
            Debug.LogError("No path found! Make sure the FindShortestPath method is implemented correctly.");
            UpdateUIForNoPath();
        }
    }

    private void UpdateUIForSuccessfulPath(float cost, int moves, int positions)
    {
        if (pathfindingUI != null)
        {
            pathfindingUI.UpdatePathInfo(cost, moves, positions);
        }
    }

    private void UpdateUIForNoPath()
    {
        if (pathfindingUI != null)
        {
            pathfindingUI.UpdatePathNotFound();
        }
    }

    private void UpdateUIForError()
    {
        if (pathfindingUI != null)
        {
            pathfindingUI.UpdatePathNotFound();
        }
    }

    private void OnDrawGizmos()
    {
        if (characterMovement?.GetGrid() != null && isActiveAndEnabled)
        {
            PathfindingAlgorithm.Visualize(characterMovement.GetGrid(), visual_offset);
        }
    }

    private float CalculatePathCost(List<Vector2Int> path)
    {
        if (path == null || path.Count < 2)
            return 0f;

        float totalCost = 0f;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2Int from = path[i];
            Vector2Int to = path[i + 1];

            float stepCost = GetMovementCost(from, to);
            totalCost += stepCost;
        }

        return totalCost;
    }

    private float GetMovementCost(Vector2Int from, Vector2Int to)
    {
        // Check if this is vent teleportation
        if (mapDataInterface.HasVent(from.x, from.y) && mapDataInterface.HasVent(to.x, to.y))
        {
            int ventDeltaX = Mathf.Abs(to.x - from.x); // Renamed to ventDeltaX
            int ventDeltaY = Mathf.Abs(to.y - from.y);

            // If it's teleportation (not adjacent), return vent cost
            if (ventDeltaX > 1 || ventDeltaY > 1 || (ventDeltaX == 1 && ventDeltaY == 1))
            {
                return mapDataInterface.GetVentCost(from.x, from.y);
            }
        }

        // Regular movement cost calculation
        float baseCost = 1.0f;

        int moveDeltaX = to.x - from.x; // Renamed to moveDeltaX
        int moveDeltaY = to.y - from.y;

        // Check horizontal movement (blocked by vertical walls)
        if (moveDeltaX != 0)
        {
            int wallX = moveDeltaX > 0 ? to.x : from.x;
            int wallY = from.y;

            float wallCost = mapDataInterface.GetVerticalWallCost(wallX, wallY);
            if (wallCost > 1.0f)
                baseCost += wallCost - 1.0f; // Add extra cost for jumping
        }

        // Check vertical movement (blocked by horizontal walls)
        if (moveDeltaY != 0)
        {
            int wallX = from.x;
            int wallY = moveDeltaY > 0 ? to.y : from.y;

            float wallCost = mapDataInterface.GetHorizontalWallCost(wallX, wallY);
            if (wallCost > 1.0f)
                baseCost += wallCost - 1.0f; // Add extra cost for jumping
        }

        return baseCost;
    }

    private void VisualizePath(List<Vector2Int> path)
    {
        ClearDebugPath();

        for (int i = 0; i < path.Count; i++)
        {
            var position = path[i];

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = characterMovement.GetGrid().GetCellCenter(position.x, position.y) + Vector3.up * 0.1f;
            sphere.transform.localScale = Vector3.one * pathNodeSize;

            Renderer renderer = sphere.GetComponent<Renderer>();

            // Determine the type of movement to this node
            Color nodeColor = pathColor; // Default
            if (i > 0)
            {
                Vector2Int prevPos = path[i - 1];
                float stepCost = GetMovementCost(prevPos, position);

                // Check if this is vent teleportation
                if (mapDataInterface.HasVent(prevPos.x, prevPos.y) && mapDataInterface.HasVent(position.x, position.y))
                {
                    int deltaX = Mathf.Abs(position.x - prevPos.x);
                    int deltaY = Mathf.Abs(position.y - prevPos.y);

                    if (deltaX > 1 || deltaY > 1 || (deltaX == 1 && deltaY == 1))
                    {
                        nodeColor = ventPathColor; // Cyan for vent teleportation
                    }
                    else if (stepCost > 1.0f)
                    {
                        nodeColor = jumpPathColor; // Red for wall jumping
                    }
                }
                else if (stepCost > 1.0f)
                {
                    nodeColor = jumpPathColor; // Red for wall jumping
                }
            }

            renderer.material.color = nodeColor;

            // Remove collider to avoid interference
            Destroy(sphere.GetComponent<Collider>());

            debugPathObjects.Add(sphere);
        }

        // Draw lines to show vent teleportation connections
        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int prevPos = path[i - 1];
            Vector2Int currentPos = path[i];

            // Check if this is vent teleportation
            if (mapDataInterface.HasVent(prevPos.x, prevPos.y) && mapDataInterface.HasVent(currentPos.x, currentPos.y))
            {
                int deltaX = Mathf.Abs(currentPos.x - prevPos.x);
                int deltaY = Mathf.Abs(currentPos.y - prevPos.y);

                if (deltaX > 1 || deltaY > 1 || (deltaX == 1 && deltaY == 1))
                {
                    // Create a line renderer to show teleportation
                    GameObject lineObj = new GameObject("VentTeleportLine");
                    LineRenderer line = lineObj.AddComponent<LineRenderer>();
                    line.material = new Material(Shader.Find("Sprites/Default"));
                    line.startColor = ventPathColor;
                    line.endColor = ventPathColor;
                    line.startWidth = 0.1f;
                    line.endWidth = 0.1f;
                    line.positionCount = 2;

                    Vector3 startPos = characterMovement.GetGrid().GetCellCenter(prevPos.x, prevPos.y) + Vector3.up * 0.2f;
                    Vector3 endPos = characterMovement.GetGrid().GetCellCenter(currentPos.x, currentPos.y) + Vector3.up * 0.2f;

                    line.SetPosition(0, startPos);
                    line.SetPosition(1, endPos);

                    debugPathObjects.Add(lineObj);
                }
            }
        }
    }

    private void ClearDebugPath()
    {
        foreach (var obj in debugPathObjects)
        {
            if (obj != null) Destroy(obj);
        }
        debugPathObjects.Clear();
    }

    [ContextMenu("Find Path")]
    public void FindPathManual()
    {
        StartPathfinding();
    }

    [ContextMenu("Clear Path Visualization")]
    public void ClearPathManual()
    {
        ClearDebugPath();
    }

    private void OnDestroy()
    {
        ClearDebugPath();
        PathfindingAlgorithm.OnApplicationEnd();
    }
}