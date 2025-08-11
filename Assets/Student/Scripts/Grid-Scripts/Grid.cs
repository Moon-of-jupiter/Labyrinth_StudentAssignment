using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    [Header("Grid Settings")]
    public int xSize = 10;
    public int zSize = 10;
    public float cellSize = 1f;
    public float gridHeight = 0f;

    [Header("Visualization")]
    public bool showGridLines = true;
    public bool showCellCenters = false;

    private Mesh mesh;

    private void Start()
    {
        //GenerateMesh();
    }

    public void GenerateMesh()
    {
        mesh = new Mesh { name = "Procedural Grid" };
        GetComponent<MeshFilter>().mesh = mesh;

        // Create vertices
        int vertexCount = (xSize + 1) * (zSize + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x * cellSize, gridHeight, z * cellSize);
                uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
            }
        }

        // Create triangles
        int[] triangles = new int[xSize * zSize * 6];
        for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public Vector3 GridToWorldPosition(int gridX, int gridZ)
    {
        return new Vector3(gridX * cellSize, gridHeight, gridZ * cellSize);
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.z / cellSize)
        );
    }

    public Vector3 GetCellCenter(int gridX, int gridZ)
    {
        return new Vector3(
            gridX * cellSize + cellSize * 0.5f,
            gridHeight,
            gridZ * cellSize + cellSize * 0.5f
        );
    }

    public bool IsValidGridPosition(int gridX, int gridZ)
    {
        return gridX >= 0 && gridX < xSize && gridZ >= 0 && gridZ < zSize;
    }

    private void OnDrawGizmos()
    {
        if (!showGridLines && !showCellCenters) return;

        if (showGridLines)
        {
            DrawGridLines();
        }

        if (showCellCenters)
        {
            DrawCellCenters();
        }

        // Draw grid bounds
        Gizmos.color = Color.red;
        Vector3 center = new Vector3(xSize * cellSize * 0.5f, gridHeight, zSize * cellSize * 0.5f);
        Vector3 size = new Vector3(xSize * cellSize, 0.1f, zSize * cellSize);
        Gizmos.DrawWireCube(transform.TransformPoint(center), size);
    }

    private void DrawGridLines()
    {
        Gizmos.color = Color.cyan;

        // Horizontal lines
        for (int z = 0; z <= zSize; z++)
        {
            Vector3 start = new Vector3(0, gridHeight, z * cellSize);
            Vector3 end = new Vector3(xSize * cellSize, gridHeight, z * cellSize);
            Gizmos.DrawLine(transform.TransformPoint(start), transform.TransformPoint(end));
        }

        // Vertical lines
        for (int x = 0; x <= xSize; x++)
        {
            Vector3 start = new Vector3(x * cellSize, gridHeight, 0);
            Vector3 end = new Vector3(x * cellSize, gridHeight, zSize * cellSize);
            Gizmos.DrawLine(transform.TransformPoint(start), transform.TransformPoint(end));
        }
    }

    private void DrawCellCenters()
    {
        Gizmos.color = Color.yellow;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                Vector3 center = GetCellCenter(x, z);
                Gizmos.DrawSphere(transform.TransformPoint(center), 0.1f);
            }
        }
    }
}