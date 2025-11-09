using UnityEngine;

namespace SLG.Builder
{
    public class BuildManager : MonoBehaviour
    {
        private static BuildManager instance;
        public static BuildManager Instance => instance;

        [SerializeField] private Material previewMaterial;

        private Camera cam;
        private RaycastHit hit;
        private GameObject previewObject;
        private Renderer previewRenderer;

        public BuildingData selectBuilding;
        public bool isBuilder;

        private void Awake()
        {
            instance = this;
            cam = Camera.main;
        }

        private void Update()
        {
            if (isBuilder == false || selectBuilding == null) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector2Int coordinate = GridManager.Instance.WorldToGrid(hit.point);
                Vector3 MouseOnPosition = GridManager.Instance.GridToWorld(coordinate);
                if(MouseOnPosition == Vector3.zero)
                {
                    MouseOnPosition = hit.point;
                }

                if (previewObject == null)
                {
                    previewObject = Instantiate(selectBuilding.Prefab);
                    previewRenderer = previewObject.GetComponentInChildren<Renderer>();
                    SetPreviewMaterialColor(Color.green);
                }
                previewObject.transform.position = MouseOnPosition;

                bool canBuild = GridManager.Instance.CanBuild(coordinate, selectBuilding.Size);
                GridManager.Instance.HighlightBuild(coordinate, selectBuilding.Size, canBuild);
                SetPreviewMaterialColor(canBuild ? Color.green : Color.red);

                // 건물 생성
                if (Input.GetMouseButtonDown(0) && canBuild)
                {
                    selectBuilding.CreateBuilding(MouseOnPosition);
                    GridManager.Instance.CreatedBuilding(coordinate, selectBuilding.Size);
                }
            }
        }

        /// <summary>
        /// 건물 선택
        /// </summary>
        public void SelectBuilding(BuildingData data)
        {
            selectBuilding = data;
        }

        /// <summary>
        /// 건물 프리뷰
        /// </summary>
        void SetPreviewMaterialColor(Color color)
        {
            if (previewRenderer != null)
            {
                previewRenderer.material = previewMaterial;
                previewRenderer.material.SetColor("_TintColor", color);
                previewRenderer.material.SetFloat("_Alpha", 0.5f);
            }
        }
    }
}