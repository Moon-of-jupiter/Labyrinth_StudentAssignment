using System.Collections.Generic;
using UnityEngine;

public struct NavNode
{
    public float h_cost;
    public float g_cost;
    public float f_cost => h_cost + g_cost;

    public Vector2Int parent;

    public Vector2Int position;

    public NavNode(Vector2Int position, Vector2Int parent, float h_cost, float g_cost)
    {
        this.position = position;
        this.parent = parent;

        this.h_cost = h_cost;
        this.g_cost = g_cost;
    }
    
    //public void AppendConnnection(MapConnection mapConnection)
    //{
    //    if (!(mapConnection.a == position || mapConnection.b == position)) return;
        
        
    //    parents.Add(position);
        
    //    if (mapConnection.a == position)
    //    {
    //        position = mapConnection.b;
    //    }
    //    else if(mapConnection.b == position)
    //    {
    //        position = mapConnection.a;
    //    }

    //    g_cost += mapConnection.g_cost;
    //}
}
