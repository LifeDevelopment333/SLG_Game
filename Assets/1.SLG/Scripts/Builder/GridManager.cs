using UnityEngine;

namespace SLG.Builder
{
    public class GridManager : MonoBehaviour
    {
        private static GridManager instance;
        public static GridManager Instance => instance;

        [SerializeField] private Transform CenterPoint;
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
            Vector2 originPoint = new Vector3(0, 0);
            if (CenterPoint != null)
            {
                originPoint = new Vector2(CenterPoint.position.x - (width / 2), CenterPoint.position.z - (height / 2));
            }

            cells = new GridCell[10, 10];
            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new GridCell
                    {
                        GridPosition = new Vector2(originPoint.x + x, originPoint.y + y),
                        isOccupied = false
                    };
                }
            }
        }

        public void GridToWorld()
        {

        }

        public void WorldToGrid()
        {

        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            // 그리드 데이터가 없으면 미리보기용 간이 계산
            float startX = CenterPoint != null ? CenterPoint.position.x - (width / 2f) : 0f;
            float startZ = CenterPoint != null ? CenterPoint.position.z - (height / 2f) : 0f;

            // 셀 크기 = 1단위로 가정 (필요하면 CellSize 추가)
            float cellSize = 1f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 cellCenter = new Vector3(startX + x + 0.5f, 0, startZ + y + 0.5f);
                    Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0, cellSize));
                }
            }

            // 센터 포인트 표시
            if (CenterPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(CenterPoint.position, 0.2f);
            }
        }
#endif
    }

    public struct GridCell
    {
        public Vector2 GridPosition;
        public bool isOccupied;
    }
}