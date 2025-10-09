using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class MapGraphManager
{
    private Dictionary<Vector2Int, List<MapConnection>> map_node_connections_by_node = new();

    private HashSet<Vector2Int> map_nodes = new();

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

            if (!mapData.HasHorizontalWall(thisNode.x, thisNode.y))
            {
                if (map_nodes.TryGetValue(thisNode + new Vector2Int(0, -1), out var otherNode))
                {
                    float cost = mapData.GetHorizontalWallCost(thisNode.x, thisNode.y);

                    AddConnection(thisNode, otherNode, cost);
                    AddConnection(otherNode, thisNode, cost);
                }
            }

            if (!mapData.HasVerticalWall(thisNode.x, thisNode.y))
            {
                if (map_nodes.TryGetValue(thisNode + new Vector2Int(-1, 0), out var otherNode))
                {
                    float cost = mapData.GetVerticalWallCost(thisNode.x, thisNode.y);

                    AddConnection(thisNode, otherNode, cost);
                    AddConnection(otherNode, thisNode, cost);
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
        List<MapConnection> connections;

        if (!map_node_connections_by_node.TryGetValue(a, out connections))
        {
            connections = new();

            map_node_connections_by_node.Add(a, connections);


        }

        connections.Add(new MapConnection(a, b, g_cost));
    }
}
