using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public GridMapManager grid;
    public Camera cam;
    public BuildingData selectedBuilding;
    private GameObject previewObj;
    private Renderer previewRenderer;

    void Update()
    {
        if (selectedBuilding == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 worldPos = hit.point;
            Vector2Int gridPos = grid.WorldToGrid(worldPos);
            Vector3 snappedPos = grid.GridToWorld(gridPos);

            if (previewObj == null)
            {
                previewObj = Instantiate(selectedBuilding.prefab);
                previewRenderer = previewObj.GetComponentInChildren<Renderer>();
                SetPreviewMaterialColor(Color.green);
            }

            previewObj.transform.position = snappedPos;

            bool canPlace = grid.CanPlace(gridPos, selectedBuilding.size);
            SetPreviewMaterialColor(canPlace ? Color.green : Color.red);

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                var building = Instantiate(selectedBuilding.prefab);
                grid.PlaceBuilding(gridPos, selectedBuilding.size, building);
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
