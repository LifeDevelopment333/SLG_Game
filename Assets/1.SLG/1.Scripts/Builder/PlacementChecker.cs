using UnityEngine;

public static class PlacementChecker
{
    /// <summary>
    /// 건설 가능 여부
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="data"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static bool CanBuild(int x, int z, GridData data, int size, int rotation)
    {
        int startX = GridUtil.GetStartX(x, size);
        int startZ = GridUtil.GetStartZ(z, size);
  
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                RotationUtil.RotateCell(i, j, size, rotation, out int ri, out int rj);

                int px = startX + ri;
                int pz = startZ + rj;

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
