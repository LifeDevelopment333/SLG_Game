using SLG.Builder;
using System.Transactions;
using UnityEngine;

public class Building : MonoBehaviour, IBuilding, IGameTick
{
    private BuildingData data;
    public BuildingData Data => data;
    private int level = 1;

    private float buildTimer = 0f;
    private bool isConstruction = false;
    private Renderer ConstructionRenderer;

    private BuildManager.PlacedBuilding placedBuilding;

    public void Initialize(BuildingData data)
    {
        this.data = data;
        transform.tag = "Building";
        gameObject.layer = LayerMask.NameToLayer("Building");
    }

    #region IGameTick 구현
    public void OnTick(float deltaTime)
    {
        if(isConstruction)
        {
            buildTimer += deltaTime;

            float progress = Mathf.Clamp01(buildTimer / data.BuildTime);
            ConstructionRenderer.material.SetFloat("_BuildProgress", progress);

            if (buildTimer >= data.BuildTime)
            {
                isConstruction = false;
                ConstructionRenderer.material = placedBuilding.Material;
                ConstructionRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                SimulationSystem.Instance.Unregister(this);
            }
            return;
        }

        // 건물 작동 로직
        //
    }
    #endregion

    public void StartConstruction(BuildManager.PlacedBuilding placedBuilding)
    {
        buildTimer = 0f;
        isConstruction = true;
        this.placedBuilding = placedBuilding;

        ConstructionRenderer = transform.GetComponentInChildren<Renderer>();

        CalculateMinMaxY(ConstructionRenderer, out float minY, out float maxY);

        ConstructionRenderer.material.SetColor("_TintColor", Color.white);
        ConstructionRenderer.material.SetFloat("_Alpha", 1f);
        ConstructionRenderer.material.SetFloat("_BuildProgress", 0f);
        ConstructionRenderer.material.SetFloat("_MinY", minY);
        ConstructionRenderer.material.SetFloat("_MaxY", maxY);

        ConstructionRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        SimulationSystem.Instance.Register(this);
    }

    private void CalculateMinMaxY(Renderer renderer, out float minY, out float maxY)
    {
        MeshFilter mf = renderer.GetComponent<MeshFilter>();

        if (mf == null || mf.sharedMesh == null)
        {
            minY = 0f;
            maxY = 1f;
            return;
        }

        Bounds bounds = mf.sharedMesh.bounds; // Object Space
        minY = bounds.min.y;
        maxY = bounds.max.y;
    }


    #region IBuilding 구현
    public void Remove()
    {
        BuildManager.Instance.RemoveBuilding(this);
    }

    public void Select()
    {
        BuildingHighlighter.Instance.ShowSelect(this);
    }

    public void DeSelect()
    {
        BuildingHighlighter.Instance.HideSelect();
    }

    public void Upgrade()
    {

    }

    public void HoverEnter()
    {
        Debug.Log("Hover Enter Building: " + data.name);
        BuildingHighlighter.Instance.ShowHover(this);
    }

    public void HoverExit()
    {
        BuildingHighlighter.Instance.HideHover();
    }
    #endregion
}
