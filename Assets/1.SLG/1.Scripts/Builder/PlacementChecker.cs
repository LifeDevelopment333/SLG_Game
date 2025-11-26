using UnityEngine;

public static class PlacementChecker
{
    public static bool CanBuild(int x, int z, GridData data, int size)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int px = x + i;
                int pz = z + j;

                if (px < 0 || pz < 0 || px >= data.GridSize || pz >= data.GridSize)
                    return false;

                GridCell cell = data.GetCell(px, pz);

                if (!cell.isBuildable) return false;
                if (cell.isOccupied) return false;
            }
        }

        return true;
    }

}
