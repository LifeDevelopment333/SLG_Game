using NUnit.Framework.Constraints;
using SLG.Builder;
using System;
using UnityEngine;

public class BuildingHighlighter : MonoBehaviour
{
    private static BuildingHighlighter instance;
    public static BuildingHighlighter Instance => instance;

    [SerializeField] private GridData gridData;
    [Header("하이라이트 라인 머터리얼")]
    [SerializeField] private Material lineHighlightMaterial;
    [Header("확장 하이라이트 라인 머터리얼")]
    [SerializeField] private Material expand_lineHightlightMaterial;

    private int hoverX, hoverZ, hoverSize, hoverRot;
    private int selectX, selectZ, selectSize, selectRot, selectInfluence;

    private MeshFilter lineMF, expandMF;
    private MeshRenderer lineMR, expandMR;

    [Header("렌더러 설정")]
    public float lineWidth = 0.05f;
    public float heightOffset = 0.02f;
    public float fillOffset = 0.01f;

    private void Awake()
    {
        instance = this;

        GameObject line = new GameObject("BuildingHighlightLine");
        line.transform.SetParent(transform, false);
        lineMF = line.AddComponent<MeshFilter>();
        lineMR = line.AddComponent<MeshRenderer>();
        lineMR.material = lineHighlightMaterial;
        lineMR.material.renderQueue = 4000;

        GameObject expandLine = new GameObject("BuildingExpandHighlightLine");
        expandLine.transform.SetParent(transform, false);
        expandMF = expandLine.AddComponent<MeshFilter>();
        expandMR = expandLine.AddComponent<MeshRenderer>();
        expandMR.material = expand_lineHightlightMaterial;
        expandMR.material.renderQueue = 3000;
    }

    private void GenerateLineMesh()
    {
        int cellCount = hoverSize * hoverSize;

        MeshBuffer buffer = new MeshBuffer(cellCount);

        int startX = GridUtil.GetStartX(hoverX, hoverSize);
        int startZ = GridUtil.GetStartZ(hoverZ, hoverSize);

        for(int i = 0; i < hoverSize; i++)
        {
            for(int j = 0; j < hoverSize; j++)
            {
                RotationUtil.RotateCell(i, j, hoverSize, hoverRot, out int ri, out int rj);
                int gx = startX + ri;
                int gz = startZ + rj;

                if(gx < 0 || gx >= gridData.GridSize || gz < 0 || gz >= gridData.GridSize)
                    continue;
                
                GridCell cell = gridData.GetCell(gx, gz);
                Vector3 center = cell.GridPosition;
                float cs = gridData.CellSize * 0.5f;

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

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        m.SetVertices(buffer.verts);
        m.SetTriangles(buffer.tris, 0);

        lineMF.sharedMesh = m;
    }

    private void GenerateExpandLineMesh()
    {
        int influence = selectInfluence;

        // 1. 건물 실제 타일 시작점
        int startX = GridUtil.GetStartX(selectX, selectSize);
        int startZ = GridUtil.GetStartZ(selectZ, selectSize);

        // 2. 전체 포함 범위 계산
        int minX = startX - influence;
        int maxX = startX + (selectSize - 1) + influence;

        int minZ = startZ - influence;
        int maxZ = startZ + (selectSize - 1) + influence;

        MeshBuffer buffer = new MeshBuffer((maxX - minX + 1) * (maxZ - minZ + 1));

        for (int gx = minX; gx <= maxX; gx++)
        {
            for (int gz = minZ; gz <= maxZ; gz++)
            {
                if (gx < 0 || gx >= gridData.GridSize || gz < 0 || gz >= gridData.GridSize)
                    continue;

                GridCell cell = gridData.GetCell(gx, gz);
                Vector3 center = cell.GridPosition;

                float cs = gridData.CellSize * 0.5f;

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

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        m.SetVertices(buffer.verts);
        m.SetTriangles(buffer.tris, 0);

        expandMF.sharedMesh = m;
    }

    private Vector3 Ray(Vector3 pos)
    {
        pos.y += gridData.RayHeight;

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, gridData.RayHeight * 2, gridData.TerrainLayer))
            return hit.point + Vector3.up * heightOffset;

        return pos + Vector3.up * heightOffset;
    }

    public void ShowHover(Building building)
    {
        GridUtil.WorldToGrid(building.transform.position, out int x, out int z, gridData);

        hoverX = x;
        hoverZ = z;
        hoverSize = building.Data.Size;
        hoverRot = NormalizeRotation(building.transform.eulerAngles.y);

        GenerateLineMesh();
    }

    public void HideHover()
    {
        ClearMesh();
    }

    public void ShowSelect(Building building)
    {
        GridUtil.WorldToGrid(building.transform.position, out int x, out int z, gridData);

        selectX = x;
        selectZ = z;
        selectSize = building.Data.Size;
        selectRot = NormalizeRotation(building.transform.eulerAngles.y);
        selectInfluence = building.Data.InfluenceRange;

        GenerateExpandLineMesh();
    }

    public void HideSelect()
    {
        ClearExpandMesh();
    }

    private int NormalizeRotation(float rot)
    {
        int r = Mathf.RoundToInt(rot / 90f) * 90;
        r %= 360;
        if (r < 0) r += 360;
        return r;
    }

    private void ClearMesh()
    {
        lineMF.sharedMesh = null;
    }

    private void ClearExpandMesh()
    {
        expandMF.sharedMesh = null;
    }
}
