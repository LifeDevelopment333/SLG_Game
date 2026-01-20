using SLG.EnumTypes;
using UnityEngine;

public class ResourceSystem : MonoBehaviour , IBuildingSystem
{
    [SerializeField] private ResourceType type;

    private Building building;

    private int level;
    private float timer;

    private UnitSystem unitSystem;

    public void Initialize(Building building)
    {
        this.building = building;
        level = building.Level;

        unitSystem = GetComponent<UnitSystem>();
    }

    public void Run()
    {
        if(unitSystem == null || unitSystem.HasWorkingUnit())
        {
            Debug.Log("일하는 유닛이 있어 자원 생산이 중지됩니다.");
            return;
        }

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
