using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class MapGraphManager
{
    public List<MapNode> mapNodes = new();
    public List<MapConnection> mapConnections = new();

    public Dictionary<Vector2Int, MapNode> nodes_by_pos = new();

    private Vector3 visualOffset = new Vector3(0.5f, 0.5f, 0.5f);

    // raw constructor for building every map node
    public MapGraphManager(IMapData mapData)
    {
        for(int x = 0; x < mapData.Width; x++)
        {
            for (int y = 0; y < mapData.Height; y++)
            {
                AddMapNode(new MapNode(new Vector2Int(x, y)));
            }
        }

        for(int i = 0; i < mapNodes.Count(); i++)
        {
            var thisNode = mapNodes[i];
            var pos = thisNode.position;

            if (!mapData.HasHorizontalWall(pos.x, pos.y))
            {
                if(nodes_by_pos.TryGetValue(pos + new Vector2Int(0,-1), out var otherNode))
                {
                    AddConnection(
                        new MapConnection
                        (
                            thisNode, 
                            otherNode, 
                            mapData.GetHorizontalWallCost(pos.x,pos.y)
                        ));
                }
            }

            if (!mapData.HasVerticalWall(pos.x, pos.y))
            {
                if (nodes_by_pos.TryGetValue(pos + new Vector2Int(-1, 0), out var otherNode))
                {
                    AddConnection(
                        new MapConnection
                        (
                            thisNode,
                            otherNode,
                            mapData.GetVerticalWallCost(pos.x, pos.y)
                        ));
                }
            }

            // add vent stuff here
        }
    }


    public void AddMapNode(MapNode node)
    {
        mapNodes.Add(node);
        nodes_by_pos.Add(node.position, node);
    }

    public void AddConnection(MapConnection connection, bool addToNodes = true)
    {
        mapConnections.Add(connection);

        if(addToNodes)
        {
            connection.positions.First().AddConnection(connection);
            connection.positions.Last().AddConnection(connection);
        }
    }

    
    public void VisualizeGraph(Grid toWorldConverterGrid, Vector3 offset)
    {
        visualOffset = offset;

        for(int i = 0; i < mapNodes.Count; i++)
        {
            

            DrawDebugPoint(mapNodes[i].position, toWorldConverterGrid);

        }

        for(int i = 0; i < mapConnections.Count; i++)
        {
            var points = mapConnections[i].positions;

            Vector2Int lastPos = points[0].position;

            for (int j = 1; j < points.Count - 1; j++)
            {
                DrawDebugLine(lastPos, points[j].position, toWorldConverterGrid);

                lastPos = points[j].position;
            }

            DrawDebugLine(lastPos, points.Last().position, toWorldConverterGrid);


        }
    }

    protected void DrawDebugPoint(Vector2Int position, Grid grid)
    {
        var pos = grid.GridToWorldPosition(position.x,position.y) + visualOffset;

        Gizmos.DrawSphere(pos, 0.05f);
    }

    protected void DrawDebugLine(Vector2Int a, Vector2Int b, Grid grid)
    {
        Debug.DrawLine(grid.GridToWorldPosition(a.x, a.y) + visualOffset, grid.GridToWorldPosition(b.x, b.y) + visualOffset, Color.yellow);
    }
}
