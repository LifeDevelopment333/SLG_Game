using SLG.RuntimeData;
using System.Collections.Generic;
using UnityEngine;

public class BuildGridRenderer : MonoBehaviour
{
    [SerializeField] private GridData gridData;

    [Header("라인 머터리얼 (2개 순서 고정: Green, Red)")]
    [SerializeField] private Material[] lineMaterials; // [0] = green, [1] = red

    [Header("채우기 머터리얼 (2개 순서 고정: Green, Red)")]
    [SerializeField] private Material[] fillMaterials; // [0] = green, [1] = red

    [Header("확장 선 머터리얼")]
    [SerializeField] private Material expandLineMaterial;

    [Header("확장 범위 설정")]
    public int expandRange = 10;

    private MeshFilter lineMF, fillMF, expandMF, areaMF;
    private MeshRenderer lineMR, fillMR, expandMR, areaMR;

    private int previewX, previewZ, previewSize, previewRot;
    private bool showGrid = false;
    private bool checkArea = false;

    private List<AreaSource> areaSources = new List<AreaSource>();

    [Header("렌더러 설정")]
    public float lineWidth = 0.05f;
    public float heightOffset = 0.02f;
    public float fillOffset = 0.01f;

    private void Awake()
    {
        // 선
        GameObject lineObj = new GameObject("GridLines");
        lineObj.transform.SetParent(transform, false);
        lineMF = lineObj.AddComponent<MeshFilter>();
        lineMR = lineObj.AddComponent<MeshRenderer>();
        lineMR.materials = lineMaterials; // 반드시 2개
        lineMR.material.renderQueue = 2500;

        // 채우기
        GameObject fillObj = new GameObject("GridFill");
        fillObj.transform.SetParent(transform, false);
        fillMF = fillObj.AddComponent<MeshFilter>();
        fillMR = fillObj.AddComponent<MeshRenderer>();
        fillMR.materials = fillMaterials; // 반드시 2개
        fillMR.material.renderQueue = 2500;

        // 확장선
        GameObject expandObj = new GameObject("GridExpand");
        expandObj.transform.SetParent(transform, false);
        expandMF = expandObj.AddComponent <MeshFilter>();
        expandMR = expandObj.AddComponent<MeshRenderer>();
        expandMR.material = expandLineMaterial;
        expandMR.material.renderQueue = 2500;

        // 성 영역
        GameObject areaObj = new GameObject("GridArea");
        areaObj.transform.SetParent(transform, false);
        areaMF = areaObj.AddComponent<MeshFilter>();
        areaMR = areaObj.AddComponent<MeshRenderer>();
        areaMR.material = expandLineMaterial;
        areaMR.material.renderQueue = 2500;
    }

    public void ShowPreviewGrid(int x, int z, int size, int rotation, int range, bool checkArea)
    {
        previewX = x;
        previewZ = z;
        previewSize = size;
        previewRot = rotation;
        expandRange = range;
        this.checkArea = checkArea;

        showGrid = true;
        GenerateMeshes();
    }

    public void ShowCastleAreas(List<AreaSource> areas)
    {
        areaSources = areas;
        showGrid = true;
        GenerateExpandMesh_Multi();
    }

    public void HidePreviewGrid()
    {
        showGrid = false;
        lineMF.mesh = null;
        fillMF.mesh = null;
        expandMF.mesh = null;
        areaMF.mesh = null;
    }

    private void GenerateMeshes()
    {
        if (!showGrid || gridData == null) return;

        GenerateLineMesh_SubMesh();
        GenerateFillMesh_SubMesh();
        GenerateFillMesh_ExpandMesh();
    }

    private void GenerateExpandMesh_Multi()
    {
        MeshBuffer buf = new MeshBuffer(1024);

        HashSet<int> visited = new HashSet<int>();

        foreach (var area in areaSources)
        {
            int cx = area.x;
            int cz = area.z;
            int range = area.range;

            for (int i = -range; i <= range; i++)
            {
                for (int j = -range; j <= range; j++)
                {
                    int gx = cx + i;
                    int gz = cz + j;

                    if (gx < 0 || gz < 0 ||
                        gx >= gridData.GridSize || gz >= gridData.GridSize)
                        continue;

                    int key = gridData.Index(gx, gz);
                    if (!visited.Add(key))
                        continue; // 이미 그린 셀

                    GridCell cell = gridData.GetCell(gx, gz);

                    float cs = gridData.CellSize * 0.5f;
                    Vector3 center = cell.GridPosition;

                    Vector3 A = Ray(center + new Vector3(-cs, 0, -cs));
                    Vector3 B = Ray(center + new Vector3(+cs, 0, -cs));
                    Vector3 C = Ray(center + new Vector3(+cs, 0, +cs));
                    Vector3 D = Ray(center + new Vector3(-cs, 0, +cs));

                    buf.AddLine(A, B, lineWidth);
                    buf.AddLine(B, C, lineWidth);
                    buf.AddLine(C, D, lineWidth);
                    buf.AddLine(D, A, lineWidth);
                }
            }
        }

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.SetVertices(buf.verts);
        m.SetTriangles(buf.tris, 0);

        areaMF.sharedMesh = m;
    }

    /// <summary>
    /// 라인 매쉬
    /// </summary>
    private void GenerateLineMesh_SubMesh()
    {
        int cellCount = previewSize * previewSize;

        MeshBuffer green = new MeshBuffer(cellCount * 4);
        MeshBuffer red = new MeshBuffer(cellCount * 4);

        int startX = GridUtil.GetStartX(previewX, previewSize);
        int startZ = GridUtil.GetStartZ(previewZ, previewSize);

        for (int i = 0; i < previewSize; i++)
        {
            for (int j = 0; j < previewSize; j++)
            {
                RotationUtil.RotateCell(i, j, previewSize, previewRot, out int ri, out int rj);
                int gx = startX + ri;
                int gz = startZ + rj;

                if (gx < 0 || gz < 0 || gx >= gridData.GridSize || gz >= gridData.GridSize)
                    continue;

                GridCell cell = gridData.GetCell(gx, gz);
                bool canBuild = (cell.isBuildable && !cell.isOccupied);
                if(checkArea)
                {
                    canBuild &= AreaSystem.Instance.IsInAnyBuildArea(gx, gz);
                }
                MeshBuffer buffer = canBuild ? green : red;

                float cs = gridData.CellSize * 0.5f;
                Vector3 center = cell.GridPosition;

                Vector3 A = Ray(center + new Vector3(-cs, 0, -cs));
                Vector3 B = Ray(center + new Vector3(+cs, 0, -cs));
                Vector3 C = Ray(center + new Vector3(+cs, 0, +cs));
                Vector3 D = Ray(center + new Vector3(-cs, 0, +cs));

                buffer.AddLine(A, B, lineWidth);
                buffer.AddLine(B, C, lineWidth);
                buffer.AddLine(C, D, lineWidth);
                buffer.AddLine(D, A, lineWidth);
            }
        }

        // 🔹 버텍스 하나로 합치기
        int greenVertCount = green.verts.Count;
        int redVertCount = red.verts.Count;

        var allVerts = new List<Vector3>(greenVertCount + redVertCount);
        allVerts.AddRange(green.verts);
        allVerts.AddRange(red.verts);

        // 🔹 빨강 삼각형 인덱스는 버텍스 개수만큼 offset
        var redTrisOffset = new List<int>(red.tris.Count);
        foreach (int id in red.tris)
            redTrisOffset.Add(id + greenVertCount);

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.subMeshCount = 2;

        m.SetVertices(allVerts);
        m.SetTriangles(green.tris, 0);      // submesh 0 = green
        m.SetTriangles(redTrisOffset, 1);   // submesh 1 = red

        m.RecalculateNormals();
        m.RecalculateBounds();

        lineMF.sharedMesh = m;
    }

    /// <summary>
    /// 채우기용 매쉬
    /// </summary>
    private void GenerateFillMesh_SubMesh()
    {
        int cellCount = previewSize * previewSize;

        MeshBuffer green = new MeshBuffer(cellCount);
        MeshBuffer red = new MeshBuffer(cellCount);

        int startX = GridUtil.GetStartX(previewX, previewSize);
        int startZ = GridUtil.GetStartZ(previewZ, previewSize);

        for (int i = 0; i < previewSize; i++)
        {
            for (int j = 0; j < previewSize; j++)
            {
                RotationUtil.RotateCell(i, j, previewSize, previewRot, out int ri, out int rj);
                int gx = startX + ri;
                int gz = startZ + rj;

                if (gx < 0 || gz < 0 || gx >= gridData.GridSize || gz >= gridData.GridSize)
                    continue;

                GridCell cell = gridData.GetCell(gx, gz);
                bool canBuild = (cell.isBuildable && !cell.isOccupied);
                if (checkArea)
                {
                    canBuild &= AreaSystem.Instance.IsInAnyBuildArea(gx, gz);
                }

                MeshBuffer buffer = canBuild ? green : red;

                float cs = gridData.CellSize * 0.5f;
                Vector3 center = cell.GridPosition;

                Vector3 A = Ray(center + new Vector3(-cs, 0, -cs));
                Vector3 B = Ray(center + new Vector3(+cs, 0, -cs));
                Vector3 C = Ray(center + new Vector3(+cs, 0, +cs));
                Vector3 D = Ray(center + new Vector3(-cs, 0, +cs));

                buffer.AddQuad(A, B, C, D);
            }
        }

        int greenVertCount = green.verts.Count;
        int redVertCount = red.verts.Count;

        var allVerts = new List<Vector3>(greenVertCount + redVertCount);
        allVerts.AddRange(green.verts);
        allVerts.AddRange(red.verts);

        var redTrisOffset = new List<int>(red.tris.Count);
        foreach (int id in red.tris)
            redTrisOffset.Add(id + greenVertCount);

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.subMeshCount = 2;

        m.SetVertices(allVerts);
        m.SetTriangles(green.tris, 0);
        m.SetTriangles(redTrisOffset, 1);

        m.RecalculateNormals();
        m.RecalculateBounds();

        fillMF.sharedMesh = m;
    }

    private void GenerateFillMesh_ExpandMesh()
    {
        int range = expandRange;
        int maxCount = (range * 2 + 1) * (range * 2 + 1);

        MeshBuffer buf = new MeshBuffer(maxCount * 4);

        int startX = GridUtil.GetStartX(previewX, previewSize);
        int startZ = GridUtil.GetStartZ(previewZ, previewSize);

        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                int gx = previewX + i;
                int gz = previewZ + j;

                if (gx < 0 || gz < 0 || gx >= gridData.GridSize || gz >= gridData.GridSize)
                    continue;

                GridCell cell = gridData.GetCell(gx, gz);

                if (cell.isOccupied)
                    continue;

                float cs = gridData.CellSize * 0.5f;
                Vector3 center = cell.GridPosition;

                Vector3 A = Ray(center + new Vector3(-cs, 0, -cs));
                Vector3 B = Ray(center + new Vector3(+cs, 0, -cs));
                Vector3 C = Ray(center + new Vector3(+cs, 0, +cs));
                Vector3 D = Ray(center + new Vector3(-cs, 0, +cs));

                buf.AddLine(A, B, lineWidth);
                buf.AddLine(B, C, lineWidth);
                buf.AddLine(C, D, lineWidth);
                buf.AddLine(D, A, lineWidth);
            }
        }

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        m.SetVertices(buf.verts);
        m.SetTriangles(buf.tris, 0);

        expandMF.sharedMesh = m;
    }

    private Vector3 Ray(Vector3 pos)
    {
        pos.y += gridData.RayHeight;

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, gridData.RayHeight * 2, gridData.TerrainLayer))
            return hit.point + Vector3.up * heightOffset;

        return pos + Vector3.up * heightOffset;
    }

    private void ClearGridAndMesh()
    {
        lineMF.sharedMesh = null;
        fillMF.sharedMesh = null;
        expandMF.sharedMesh = null;
    }
}

public class MeshBuffer
{
    public List<Vector3> verts = new List<Vector3>();
    public List<int> tris = new List<int>();

    private int vertIndex = 0;

    public MeshBuffer(int max)
    {
        verts.Capacity = max * 4;
        tris.Capacity = max * 6;
    }

    public void AddLine(Vector3 a, Vector3 b, float width)
    {
        Vector3 dir = (b - a).normalized;
        Vector3 side = Vector3.Cross(Vector3.up, dir) * (width * 0.5f);

        Vector3 v0 = a - side;
        Vector3 v1 = a + side;
        Vector3 v2 = b - side;
        Vector3 v3 = b + side;

        verts.Add(v0);
        verts.Add(v1);
        verts.Add(v2);
        verts.Add(v3);

        tris.Add(vertIndex + 0);
        tris.Add(vertIndex + 1);
        tris.Add(vertIndex + 2);

        tris.Add(vertIndex + 2);
        tris.Add(vertIndex + 1);
        tris.Add(vertIndex + 3);

        vertIndex += 4;
    }

    public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        verts.Add(a);
        verts.Add(b);
        verts.Add(c);
        verts.Add(d);

        tris.Add(vertIndex + 0);
        tris.Add(vertIndex + 1);
        tris.Add(vertIndex + 2);

        tris.Add(vertIndex + 0);
        tris.Add(vertIndex + 2);
        tris.Add(vertIndex + 3);

        vertIndex += 4;
    }
}


