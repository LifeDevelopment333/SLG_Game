using SLG.Builder;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSelect : MonoBehaviour
{
    [SerializeField] private LayerMask buildingLayer;

    private Building selectBuilding;
    private Building hoverBuilding;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (BuildManager.Instance.IsBuildMode == true || EventSystem.current.IsPointerOverGameObject()) return;

        HandleHover();
        HandleClick();
    }

    private void HandleHover()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 500f, buildingLayer))
        {
            Building building = hit.transform.GetComponentInParent<Building>();
            if (hoverBuilding != building)
            {
                hoverBuilding?.HoverExit();
                hoverBuilding = building;
                hoverBuilding.HoverEnter();
            }
        }
        else
        {
            hoverBuilding?.HoverExit();
            hoverBuilding = null;
        }
    }

    private void HandleClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(hoverBuilding != selectBuilding)
            {
                selectBuilding?.DeSelect();
                selectBuilding = hoverBuilding;
                selectBuilding?.Select();
            }
        }
    }
}
