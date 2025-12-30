using SLG.Builder;
using SLG.RuntimeData;
using SLG.SaveData;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour, IBuilding, IGameTick, ISaveData<BuildingSaveData>
{
    private BuildingData data;
    public BuildingData Data => data;

    private float buildTimer = 0f;
    private bool isConstruction = false;
    private Renderer ConstructionRenderer;
    private PlacedBuilding placedBuilding = new PlacedBuilding();
    private List<IBuildingSystem> buildingSystems = new List<IBuildingSystem>();

    private int level = 1;
    public int Level => level;

    public bool IsConstruction => isConstruction;
    public PlacedBuilding PlacedBuilding => placedBuilding;

    public void Initialize(BuildingData data)
    {
        this.data = data;
        transform.tag = "Building";
        gameObject.layer = LayerMask.NameToLayer("Building");

        buildingSystems = GetComponents<IBuildingSystem>().ToList();

        SystemInitialize();
    }

    private void SystemInitialize()
    {
        foreach (IBuildingSystem system in buildingSystems)
        {
            system.Initialize(this);
        }
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
            }
            return;
        }

        foreach(IBuildingSystem system in buildingSystems)
        {
            system.Run();
        }
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

    public bool CanUpgrade()
    {
        var cost = data.GetUpgradeCost(level);

        if (cost == null) return false;

        return ResourceManager.Instance.CanConsume(cost);
    }

    public void Upgrade()
    {
        var cost = data.GetUpgradeCost(level);
        if (cost == null) return;

        if (ResourceManager.Instance.CanConsume(cost) == false) return;

        ResourceManager.Instance.Consume(cost);

        level++;

        foreach (IBuildingSystem system in buildingSystems)
        {
            system.Upgrade(level);
        }
    }

    public void HoverEnter()
    {
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

        data.level = this.level;

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

        level = data.level;

        placedBuilding.x = data.gridX;
        placedBuilding.z = data.gridZ;
        placedBuilding.size = data.size;

        if (buildTimer > 0)
        {
            Renderer renderer = transform.GetComponentInChildren<Renderer>();
            BuildManager.Instance.ApplyPreviewMaterial(Color.white, renderer);
            StartConstructionRenderer();
        }
        else
        {
            isConstruction = false;
            SimulationSystem.Instance.Register(this);
        }
    }
    #endregion
}
