using UnityEngine;

namespace SLG.Builder
{
    public class BuildManager : MonoBehaviour
    {
        private static BuildManager instance;
        public static BuildManager Instance => instance;

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

                if (previewObject == null)
                {
                    previewObject = Instantiate(selectBuilding.prefab);
                    previewRenderer = previewObject.GetComponentInChildren<Renderer>();
                    SetPreviewMaterialColor(Color.green);
                }
                previewObject.transform.position = MouseOnPosition;

                bool canBuild = GridManager.Instance.CanBuild(coordinate);
                SetPreviewMaterialColor(canBuild ? Color.green : Color.red);

                if (Input.GetMouseButtonDown(0) && canBuild)
                {
                    GameObject building = Instantiate(selectBuilding.prefab);
                    building.transform.position = MouseOnPosition;
                }
            }
        }

        void SetPreviewMaterialColor(Color color)
        {
            if (previewRenderer != null)
            {
                foreach (var mat in previewRenderer.materials)
                {
                    mat.color = color;
                }
            }
        }
    }
}