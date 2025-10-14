using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MapConnection
{
    public Vector2Int a;
    public Vector2Int b;

    public float g_cost;

    public MapConnection(Vector2Int a, Vector2Int b, float g_cost)
    {
        this.a = a;
        this.b = b;
        this.g_cost = g_cost;
    }



}
