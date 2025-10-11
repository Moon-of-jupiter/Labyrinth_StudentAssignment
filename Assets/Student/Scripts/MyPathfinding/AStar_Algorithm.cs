using System.Collections.Generic;
using System.Linq;
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

        BakeHCost();

        var f_cost_comp = new SimpleLamdaComparer<NavNode>((NavNode a, NavNode b) => 
        {
            return -a.f_cost.CompareTo(b.f_cost); 
        });

        open = new BinaryHeap<NavNode>(f_cost_comp, 4);


        this.start = start;
        this.end = end;

        startNode = CreateNode(start);
        OpenNode(startNode);

        
    }

    private void BakeHCost()
    {
        foreach(var pos in map_graph_data.map_nodes)
        {
            float h_cost = map_graph_data.GetDistance(pos, end);

            if(!map_node_h_cost.TryAdd(pos, h_cost))
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
        path.Add(current.position);

        while(current.position != startNode.position)
        {
            for(var i = current.parents.Count -1; i >= 0 ; i--)
            {
                path.Add((current.parents[i]));
            }

            current = nodes_byPos[current.parents[0]];
        }

        path.Reverse();
    }

    public bool Loop_PathFindOneStep()
    {
        int c = 0;
        while (PathFindOneStep())
        {
            c++;
            if(c > 1000)
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
        foreach (var conn in map_graph_data.map_node_connections_by_node[pos])
        {
            OpenNode(AppendConnection(CreateNode(pos), conn));
        }
    }

    private NavNode AppendConnection(NavNode node, MapConnection conn)
    {
        node.AppendConnnection(conn);
        return node;
    }

    
    
    private NavNode CreateNode(Vector2Int pos, float one_g_cost = 0)
    {
        return new NavNode()
        {
            position = pos,
            parents = new List<Vector2Int>(),
            g_cost = one_g_cost,
            h_cost = map_node_h_cost[pos]
        };
    }

    
    private void OpenNode(NavNode node)
    {
        if (closed.Contains(node.position)) return;

        if (nodes_byPos.ContainsKey(node.position))
        {
            if (node.g_cost < nodes_byPos[node.position].g_cost)
            {
                ReplaceOpenNode(node);
            }

            return;
        }

        AddOpenNode(node);
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

        //nodes_byPos.Remove(current.position);
        closed.Add(current.position);

        return current;
    }

}
