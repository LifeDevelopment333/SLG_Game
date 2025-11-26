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

    private void OnRenderObject()
    {
        if (!enableGrid) return;
        if (gridData == null) return;

        // 🔥 GameView 렌더링 카메라 확인
        if (Camera.current != Camera.main)
            return;

        // 🔥 GL 시작
        Material mat = GetLineMaterial();
        if (mat == null)
        {
            Debug.LogWarning("Line Material 생성 실패!");
            return;
        }

        mat.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(Camera.main.worldToCameraMatrix.inverse);
        GL.Begin(GL.LINES);

        DrawPreviewGrid();

        GL.End();
        GL.PopMatrix();
    }

    private void DrawPreviewGrid()
    {
        for (int i = 0; i < previewSize; i++)
        {
            for (int j = 0; j < previewSize; j++)
            {
                int gx = previewX + i;
                int gz = previewZ + j;

                if (gx < 0 || gz < 0 || gx >= gridData.GridSize || gz >= gridData.GridSize)
                    continue;

                GridCell cell = gridData.GetCell(gx, gz);

                // 🔥 셀 색상
                GL.Color(cell.isBuildable && !cell.isOccupied ? Color.green : Color.red);

                float cs = gridData.CellSize * 0.5f;
                Vector3 center = cell.GridPosition;

                // 🔥 모서리 4개를 Terrain에 맞게 Raycast
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
    }

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

    // 🔥 URP에서도 안정적으로 작동하는 GL 머티리얼
    private static Material lineMat;
    Material GetLineMaterial()
    {
        if (lineMat == null)
        {
            // URP 호환 헤덴 셰이더 적용
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            if (shader == null)
            {
                Debug.LogError("Internal-Colored Shader not found!");
                return null;
            }

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
