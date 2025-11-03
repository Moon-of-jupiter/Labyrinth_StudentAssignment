using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStar_Algorithm
{
    public bool pathFound { get; private set; }

    private Dictionary<Vector2Int, float> map_node_h_cost = new();



    private MapGraphManager map_graph_data;
    private Vector2Int start;
    private Vector2Int end;

    //private Dictionary<Vector2Int, NavNode> nodes;

    private BinaryHeap<NavNode> open;
    private Dictionary<Vector2Int, NavNode> nodes_byPos = new();
    private HashSet<Vector2Int> closed = new();

    private NavNode startNode;
    private NavNode endNode;




    public AStar_Algorithm(Vector2Int start, Vector2Int end, MapGraphManager map_graph_data)
    {

        this.map_graph_data = map_graph_data;
        this.start = start;
        this.end = end;

        BakeHCost();

        var f_cost_comp = new SimpleLamdaComparer<NavNode>((NavNode a, NavNode b) =>
        {
            return -a.f_cost.CompareTo(b.f_cost); ;
        });

        open = new BinaryHeap<NavNode>(f_cost_comp, 4);


        

       //&&startNode = new NavNode();
        OpenNode(start, start, map_node_h_cost[start], 0);


    }

    private void BakeHCost()
    {
        foreach (var pos in map_graph_data.map_nodes)
        {
            float h_cost = map_graph_data.GetDistance(pos, end);

            if (!map_node_h_cost.TryAdd(pos, h_cost))
            {
                map_node_h_cost[pos] = h_cost;
            }
        }
    }

    public bool FindPath(out List<Vector2Int> path)
    {
        path = new();

        if (!Loop_PathFindOneStep()) return false;

        FollowPathBack(endNode, path);

        return true;
    }

    private void FollowPathBack(NavNode node, List<Vector2Int> path)
    {
        var current = node;
        //path.Add(current.position);
        while(current.position != start)
        {
            path.Add(current.position);
            current = nodes_byPos[current.parent];
        }

        path.Add(current.position);

        path.Reverse();
    }

    public bool Loop_PathFindOneStep()
    {
        int c = 0;
        while (PathFindOneStep())
        {
            c++;
            if (c > 100000)
            {
                Debug.LogError("a* failed, too many iterations");

                return false;
            }
        }

        return pathFound;
    }

    public bool PathFindOneStep()
    {
        if (pathFound || open.IsEmpty()) return false;

        NavNode current = CloseFirstNode();

        if (current.position == end)
        {
            pathFound = true;
            endNode = current;
            return false;
        }

        CreateNeighbours(current.position);

        return true;
    }

    private void CreateNeighbours(Vector2Int pos)
    {
        
        var connections = map_graph_data.map_node_connections_by_node[pos];
        for (int i = 0; i < connections.Count(); i++)
        {
            if (closed.Contains(connections[i].b)) continue;
            OpenNode(connections[i].b, connections[i].a, map_node_h_cost[connections[i].b], connections[i].g_cost + nodes_byPos[connections[i].a].g_cost);
        }
    }


      // legacy
    //private void OpenNode(NavNode node)
    //{
    //    if (closed.Contains(node.position)) return;

    //    if (nodes_byPos.ContainsKey(node.position))
    //    {
    //        if (node.g_cost < nodes_byPos[node.position].g_cost)
    //        {
    //            ReplaceOpenNode(node);
    //        }

    //        return;
    //    }

    //    AddOpenNode(node);
    //}

    private NavNode OpenNode(Vector2Int position, Vector2Int parent, float h_cost, float g_cost)
    {
        if (closed.Contains(position)) return null;
        NavNode node = null;


        if (nodes_byPos.ContainsKey(position))
        {
            if (!(g_cost < nodes_byPos[position].g_cost)) return null;
            ReplaceOpenNode(node = new NavNode(position, parent, h_cost, g_cost));
        }
        else
        {
            AddOpenNode(node = new NavNode(position, parent, h_cost, g_cost));
        }

       

        return node;
    }

    private void AddOpenNode(NavNode node)
    {
        open.Push(node);
        nodes_byPos.Add(node.position, node);
    }

    private void ReplaceOpenNode(NavNode node)
    {
        open.ReplaceItem(nodes_byPos[node.position], node);
        nodes_byPos[node.position] = node;
    }

    private NavNode CloseFirstNode()
    {
        var current = open.PopFirst();

        closed.Add(current.position);

        return current;
    }

}


