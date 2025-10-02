using UnityEngine;
using System.Collections.Generic;
public class MapNode
{
    public List<MapConnection> connections = new();

    public Vector2Int position;

    public bool saveWhenOptimizing => isIntersection || isImportant;

    public bool isIntersection => connections.Count != 2;
    public bool isImportant;

    public MapNode(Vector2Int pos, bool isImportant = false)
    {
        position = pos;

        this.isImportant = isImportant;
    }

    public void AddConnection(MapConnection connection)
    {
        connections.Add(connection);
    }




}
