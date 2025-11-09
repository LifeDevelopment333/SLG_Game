using System;
using UnityEngine;

namespace SLG.Builder
{
    public class GridManager : MonoBehaviour
    {
        private static GridManager instance;
        public static GridManager Instance => instance;

        [SerializeField] private Transform CenterPointObject;
        [SerializeField] private Material tileMaterial;
        [SerializeField] private Transform parentTiles;
        private Vector3 originPoint;
        private GridCell[,] cells;

        [Header("격자 옵션")]
        public int width = 10;
        public int height = 10;
        public int cellSize = 1;
        [SerializeField] Color canBuildColor = Color.green;
        [SerializeField] Color cannotBuildColor = Color.red;

        public bool isDebugMode = false;

        private void Awake()
        {
            instance = this;

            CreateGrid();
        }

        /// <summary>
        /// 격자 재생성
        /// </summary>
        public void ReCreateGrid()
        {
            DeleteGrid();
            CreateGrid();
        }

        void DeleteGrid()
        {
            if (cells.Length <= 0) return;

            cells = new GridCell[0, 0];

            foreach (Transform child in parentTiles)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        /// <summary>
        /// 격자 그리드 생성
        /// </summary>
        void CreateGrid()
        {
            originPoint = Vector3.zero;
            if (CenterPointObject != null)
            {
                originPoint = new Vector3(
                    CenterPointObject.position.x - (width * cellSize) / 2f,
                    CenterPointObject.position.y,
                    CenterPointObject.position.z - (height * cellSize) / 2f
                    );
            }

            cells = new GridCell[width, height];

            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    tile.transform.parent = parentTiles;

                    Vector3 pos = new Vector3(
                        originPoint.x + (x * cellSize) + (cellSize / 2f),
                        originPoint.y,
                        originPoint.z + (y * cellSize) + (cellSize / 2f)
                        );

                    tile.transform.position = pos + (Vector3.up * 0.01f);
                    tile.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    tile.transform.localScale = Vector3.one * cellSize;

                    tile.name = $"Tile_{x}_{y}";
                    MeshRenderer tile_Renderer = tile.GetComponent<MeshRenderer>();
                    tile_Renderer.material = new Material(tileMaterial);
                    tile_Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    tile_Renderer.receiveShadows = false;

                    cells[x, y] = new GridCell
                    {
                        GridPosition = pos,
                        Material = tile_Renderer.sharedMaterial,
                        isOccupied = false
                    };
                }
            }
        }

        /// <summary>
        /// 그리드 좌표를 월드 포지션으로
        /// </summary>
        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            if(cells == null || cells.Length == 0) return Vector3.zero;
            if (gridPos.x >= width || gridPos.y >= height || gridPos.x < 0 || gridPos.y < 0) return Vector3.zero;

            // 그리드 좌표를 월드 좌표로 변환하는 로직
            return cells[gridPos.x, gridPos.y].GridPosition;
        }

        /// <summary>
        /// 월드 좌표를 그리드 좌표로
        /// </summary>
        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            // 월드 좌표를 그리드 좌표로 변환하는 로직
            int gridX = Mathf.FloorToInt((worldPos.x - originPoint.x) / cellSize);
            int gridY = Mathf.FloorToInt((worldPos.z - originPoint.z) / cellSize);

            return new Vector2Int(gridX, gridY);
        }

        public bool CanBuild(Vector2Int gridPos, Vector2Int buildSize)
        {
            if (cells == null || cells.Length == 0) return false;
            if (gridPos.x >= width || gridPos.y >= height || gridPos.x < 0 || gridPos.y < 0) return false;

            for(int x = 0; x < buildSize.x; x++)
            {
                for(int y = 0; y < buildSize.y; y++)
                {
                    if (gridPos.x + x >= width || gridPos.y + y >= height) return false;
                    if (cells[gridPos.x + x, gridPos.y + y].isOccupied) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 타일 하이라이트 강조 (건설 가능/불가능)
        /// </summary>
        public void HighlightBuild(Vector2Int gridPos, Vector2Int buildSize, bool canBuild)
        {
            if (cells == null || cells.Length == 0) return;

            // 모든 타일 초기화 (살짝 투명한 회색)
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cells[x, y].Material == null) continue;
                    cells[x, y].Material.color = new Color(1f, 1f, 1f, 0.05f);
                }
            }

            // 범위 내 타일 강조
            Color c = canBuild ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.4f);

            for (int x = 0; x < buildSize.x; x++)
            {
                for (int y = 0; y < buildSize.y; y++)
                {
                    int gx = gridPos.x + x;
                    int gy = gridPos.y + y;

                    if (gx < 0 || gy < 0 || gx >= width || gy >= height)
                        continue;

                    if (cells[gx, gy].Material != null)
                        cells[gx, gy].Material.color = c;
                }
            }
        }

        public void CreatedBuilding(Vector2Int gridPos, Vector2Int buildSize)
        {
            for(int x = 0; x < buildSize.x; x++)
            {
                for(int y = 0; y < buildSize.y; y++)
                {
                    cells[gridPos.x + x, gridPos.y + y].isOccupied = true;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!isDebugMode)
                return;

            // 1️⃣ originPoint 계산을 CreateGrid()와 동일하게 보장
            Vector3 debugOrigin = Vector3.zero;
            if (CenterPointObject != null)
            {
                debugOrigin = new Vector3(
                    CenterPointObject.position.x - (width * cellSize) / 2f,
                    CenterPointObject.position.y,
                    CenterPointObject.position.z - (height * cellSize) / 2f
                );
            }

            // 2️⃣ CenterPoint 및 Origin 시각화
            if (CenterPointObject != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(CenterPointObject.position, 0.2f);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(debugOrigin, 0.1f);

            // 3️⃣ 타일 위치와 동일하게 Y 오프셋 적용
            float yOffset = 0.01f;

            // 4️⃣ 격자 라인 표시
            Gizmos.color = new Color(1f, 1f, 1f, 0.4f);
            float totalWidth = width * cellSize;
            float totalHeight = height * cellSize;

            for (int y = 0; y <= height; y++)
            {
                Vector3 p1 = debugOrigin + new Vector3(0, yOffset, y * cellSize);
                Vector3 p2 = debugOrigin + new Vector3(totalWidth, yOffset, y * cellSize);
                Gizmos.DrawLine(p1, p2);
            }

            for (int x = 0; x <= width; x++)
            {
                Vector3 p1 = debugOrigin + new Vector3(x * cellSize, yOffset, 0);
                Vector3 p2 = debugOrigin + new Vector3(x * cellSize, yOffset, totalHeight);
                Gizmos.DrawLine(p1, p2);
            }

            // 5️⃣ 셀 중심점 표시
            Gizmos.color = Color.green;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 cellCenter = debugOrigin + new Vector3(
                        (x * cellSize) + (cellSize / 2f),
                        yOffset,
                        (y * cellSize) + (cellSize / 2f)
                    );
                    Gizmos.DrawSphere(cellCenter, 0.05f);
                }
            }
        }
#endif
    }

    public struct GridCell
    {
        public Vector3 GridPosition;
        public Material Material;
        public bool isOccupied;
    }
}