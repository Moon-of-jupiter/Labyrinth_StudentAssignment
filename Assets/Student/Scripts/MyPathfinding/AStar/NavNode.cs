using UnityEngine;

public class NavNode
{
    public float h_cost;
    public float g_cost;
    public float f_cost => h_cost + g_cost;

    public NavNode parent;

    public MapNode coresponding_MapNode;


    public NavNode(NavNode parent, float added_g_cost, float h_cost)
    {
        this.g_cost = added_g_cost;
        this.h_cost = h_cost;

        this.parent = parent;

        if(parent != null)
        {
            g_cost += parent.g_cost;
        }
    }

}
