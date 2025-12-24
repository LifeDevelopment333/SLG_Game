using System.Collections.Generic;
using UnityEngine;

public class AreaSystem : MonoBehaviour
{
    private static AreaSystem instance;
    public static AreaSystem Instance => instance;

    private List<CastleSystem> systemList = new List<CastleSystem>();

    private void Awake()
    {
        instance = this;
    }

    public void Register(CastleSystem system)
    {
        systemList.Add(system);
    }

    public void Unregister(CastleSystem system)
    {
        systemList.Remove(system);
    }

    /// <summary>
    /// 캐슬의 범위에 들어있는지 체크
    /// </summary>
    public bool IsInAnyBuildArea(int x, int z)
    {
        foreach (CastleSystem system in systemList)
        {
            if (system.IsInArea(x, z))
                return true;
        }

        return false;
    }
}
