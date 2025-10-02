using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MapConnection
{
    float cost;

    public MapConnection(MapNode a, MapNode b, float cost)
    {

    }

    public MapConnection(List<MapConnection> connectionChain)
    {
        for(int i = 0; i < connectionChain.Count; i++)
        {
            cost += connectionChain[i].cost;

            positions.AddRange(connectionChain[i].positions);
        }
    }

    public LinkedList<MapNode> positions;


}
