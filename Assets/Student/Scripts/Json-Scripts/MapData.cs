using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[Serializable]
public class MapData
{
    public int width;
    public int height;
    public Wall[] hwalls;
    public Wall[] vwalls;
    public Vents[] vents;
    public Quest[] quests;
}

[Serializable]
public class Wall
{
    public int x;
    public int y;
    public float cost = float.MaxValue;
}

[Serializable]
public class Vents
{
    public int x;
    public int y;
    public float cost = 10f;
}

[Serializable]
public class Quest
{
    public Position from;
    public Position to;
}

[Serializable]
public class Position
{
    public int x;
    public int y;
}
