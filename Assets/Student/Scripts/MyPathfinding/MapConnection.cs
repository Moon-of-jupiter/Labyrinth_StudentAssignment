using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MapConnection
{
    float cost;
    public List<MapNode> positions = new();
    public MapConnection(MapNode a, MapNode b, float cost)
    {
        positions.Add(a); positions.Add(b);
        this.cost = cost;
    }

    public MapConnection(List<MapConnection> connectionChain)
    {
        for(int i = 0; i < connectionChain.Count; i++)
        {
            cost += connectionChain[i].cost;

            positions.AddRange(connectionChain[i].positions);
        }
    }

    

    
}
