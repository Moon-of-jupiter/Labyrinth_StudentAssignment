using System.Collections.Generic;
using UnityEngine;

public class NavNode
{
    public float h_cost;
    public float g_cost;
    public float f_cost => h_cost + g_cost;

    public List<Vector2Int> parents;

    public Vector2Int position;

    public NavNode()
    {

    }

    public void AppendConnnection(MapConnection mapConnection)
    {
        if (!(mapConnection.a == position || mapConnection.b == position)) return;
        
        
        parents.Add(position);
        
        if (mapConnection.a == position)
        {
            position = mapConnection.b;
        }
        else if(mapConnection.b == position)
        {
            position = mapConnection.a;
        }

        g_cost += mapConnection.g_cost;
    }
}
