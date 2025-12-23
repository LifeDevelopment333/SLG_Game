using SLG.Builder;
using SLG.RuntimeData;
using SLG.SaveData;
using UnityEngine;

public class Building : MonoBehaviour, IBuilding, IGameTick, ISaveData<BuildingSaveData>
{
    private BuildingData data;
    public BuildingData Data => data;

    private float buildTimer = 0f;
    private bool isConstruction = false;
    private Renderer ConstructionRenderer;
    private Material originMaterial;

    private PlacedBuilding placedBuilding = new PlacedBuilding();

    public void Initialize(BuildingData data)
    {
        this.data = data;
        transform.tag = "Building";
        gameObject.layer = LayerMask.NameToLayer("Building");
    }

    private void OnDestroy()
    {
        SimulationSystem.Instance.Unregister(this);
    }

    #region IGameTick 구현
    public void OnTick(float deltaTime)
    {
        if(isConstruction)
        {
            buildTimer -= deltaTime;

            float progress = Mathf.Clamp01((data.BuildTime - buildTimer) / data.BuildTime);
            ConstructionRenderer.material.SetFloat("_BuildProgress", progress);

            if (buildTimer <= 0)
            {
                buildTimer = 0f;

                isConstruction = false;
                ConstructionRenderer.material = data.OriginMaterial;
                ConstructionRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                SimulationSystem.Instance.Unregister(this);
            }
            return;
        }

        // 건물 작동 로직
        //
    }
    #endregion

    public void StartConstruction(PlacedBuilding placedBuilding)
    {
        buildTimer = data.BuildTime;
        this.placedBuilding = placedBuilding;

        StartConstructionRenderer();
    }

    private void StartConstructionRenderer()
    {
        isConstruction = true;

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
        minY = bounds.min.z;
        maxY = bounds.max.z;
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

    public BuildingSaveData SaveData()
    {
        BuildingSaveData data = new BuildingSaveData();

        data.id = this.data.BuildingName;

        data.gridX = placedBuilding.x;
        data.gridZ = placedBuilding.z;
        data.rotate = transform.eulerAngles;
        data.size = placedBuilding.size;

        data.buildTimer = buildTimer;

        return data;
    }

    public void LoadData(BuildingSaveData data)
    {
        buildTimer = data.buildTimer;

        placedBuilding.x = data.gridX;
        placedBuilding.z = data.gridZ;
        placedBuilding.size = data.size;

        if(buildTimer > 0)
        {
            Renderer renderer = transform.GetComponentInChildren<Renderer>();
            BuildManager.Instance.ApplyPreviewMaterial(Color.white, renderer);
            StartConstructionRenderer();
        }
    }
    #endregion
}
