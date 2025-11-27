using UnityEngine;

public class BuildGridRenderer : MonoBehaviour
{
    [SerializeField] private GridData gridData;
    [SerializeField] private Material lineMaterial;  // Unlit
    [SerializeField] private Material fillMaterial;  // Unlit Transparent

    private MeshFilter lineMF, fillMF;
    private MeshRenderer lineMR, fillMR;

    private int previewX, previewZ, previewSize;
    private bool showGrid = false;

    public float lineWidth = 0.05f;
    public float heightOffset = 0.02f;
    public float fillOffset = 0.01f;       // fill plane is lower than line slightly

    private void Awake()
    {
        // Mesh for Lines
        GameObject lineObj = new GameObject("GridLines");
        lineObj.transform.SetParent(transform, false);
        lineMF = lineObj.AddComponent<MeshFilter>();
        lineMR = lineObj.AddComponent<MeshRenderer>();
        lineMR.material = lineMaterial;

        // Mesh for Fill
        GameObject fillObj = new GameObject("GridFill");
        fillObj.transform.SetParent(transform, false);
        fillMF = fillObj.AddComponent<MeshFilter>();
        fillMR = fillObj.AddComponent<MeshRenderer>();
        fillMR.material = fillMaterial;
    }

    public void ShowPreviewGrid(int x, int z, int size)
    {
        previewX = x;
        previewZ = z;
        previewSize = size;

        showGrid = true;
        GenerateMeshes();
    }

    public void HidePreviewGrid()
    {
        showGrid = false;
        lineMF.mesh = null;
        fillMF.mesh = null;
    }

    private void GenerateMeshes()
    {
        if (!showGrid || gridData == null) return;

        GenerateLineMesh();
        //GenerateFillMesh();
    }

    // -------------------------------------------------------------
    // 1) Line Mesh 생성 (Build 가능 = Green, 불가 Red)
    // -------------------------------------------------------------
    private void GenerateLineMesh()
    {
        int cellCount = previewSize * previewSize;
        int lineCount = cellCount * 4;

        Vector3[] verts = new Vector3[lineCount * 4];
        int[] tris = new int[lineCount * 6];

        int idx = 0;

        int startX = GridUtil.GetStartX(previewX, previewSize);
        int startZ = GridUtil.GetStartZ(previewZ, previewSize);

        for (int i = 0; i < previewSize; i++)
        {
            for (int j = 0; j < previewSize; j++)
            {
                int gx = startX + i;
                int gz = startZ + j;

                if (gx < 0 || gz < 0 || gx >= gridData.GridSize || gz >= gridData.GridSize)
                    continue;

                GridCell cell = gridData.GetCell(gx, gz);

                // 라인 색상 설정
                Color lineColor = (cell.isBuildable && !cell.isOccupied) ? Color.green : Color.red;
                lineMR.material.color = lineColor;

                float cs = gridData.CellSize * 0.5f;
                Vector3 center = cell.GridPosition;

                Vector3 A = RayToTerrain(center + new Vector3(-cs, 0, -cs));
                Vector3 B = RayToTerrain(center + new Vector3(+cs, 0, -cs));
                Vector3 C = RayToTerrain(center + new Vector3(+cs, 0, +cs));
                Vector3 D = RayToTerrain(center + new Vector3(-cs, 0, +cs));

                LineMeshGenerator.AddLine(A, B, lineWidth, ref idx, verts, tris);
                LineMeshGenerator.AddLine(B, C, lineWidth, ref idx, verts, tris);
                LineMeshGenerator.AddLine(C, D, lineWidth, ref idx, verts, tris);
                LineMeshGenerator.AddLine(D, A, lineWidth, ref idx, verts, tris);
            }
        }

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.vertices = verts;
        m.triangles = tris;
        m.RecalculateNormals();
        m.RecalculateBounds();

        lineMF.sharedMesh = m;
    }

    // -------------------------------------------------------------
    // 2) Fill Mesh 생성 (반투명 내부 사각형)
    // -------------------------------------------------------------
    private void GenerateFillMesh()
    {
        int cellCount = previewSize * previewSize;

        Vector3[] verts = new Vector3[cellCount * 4];
        int[] tris = new int[cellCount * 6];
        int v = 0;
        int t = 0;

        int half = previewSize / 2;
        int startX = previewX - half;
        int startZ = previewZ - half;

        for (int i = 0; i < previewSize; i++)
        {
            for (int j = 0; j < previewSize; j++)
            {
                int gx = startX + i;
                int gz = startZ + j;

                if (gx < 0 || gz < 0 || gx >= gridData.GridSize || gz >= gridData.GridSize)
                    continue;

                GridCell cell = gridData.GetCell(gx, gz);

                float cs = gridData.CellSize * 0.5f;
                Vector3 center = cell.GridPosition - Vector3.up * fillOffset;

                // Fill 색
                Color fillColor = (cell.isBuildable && !cell.isOccupied) ?
                    new Color(0, 1, 0, 0.2f) :
                    new Color(1, 0, 0, 0.2f);

                fillMR.material.color = fillColor;

                Vector3 A = center + new Vector3(-cs, 0, -cs);
                Vector3 B = center + new Vector3(+cs, 0, -cs);
                Vector3 C = center + new Vector3(+cs, 0, +cs);
                Vector3 D = center + new Vector3(-cs, 0, +cs);

                verts[v + 0] = A;
                verts[v + 1] = B;
                verts[v + 2] = C;
                verts[v + 3] = D;

                tris[t + 0] = v + 0;
                tris[t + 1] = v + 1;
                tris[t + 2] = v + 2;

                tris[t + 3] = v + 0;
                tris[t + 4] = v + 2;
                tris[t + 5] = v + 3;

                v += 4;
                t += 6;
            }
        }

        Mesh fillMesh = new Mesh();
        fillMesh.vertices = verts;
        fillMesh.triangles = tris;
        fillMesh.RecalculateNormals();
        fillMesh.RecalculateBounds();

        fillMF.sharedMesh = fillMesh;
    }

    private Vector3 RayToTerrain(Vector3 pos)
    {
        pos.y += gridData.RayHeight;

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, gridData.RayHeight * 2, gridData.TerrainLayer))
            return hit.point + Vector3.up * heightOffset;

        return pos + Vector3.up * heightOffset;
    }
}
