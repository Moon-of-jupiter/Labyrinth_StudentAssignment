using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
public class MapGraphManager
{
    public Dictionary<Vector2Int, List<MapConnection>> map_node_connections_by_node { get; protected set; } = new();

    public HashSet<Vector2Int> map_nodes { get; protected set; } = new();

    public IMapData mapData;

    public MapGraphManager(IMapData mapData)
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

        foreach (var thisNode in map_nodes)
        {

            if (!Has_H_Wall(thisNode, out float cost_h))
            {
                if (map_nodes.TryGetValue(thisNode + new Vector2Int(0, -1), out var otherNode))
                {
                    AddConnection(thisNode, otherNode, cost_h);
                    AddConnection(otherNode, thisNode, cost_h);
                }
            }

            if (!Has_V_Wall(thisNode, out float cost_v))
            {
                if (map_nodes.TryGetValue(thisNode + new Vector2Int(-1, 0), out var otherNode))
                {
                    AddConnection(thisNode, otherNode, cost_v);
                    AddConnection(otherNode, thisNode, cost_v);
                }
            }

            
        }

        foreach (var vent in mapData.GetAllVentPositions())
        {
            if (!map_nodes.Contains(vent)) continue;

            foreach (var otherVent in mapData.GetOtherVentPositions(vent))
            {
                if (!map_nodes.Contains(otherVent)) continue;

                AddConnection(otherVent, vent, mapData.GetVentCost(vent.x, vent.y));
            }
        }
    }

    private bool Has_H_Wall(Vector2Int pos, out float cost)
    {
        cost = mapData.GetHorizontalWallCost(pos.x, pos.y);

        return cost > float.MaxValue / 2f;
    }

    private bool Has_V_Wall(Vector2Int pos, out float cost)
    {
        cost = mapData.GetVerticalWallCost(pos.x, pos.y);

        return cost > float.MaxValue / 2f;
    }

    

    private void AddMapNode(Vector2Int newNode)
    {
        map_nodes.Add(newNode);
    }


    private void AddConnection(Vector2Int a, Vector2Int b, float g_cost)
    {
        List<MapConnection> connections;

        if (!map_node_connections_by_node.TryGetValue(a, out connections))
        {
            connections = new();

            map_node_connections_by_node.Add(a, connections);


        }

        connections.Add(new MapConnection(a, b, g_cost));
    }


    public float GetDistance(Vector2Int a, Vector2Int b)
    {
        return (int)Vector2.Distance(a, b);
    }
}
