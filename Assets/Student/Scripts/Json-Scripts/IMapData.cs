using UnityEngine;
using System.Collections.Generic;

public interface IMapData
{
    int Width { get; }
    int Height { get; }

    bool HasHorizontalWall(int x, int y);
    bool HasVerticalWall(int x, int y);

    float GetHorizontalWallCost(int x, int y);
    float GetVerticalWallCost(int x, int y);

    Vector2Int GetQuestStart(int questIndex);
    Vector2Int GetQuestGoal(int questIndex);

    // Vent-related methods
    bool HasVent(int x, int y);
    float GetVentCost(int x, int y);
    List<Vector2Int> GetAllVentPositions();
    List<Vector2Int> GetOtherVentPositions(Vector2Int currentVent);
}