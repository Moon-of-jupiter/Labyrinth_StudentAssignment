using System.Collections.Generic;
using UnityEngine;

public class AStar_Algorithm
{


    private Dictionary<Vector2Int, float> map_node_h_cost;

    public bool pathFound { get; private set; }

    private MapGraphManager map_graph_data;

    public AStar_Algorithm(MapGraphManager map_graph_data)
    {
        this.map_graph_data = map_graph_data;
        
        
    }

    public void PathFindOneSetp()
    {
        if (pathFound) return;



    }

}
