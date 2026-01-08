using SLG.EnumTypes;
using SLG.RuntimeData;
using SLG.SaveData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour , ISaveData<ResourceSaveData>
{
    private static ResourceManager instance;
    public static ResourceManager Instance => instance;

    // 현재 자원
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    public IReadOnlyDictionary<ResourceType, int> Resources => resources;

    // 초기 자원 설정
    public List<ResourceCost> initResources;

    // 이벤트
    public event Action<ResourceType, int> OnResourceChanged;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // 초기 생성되는 자원 Load시 문제 발생 시 제거필요
        foreach (var resource in initResources)
        {
            resources[resource.type] = resource.amount;
            OnResourceChanged?.Invoke(resource.type, resource.amount);
        }
    }

    public int Get(ResourceType type)
    {
        return resources[type];
    }

    public void Add(ResourceType type, int amount)
    {
        resources[type] += amount;
        OnResourceChanged?.Invoke(type, resources[type]);
    }

    public bool CanSpend(ResourceType type, int amount)
    {
        if(resources.ContainsKey(type))
            return resources[type] >= amount;

        return false;
    }

    public bool CanConsume(Dictionary<ResourceType, int> cost)
    {
        foreach (var pair in cost)
            if (!CanSpend(pair.Key, pair.Value))
                return false;

        return true;
    }

    // 구매
    public void Consume(Dictionary<ResourceType, int> cost)
    {
        foreach (var pair in cost)
        {
            resources[pair.Key] -= pair.Value;
            OnResourceChanged?.Invoke(pair.Key, resources[pair.Key]);
        }
    }

    public bool Spend(ResourceType type, int amount)
    {
        if (CanSpend(type, amount) == false) return false;

        resources[type] -= amount;
        OnResourceChanged?.Invoke(type, resources[type]);

        return true;
    }

    // 멀티 자원 계산
    public Dictionary<ResourceType, int> MultiplyCost(Dictionary<ResourceType, int> baseCost, int count)
    {
        Dictionary<ResourceType, int> result = new();

        foreach (var pair in baseCost)
        {
            result[pair.Key] = pair.Value * count;
        }

        return result;
    }

    public ResourceSaveData SaveData()
    {
        ResourceSaveData data = new ResourceSaveData();
        data.resources = new List<ResourceCost>();
        foreach (var pair in resources)
        {
            ResourceCost resourceCost = new ResourceCost();
            resourceCost.type = pair.Key;
            resourceCost.amount = pair.Value;
            data.resources.Add(resourceCost);
        }
        return data;
    }

    public void LoadData(ResourceSaveData data)
    {
        resources.Clear();
        foreach (var resource in data.resources)
        {
            resources[resource.type] = resource.amount;
            OnResourceChanged?.Invoke(resource.type, resource.amount);
        }
    }
}
