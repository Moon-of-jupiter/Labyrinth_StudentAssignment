using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class PathfindingAlgorithm
{
    //private static PathfindContext LastPathfind;

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

    private static Graph_Data graph_Data;


    #region A_Star_Data

    #endregion

    // I added Grid visualizationGrid in order to debug and visualize my algorithm
    public static List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, IMapData mapData)
    {
        // TODO: Implement your pathfinding algorithm here

        //LastPathfind = new PathfindContext()
        //{
        //    mapGraph = new MapGraphManager(mapData)
        //};


        graph_Data = new Graph_Data(mapData);


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
        //LastPathfind.Visualize(grid, offset);
    }

    public static void OnApplicationEnd()
    {
        
    }



}

#region Map_Graph_Data



public class Graph_Data
{
    private Dictionary<Vector2Int, List<Graph_Connection>> map_node_connections_by_node = new();

    private HashSet<Vector2Int> map_nodes = new();

    public IMapData mapData;

    public Graph_Data(IMapData mapData)
    {
        this.mapData = mapData;
        BuildGraph(mapData);
    }

    public void BuildGraph(IMapData mapData)
    {
        for (int x = 0; x < mapData.Width; x++)
        {
            for (int y = 0; y < mapData.Height; y++)
            {
                AddMapNode(new Vector2Int(x, y));
            }
        }

        foreach(var thisNode in map_nodes)
        {
            
            if (!mapData.HasHorizontalWall(thisNode.x, thisNode.y))
            {
                if (map_nodes.TryGetValue(thisNode + new Vector2Int(0, -1), out var otherNode))
                {
                    float cost = mapData.GetHorizontalWallCost(thisNode.x, thisNode.y);

                    AddConnection(thisNode, otherNode,  cost);
                    AddConnection(otherNode,thisNode,   cost);
                }
            }

            if (!mapData.HasVerticalWall(thisNode.x, thisNode.y))
            {
                if (map_nodes.TryGetValue(thisNode + new Vector2Int(-1, 0), out var otherNode))
                {
                    float cost = mapData.GetVerticalWallCost(thisNode.x, thisNode.y);

                    AddConnection(thisNode, otherNode,  cost);
                    AddConnection(otherNode,thisNode,   cost);
                }
            }

            // add vent stuff here
        }
    }

    private void AddMapNode(Vector2Int newNode)
    {
        map_nodes.Add(newNode);
    }


    private void AddConnection(Vector2Int a, Vector2Int b, float g_cost)
    {
        List<Graph_Connection> connections;

        if(!map_node_connections_by_node.TryGetValue(a, out connections))
        {
            connections = new();

            map_node_connections_by_node.Add(a, connections);

            
        }

        connections.Add(new Graph_Connection(a,b, g_cost));
    }

}

public class Graph_Connection
{
    public Vector2Int a;
    public Vector2Int b;

    public float g_cost;

    public Graph_Connection(Vector2Int a, Vector2Int b, float g_cost)
    {
        this.a = a;
        this.b = b;
        this.g_cost = g_cost;
    }
}

#endregion


#region PathFinding_Data
public class A_StarPathfinding
{
    private Dictionary<Vector2Int, float> map_node_h_cost;

    public bool pathFound { get; private set; }

    private Graph_Data map_graph_data;

    public A_StarPathfinding(Graph_Data map_graph_data)
    {
        this.map_graph_data = map_graph_data;
    }

    public void PathFindOneSetp()
    {
        if (pathFound) return;

        

    }

}

public class Path_Node
{
    public float h_cost;
    public float g_cost;
    public float f_cost => h_cost + g_cost;

    public List<Vector2Int> parents;
}

#endregion

//public struct PathfindContext
//{
//    public MapGraphManager mapGraph;

//    public void Visualize(Grid grid, Vector3 offset)
//    {
//        mapGraph?.VisualizeGraph(grid, offset);
//    }
//}