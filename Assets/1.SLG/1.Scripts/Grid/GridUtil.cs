using UnityEngine;

public static class GridUtil
{
    public static bool WorldToGrid(Vector3 worldPos, out int x, out int z, GridData data)
    {
        Vector3 local = worldPos - data.Origin;

        x = Mathf.FloorToInt(local.x / data.CellSize);
        z = Mathf.FloorToInt(local.z / data.CellSize);

        if (x < 0 || z < 0 || x >= data.GridSize || z >= data.GridSize)
            return false;

        return true;
    }

    public static Vector3 GridToWorld(int x, int z, GridData data)
    {
        return data.GetCell(x, z).GridPosition;
    }
}
