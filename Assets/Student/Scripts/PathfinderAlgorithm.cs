using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class PathfindingAlgorithm
{
    private static PathfindContext LastPathfind;

    /* <summary>
     TODO: Implement pathfinding algorithm here
     Find the shortest path from start to goal position in the maze.
     
     Dijkstra's Algorithm Steps:
     1. Initialize distances to all nodes as infinity
     2. Set distance to start node as 0
     3. Add start node to priority queue
     4. While priority queue is not empty:
        a. Remove node with minimum distance
        b. If it's the goal, reconstruct path
        c. For each neighbor:
           - Calculate new distance through current node
           - If shorter, update distance and add to queue
     
     MAZE FEATURES TO HANDLE:
     - Basic movement cost: 1.0 between adjacent cells
     - Walls: Some have infinite cost (impassable), others have climbing cost
     - Vents (teleportation): Allow instant travel between distant cells with usage cost
     
     AVAILABLE DATA STRUCTURES:
     - Dictionary<Vector2Int, float> - for tracking distances
     - Dictionary<Vector2Int, Vector2Int> - for tracking previous nodes (path reconstruction)
     - SortedSet<T> or List<T> - for priority queue implementation
     - mapData provides methods to check walls, vents, and boundaries
     
     HINT: Start simple with BFS (ignore wall costs and vents), then extend to weighted Dijkstra
     </summary> */


    // I added Grid visualizationGrid in order to debug and visualize my algorithm
    public static List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, IMapData mapData)
    {
        // TODO: Implement your pathfinding algorithm here

        LastPathfind = new PathfindContext()
        {
            mapGraph = new MapGraphManager(mapData)
        };

        
        
        

        //Debug.LogWarning("FindShortestPath not implemented yet!");
        return null;
    }

    public static bool IsMovementBlocked(Vector2Int from, Vector2Int to, IMapData mapData)
    {
        // TODO: Implement movement blocking logic
        // For now, allow all movement so character can move while you work on pathfinding
        return false;
    }


   

    public static void Visualize(Grid grid, Vector3 offset)
    {
        LastPathfind.Visualize(grid, offset);
    }

    public static void OnApplicationEnd()
    {
        LastPathfind = default;
    }



}


public struct PathfindContext
{
    public MapGraphManager mapGraph;

    public void Visualize(Grid grid, Vector3 offset)
    {
        mapGraph?.VisualizeGraph(grid, offset);
    }
}