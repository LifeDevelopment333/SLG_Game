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
            CreateGrid();
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
                        Material = tile_Renderer.material,
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
            Debug.Log($"그리드 좌표 : {gridX} : {gridY}");

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
            if(cells == null || cells.Length == 0)
            {
                CreateGrid();
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(originPoint, 0.2f);

            Gizmos.color = Color.white;
            foreach (GridCell cell in cells)
            {
                Gizmos.DrawWireCube(cell.GridPosition, new Vector3(cellSize, 0, cellSize));
                //Gizmos.DrawSphere(cell.GridPosition, 0.05f);
                //Gizmos.DrawSphere(cells[0,0].GridPosition, 0.05f);
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