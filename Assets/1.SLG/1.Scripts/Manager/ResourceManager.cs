using SLG.EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;
    public static ResourceManager Instance => instance;

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this;
    }

    public int Get(ResourceType type)
    {
        return resources[type];
    }

    public void Add(ResourceType type, int amount)
    {
        resources[type] = amount;
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
            resources[pair.Key] -= pair.Value;
    }

    public bool Spend(ResourceType type, int amount)
    {
        if (CanSpend(type, amount) == false) return false;

        resources[type] -= amount;

        return true;
    }
}
