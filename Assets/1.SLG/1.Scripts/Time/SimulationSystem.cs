using System.Collections.Generic;
using UnityEngine;

public class SimulationSystem : MonoBehaviour
{
    private static SimulationSystem instance;
    public static SimulationSystem Instance => instance;

    private List<IGameTick> ticks = new List<IGameTick>();

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        float dt = GameTimeSystem.Instance.DeltaTime;
        if (dt > 0)
        {
            for (int i = 0; i < ticks.Count; i++)
            {
                ticks[i].OnTick(dt);
            }
        }
    }

    public void Register(IGameTick tick)
    {
        if(ticks.Contains(tick) == false)
        {
            ticks.Add(tick);
        }
    }

    public void Unregister(IGameTick tick)
    {
        if(ticks.Contains(tick))
        {
            ticks.Remove(tick);
        }
    }
}
