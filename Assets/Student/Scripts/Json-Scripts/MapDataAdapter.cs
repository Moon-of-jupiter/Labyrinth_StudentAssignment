using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class MapDataAdapter : IMapData
{
    private readonly MapData mapData;
    private readonly int offsetX;
    private readonly int offsetY;

    public MapDataAdapter(MapData mapData, int offsetX = 0, int offsetY = 0)
    {
        this.mapData = mapData;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
    }

    public int Width => mapData.width;
    public int Height => mapData.height;

    public bool HasHorizontalWall(int x, int y)
    {
        return GetHorizontalWallCost(x, y) > 1.0f;
    }

    public bool HasVerticalWall(int x, int y)
    {
        return GetVerticalWallCost(x, y) > 1.0f;
    }

    public float GetHorizontalWallCost(int x, int y)
    {
        if (mapData.hwalls == null)
            return 1.0f;

        int checkX = x + offsetX;
        int checkY = y + offsetY;

        var wall = mapData.hwalls.FirstOrDefault(w => w.x == checkX && w.y == checkY);

        if (wall != null)
        {
            return wall.cost;
        }

        return 1.0f; // Default cost if no wall is found
    }

    public float GetVerticalWallCost(int x, int y)
    {
        if (mapData.vwalls == null)
            return 1.0f;

        int checkX = x + offsetX;
        int checkY = y + offsetY;

        var wall = mapData.vwalls.FirstOrDefault(w => w.x == checkX && w.y == checkY);

        if (wall != null)
        {
            return wall.cost;
        }

        return 1.0f; // Default cost if no wall is found
    }

    public Vector2Int GetQuestStart(int questIndex)
    {
        if (mapData.quests == null || questIndex < 0 || questIndex >= mapData.quests.Length)
            return Vector2Int.zero;

        var quest = mapData.quests[questIndex];
        return new Vector2Int(quest.from.x - offsetX, quest.from.y - offsetY);
    }

    public Vector2Int GetQuestGoal(int questIndex)
    {
        if (mapData.quests == null || questIndex < 0 || questIndex >= mapData.quests.Length)
            return Vector2Int.zero;

        var quest = mapData.quests[questIndex];
        return new Vector2Int(quest.to.x - offsetX, quest.to.y - offsetY);
    }

    // New vent-related methods
    public bool HasVent(int x, int y)
    {
        if (mapData.vents == null)
            return false;

        int checkX = x + offsetX;
        int checkY = y + offsetY;

        return mapData.vents.Any(v => v.x == checkX && v.y == checkY);
    }

    public float GetVentCost(int x, int y)
    {
        if (mapData.vents == null)
            return float.MaxValue;

        int checkX = x + offsetX;
        int checkY = y + offsetY;

        var vent = mapData.vents.FirstOrDefault(v => v.x == checkX && v.y == checkY);
        return vent?.cost ?? float.MaxValue;
    }

    public List<Vector2Int> GetAllVentPositions()
    {
        if (mapData.vents == null)
            return new List<Vector2Int>();

        return mapData.vents.Select(v => new Vector2Int(v.x - offsetX, v.y - offsetY)).ToList();
    }

    public List<Vector2Int> GetOtherVentPositions(Vector2Int currentVent)
    {
        var allVents = GetAllVentPositions();
        return allVents.Where(v => v != currentVent).ToList();
    }
}