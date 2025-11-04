using System;
using UnityEngine;

namespace SLG.Builder
{
    public class GridManager : MonoBehaviour
    {
        private static GridManager instance;
        public static GridManager Instance => instance;

        [SerializeField] private Transform CenterPointObject;
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
                    cells[x, y] = new GridCell
                    {
                        GridPosition = new Vector3(
                            originPoint.x + (x * cellSize) + (cellSize / 2f),
                            originPoint.y,
                            originPoint.z + (y * cellSize) + (cellSize / 2f)
                            ),
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

        public bool CanBuild(Vector2Int gridPos)
        {
            if (cells == null || cells.Length == 0) return false;
            if (gridPos.x >= width || gridPos.y >= height || gridPos.x < 0 || gridPos.y < 0) return false;

            return !cells[gridPos.x, gridPos.y].isOccupied;
        }

        public void CreateBuilding(Vector2Int gridPos)
        {
            cells[gridPos.x, gridPos.y].isOccupied = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(cells == null)
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
        public bool isOccupied;
    }
}