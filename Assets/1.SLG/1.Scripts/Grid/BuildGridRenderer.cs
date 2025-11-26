using UnityEngine;

public class BuildGridRenderer : MonoBehaviour
{
    [SerializeField] private GridData gridData;
    public bool enableGrid = false;

    private int previewX;
    private int previewZ;
    private int previewSize = 1;

    public void ShowPreviewGrid(int x, int z, int size)
    {
        previewX = x;
        previewZ = z;
        previewSize = size;
        enableGrid = true;
    }

    public void HidePreviewGrid()
    {
        enableGrid = false;
    }

    private void OnPostRender()
    {
        if (!enableGrid) return;
        if (gridData == null) return;

        GL.PushMatrix();
        GL.LoadIdentity();
        GL.MultMatrix(Camera.main.worldToCameraMatrix.inverse);

        Material lineMat = GetLineMaterial();
        lineMat.SetPass(0);

        GL.Begin(GL.LINES);

        // 주변 N x N만 보여줌
        for (int i = 0; i < previewSize; i++)
        {
            for (int j = 0; j < previewSize; j++)
            {
                int gx = previewX + i;
                int gz = previewZ + j;

                if (gx < 0 || gz < 0 || gx >= gridData.GridSize || gz >= gridData.GridSize)
                    continue;

                GridCell cell = gridData.GetCell(gx, gz);

                Color c = cell.isBuildable && !cell.isOccupied ? Color.green : Color.red;
                GL.Color(c);

                float cs = gridData.CellSize * 0.5f;

                Vector3 center = cell.GridPosition;

                // 4 모서리(Terrain 높이에 따라 변화)
                Vector3 A = RayToTerrain(center + new Vector3(-cs, 0, -cs));
                Vector3 B = RayToTerrain(center + new Vector3(+cs, 0, -cs));
                Vector3 C = RayToTerrain(center + new Vector3(+cs, 0, +cs));
                Vector3 D = RayToTerrain(center + new Vector3(-cs, 0, +cs));

                DrawLine(A, B);
                DrawLine(B, C);
                DrawLine(C, D);
                DrawLine(D, A);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    // Terrain에 맞게 Raycast
    Vector3 RayToTerrain(Vector3 pos)
    {
        pos.y += gridData.RayHeight;

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, gridData.RayHeight * 2, gridData.TerrainLayer))
            return hit.point;

        return pos;
    }

    void DrawLine(Vector3 a, Vector3 b)
    {
        GL.Vertex(a);
        GL.Vertex(b);
    }

    // GL용 머터리얼
    private static Material lineMat;
    Material GetLineMaterial()
    {
        if (lineMat == null)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMat = new Material(shader);
            lineMat.hideFlags = HideFlags.HideAndDontSave;
            lineMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMat.SetInt("_ZWrite", 0);
        }
        return lineMat;
    }
}
