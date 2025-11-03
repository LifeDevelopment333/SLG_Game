using UnityEngine;

namespace SLG.Builder
{
    public class GridManager : MonoBehaviour
    {
        private static GridManager instance;
        public static GridManager Instance => instance;

        [SerializeField] private Vector3 OriginPoint = new Vector3(0, 0, 0);
        private GridCell[,] cells;

        public int width = 10;
        public int height = 10;

        private void Awake()
        {
            instance = this;

            CreateGrid();
        }

        /// <summary>
        /// 격자 그리드 생성
        /// </summary>
        void CreateGrid()
        {
            cells = new GridCell[10, 10];
            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new GridCell
                    {
                        GridPosition = new Vector2(OriginPoint.x + x, OriginPoint.z + y),
                        isOccupied = false
                    };
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            for (int x = 0; x <= width; x++)
                Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, height));
            for (int y = 0; y <= height; y++)
                Gizmos.DrawLine(new Vector3(0, 0, y), new Vector3(width, 0, y));
        }
    }

    public struct GridCell
    {
        public Vector2 GridPosition;
        public bool isOccupied;
    }
}