using UnityEngine;

public class GridMapManager : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public float cellSize = 1f;

    private GridCell[,] cells;

    void Awake()
    {
        cells = new GridCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new GridCell
                {
                    GridPosition = new Vector2Int(x, y),
                    IsOccupied = false
                };
            }
        }
    }
    // 안녕하세요

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, 0, gridPos.y * cellSize);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / cellSize),
            Mathf.FloorToInt(worldPos.z / cellSize)
        );
    }

    public bool CanPlace(Vector2Int gridPos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int checkX = gridPos.x + x;
                int checkY = gridPos.y + y;
                if (checkX < 0 || checkY < 0 || checkX >= width || checkY >= height)
                    return false;
                if (cells[checkX, checkY].IsOccupied)
                    return false;
            }
        }
        return true;
    }

    public void PlaceBuilding(Vector2Int gridPos, Vector2Int size, GameObject building)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int checkX = gridPos.x + x;
                int checkY = gridPos.y + y;
                cells[checkX, checkY].IsOccupied = true;
            }
        }

        building.transform.position = GridToWorld(gridPos);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x <= width; x++)
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, height));
        for (int y = 0; y <= height; y++)
            Gizmos.DrawLine(new Vector3(0, 0, y), new Vector3(width, 0, y));
    }
}

public class GridCell
{
    public Vector2Int GridPosition;
    public bool IsOccupied;
}


