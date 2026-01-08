using SLG.EnumTypes;
using UnityEngine;

public class ResourceSystem : MonoBehaviour , IBuildingSystem
{
    [SerializeField] private ResourceType type;

    private Building building;

    private int level;
    private float timer;

    public void Initialize(Building building)
    {
        this.building = building;
        level = building.Level;
    }

    public void Run()
    {
        timer += Time.deltaTime;

        if (timer >= building.Data.GetResourceProduceData(level).interval)
        {
            timer -= building.Data.GetResourceProduceData(level).interval;

            ResourceManager.Instance.Add(type, building.Data.GetResourceProduceData(level).amount);
        }
    }

    public void Upgrade(int level)
    {
        this.level = level;
    }
}
