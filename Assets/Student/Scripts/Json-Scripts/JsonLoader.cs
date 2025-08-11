using UnityEngine;
using System.IO;
using System;

public class JsonLoader : MonoBehaviour
{
    //Aviable filenames ex1.json" : "ex2.json"
    [Header("Json Filename")]
    public string fileName;

    [Header("Quest Selection")]
    [Range(0, 2)]
    [SerializeField] private int questIndex = 0;
    private int previousQuestIndex = -1;

    [Header("Prefabs")]
    public GameObject hwallPrefab;
    public GameObject vwallPrefab;
    public GameObject hWallLowPrefab;
    public GameObject vWallLowPrefab;

    public GameObject groundPrefab;
    public GameObject ventPrefab;

    public GameObject startPrefab;
    public GameObject endPrefab;

    [Header("Grid Reference")]
    public Grid grid;

    // Public properties for external access
    public MapData MapData { get; private set; }
    public int MinX { get; private set; }
    public int MinY { get; private set; }

    private GameObject currentStartMarker;
    private GameObject currentEndMarker;

    private float hWallOffset;
    private float vWallOffset;

    void Start()
    {
        // Calculate wall offsets for proper positioning
        GameObject tempH = Instantiate(hwallPrefab);
        hWallOffset = tempH.GetComponent<Renderer>().bounds.size.x / 2f;
        Destroy(tempH);

        GameObject tempV = Instantiate(vwallPrefab);
        vWallOffset = tempV.GetComponent<Renderer>().bounds.size.z / 2f;
        Destroy(tempV);

        LoadJsonFile(fileName);

    }

    void Update()
    {
        // Check if quest index has changed
        if (questIndex != previousQuestIndex)
        {
            previousQuestIndex = questIndex;

            // Destroy old markers
            if (currentStartMarker != null)
                Destroy(currentStartMarker);
            if (currentEndMarker != null)
                Destroy(currentEndMarker);

            // Spawn new markers
            SpawnQuestMarkers();
        }
    }

    
    public MapData GetMapData()
    {
        return MapData;
    }

    
    public int GetQuestIndex()
    {
        return questIndex;
    }

    
    public int GetMinX()
    {
        return MinX;
    }

    public int GetMinY()
    {
        return MinY;
    }



    public void LoadJsonFile(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            MapData = JsonHelper.FromJson<MapData>(json);

            Debug.Log($"Map loaded successfully: {fileName}");

            CalculateBounds();
            SpawnWalls();
            SpawnVents();
            SpawnGroundTiles();
            SpawnQuestMarkers();

            GridCharacterMovement character = FindAnyObjectByType<GridCharacterMovement>();
            if (character != null)
            {
                character.InitializeCharacterAtStart();
            }
        }
        else
        {
            Debug.LogError("Could not find file: " + path);
        }
    }

    private void CalculateBounds()
    {
        if (MapData == null || MapData.hwalls == null || MapData.vwalls == null)
            return;

        // Initialize min/max values
        MinX = int.MaxValue;
        MinY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        // Find bounds from horizontal walls
        foreach (var wall in MapData.hwalls)
        {
            MinX = Mathf.Min(MinX, wall.x);
            maxX = Mathf.Max(maxX, wall.x + 1);
            MinY = Mathf.Min(MinY, wall.y);
            maxY = Mathf.Max(maxY, wall.y);
        }

        // Find bounds from vertical walls
        foreach (var wall in MapData.vwalls)
        {
            MinX = Mathf.Min(MinX, wall.x);
            maxX = Mathf.Max(maxX, wall.x);
            MinY = Mathf.Min(MinY, wall.y);
            maxY = Mathf.Max(maxY, wall.y + 1);
        }

        // Set normalized dimensions
        MapData.width = maxX - MinX;
        MapData.height = maxY - MinY;

        // Update grid size
        grid.xSize = MapData.width;
        grid.zSize = MapData.height;
        grid.GenerateMesh();
    }

    private void SpawnWalls()
    {
        // Spawn horizontal walls
        foreach (var wall in MapData.hwalls)
        {
            Vector3 pos = new Vector3(wall.x - MinX + hWallOffset, 0, wall.y - MinY);

            if (Mathf.Approximately(wall.cost, 5.5f))
            {
                Instantiate(hWallLowPrefab, pos, hWallLowPrefab.transform.rotation);
            }
            else if (Mathf.Approximately(wall.cost, float.MaxValue))
            {
                Instantiate(hwallPrefab, pos, hwallPrefab.transform.rotation);
            }
            // Otherwise: skip spawning
        } 

        // Spawn vertical walls
        foreach (var wall in MapData.vwalls)
        {
            Vector3 pos = new Vector3(wall.x - MinX, 0, wall.y - MinY + vWallOffset);

            if (Mathf.Approximately(wall.cost, 5.5f))
            {
                Instantiate(vWallLowPrefab, pos, vWallLowPrefab.transform.rotation);
            }
            else if (Mathf.Approximately(wall.cost, float.MaxValue))
            {
                Instantiate(vwallPrefab, pos, vwallPrefab.transform.rotation);
            }
            // Otherwise: skip spawning
        }

        SpawnOuterBoundary();
    }


    private void SpawnOuterBoundary()
    {
        // Top and bottom boundaries
        for (int x = 0; x < grid.xSize; x++)
        {
            Instantiate(hwallPrefab, new Vector3(x + hWallOffset, 0, 0), hwallPrefab.transform.rotation);
            Instantiate(hwallPrefab, new Vector3(x + hWallOffset, 0, grid.zSize), hwallPrefab.transform.rotation);
        }

        // Left and right boundaries
        for (int y = 0; y < grid.zSize; y++)
        {
            Instantiate(vwallPrefab, new Vector3(0, 0, y + vWallOffset), vwallPrefab.transform.rotation);
            Instantiate(vwallPrefab, new Vector3(grid.xSize, 0, y + vWallOffset), vwallPrefab.transform.rotation);
        }
    }

    private void SpawnQuestMarkers()
    {
        if (MapData?.quests == null || MapData.quests.Length == 0)
        {
            Debug.LogWarning("No quests found in map data.");
            return;
        }

        if (questIndex < 0 || questIndex >= MapData.quests.Length)
        {
            Debug.LogWarning($"Quest index {questIndex} out of range.");
            return;
        }

        var quest = MapData.quests[questIndex];

        // Convert to normalized grid coordinates
        int fromX = quest.from.x - MinX;
        int fromY = quest.from.y - MinY;
        int toX = quest.to.x - MinX;
        int toY = quest.to.y - MinY;

        // Get world positions
        Vector3 fromPos = grid.GetCellCenter(fromX, fromY);
        Vector3 toPos = grid.GetCellCenter(toX, toY);

        // Spawn markers
        currentStartMarker = Instantiate(startPrefab, fromPos, Quaternion.identity);
        currentEndMarker = Instantiate(endPrefab, toPos, Quaternion.identity);

        Debug.Log($"Quest {questIndex}: Start({fromX}, {fromY}) → Goal({toX}, {toY})");
    }

    public Vector2Int GetStartGridPosition()
    {
        if (MapData?.quests == null || MapData.quests.Length == 0)
            return Vector2Int.zero;

        var quest = MapData.quests[questIndex];
        int fromX = quest.from.x - MinX;
        int fromY = quest.from.y - MinY;

        return new Vector2Int(fromX, fromY);
    }

    private void SpawnVents()
    {
        if (MapData?.vents == null || MapData.vents.Length == 0)
            return;

        foreach (var vent in MapData.vents)
        {
            int ventX = vent.x - MinX;
            int ventY = vent.y - MinY;

            Vector3 pos = grid.GetCellCenter(ventX, ventY);
            pos.y += 0.015f;

            Instantiate(ventPrefab, pos, Quaternion.identity);
        }
    }

    private void SpawnGroundTiles()
    {
        if (groundPrefab == null || grid == null)
        {
            Debug.LogWarning("Missing ground prefab or grid reference.");
            return;
        }

        for (int x = 0; x < grid.xSize; x++)
        {
            for (int y = 0; y < grid.zSize; y++)
            {
                Vector3 centerPos = grid.GetCellCenter(x, y);
                Instantiate(groundPrefab, centerPos, Quaternion.identity);
            }
        }
    }
}