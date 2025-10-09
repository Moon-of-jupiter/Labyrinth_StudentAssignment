using System.Collections.Generic;
using UnityEngine;

public class AStar_Algorithm
{
    public bool pathFound { get; private set; }

    private Dictionary<Vector2Int, float> map_node_h_cost;
    


    private MapGraphManager map_graph_data;
    private Vector2Int start;
    private Vector2Int end;

    private Dictionary<Vector2Int, NavNode> nodes;

    private BinaryHeap<NavNode> open;
    private HashSet<Vector2Int> closed;

    private NavNode startNode;
    private NavNode endNode;
    

    

    public AStar_Algorithm(Vector2Int start, Vector2Int end, MapGraphManager map_graph_data)
    {
        this.map_graph_data = map_graph_data;

        var f_cost_comp = new SimpleLamdaComparer<NavNode>((NavNode a, NavNode b) => 
        {
            return a.f_cost.CompareTo(b.f_cost); 
        });

        open = new BinaryHeap<NavNode>(f_cost_comp, 4);


        this.start = start;
        this.end = end;

        startNode = CreateNode(start, 0, new List<Vector2Int>());
        OpenNode(startNode);

        BakeHCost();
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

    public void PathFindOneStep()
    {
        if (pathFound || open.IsEmpty()) return;

        NavNode current = open.PopFirst();

        CloseNode(current);

        if(current.position == end)
        {
            pathFound = true;
        }


    }

    private void CreateNeighbours(Vector2Int pos)
    {

    }

    private NavNode CreateNode(Vector2Int pos, float one_g_cost, List<Vector2Int> parents)
    {
        throw new System.NotImplementedException();
    }

    private void OpenNode(NavNode node)
    {
        open.Push(node);
    }

    private void CloseNode(NavNode node)
    {
        closed.Add(node.position);
    }

}
