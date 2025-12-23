using UnityEngine;
using SLG.RuntimeData;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridDataGenerator : MonoBehaviour
{
    [SerializeField] private GridData gridData;

    private GridCell[] runtimeCells; // 그리드 생성 후 임시 저장 (Gizmos용)

    public bool isDebug = false;
    public bool isReset = false;

    private void Awake()
    {
        if (isReset) GenerateGrid();
    }

    public void GenerateGrid()
    {
        if (gridData == null)
        {
            Debug.LogError("GridData ScriptableObject가 비어 있습니다!");
            return;
        }

        int gridSize = gridData.GridSize;
        float cellSize = gridData.CellSize;
        float rayHeight = gridData.RayHeight;
        float slope = gridData.BuildableSlope;
        LayerMask terrainLayer = gridData.TerrainLayer;

        runtimeCells = new GridCell[gridSize * gridSize];
        Vector3 origin = transform.position;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                int index = gridData.Index(x, z);

                Vector3 rayStart = origin + new Vector3(x * cellSize, rayHeight, z * cellSize);

                GridCell cell = new GridCell();

                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayHeight * 2f, terrainLayer))
                {
                    cell.GridPosition = hit.point;
                    cell.isBuildable = hit.normal.y >= slope;
                }
                else
                {
                    cell.GridPosition = origin + new Vector3(x * cellSize, 0f, z * cellSize);
                    cell.isBuildable = false;
                }

                runtimeCells[index] = cell;
            }
        }

        // ScriptableObject에 저장
        ApplyToScriptableObject(runtimeCells);

        Debug.Log("Grid 생성완료: " + gridSize + " x " + gridSize);
    }

    private void ApplyToScriptableObject(GridCell[] result)
    {
        gridData.GetType(); // 방지용 (사용되지 않음이라 경고뜨는 것 억제)

        gridData.GetType();
        typeof(GridData).GetField("cells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(gridData, result);

#if UNITY_EDITOR
        EditorUtility.SetDirty(gridData);
        AssetDatabase.SaveAssets();
#endif
    }

    private void OnDrawGizmos()
    {
        if (runtimeCells == null || isDebug == false) return;

        int gridSize = gridData.GridSize;
        float cellSize = gridData.CellSize;

        // 보정: position = GridDataGenerator 오브젝트가 월드 기준 0,0 시작
        Vector3 origin = transform.position;

        for (int x = 0; x < gridSize - 1; x++)
        {
            for (int z = 0; z < gridSize - 1; z++)
            {
                int idx = gridData.Index(x, z);
                GridCell cell = runtimeCells[idx];

                // 빌드 가능? 색상 선택
                Gizmos.color = cell.isBuildable ? Color.green : Color.red;

                // 셀 중심 위치
                Vector3 center = cell.GridPosition;

                // 셀 모서리 4개 계산
                Vector3 A = new Vector3(center.x - cellSize * 0.5f, center.y, center.z - cellSize * 0.5f);
                Vector3 B = new Vector3(center.x + cellSize * 0.5f, center.y, center.z - cellSize * 0.5f);
                Vector3 C = new Vector3(center.x + cellSize * 0.5f, center.y, center.z + cellSize * 0.5f);
                Vector3 D = new Vector3(center.x - cellSize * 0.5f, center.y, center.z + cellSize * 0.5f);

                // 라인 4개 그리기
                Gizmos.DrawLine(A, B);
                Gizmos.DrawLine(B, C);
                Gizmos.DrawLine(C, D);
                Gizmos.DrawLine(D, A);
            }
        }
    }
}
