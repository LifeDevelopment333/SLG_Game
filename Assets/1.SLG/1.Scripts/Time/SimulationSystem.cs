using SLG.EnumTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSystem : MonoBehaviour
{
    private static SimulationSystem instance;
    public static SimulationSystem Instance => instance;

    private List<IGameTick> ticks = new List<IGameTick>();

    private float accumulatedTime = 0f;     // 누적된 시간
    private float dayTimer = 0f;
    private int currentDay = 1;
    private DayState currentDayState = DayState.Day;

    [SerializeField] private int dayDuration = 300; // 하루 지속 시간 (초)
    [SerializeField] private int nightDuration = 180; // 밤 지속 시간 (초)

    public event Action<int> OnDayChanged;
    public event Action OnDayStarted;
    public event Action OnNightStarted;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        float dt = GameTimeSystem.Instance.DeltaTime;
        if(dt <= 0f)
            return;

        accumulatedTime += dt;
        dayTimer += dt;

        UpdateDayTime();

        if (dt > 0)
        {
            for (int i = 0; i < ticks.Count; i++)
            {
                ticks[i].OnTick(dt);
            }
        }
    }

    private void UpdateDayTime()
    {
        if(currentDayState == DayState.Day && dayTimer >= dayDuration)
        {
            currentDayState = DayState.Night;
            dayTimer = 0;
            StartNight();
        }
        else if(currentDayState == DayState.Night && dayTimer >= nightDuration)
        {
            currentDayState = DayState.Day;
            dayTimer = 0;
            currentDay++;

            StartDay();
        }
    }

    // 밤 시작
    private void StartDay()
    {
        OnDayChanged?.Invoke(currentDay);
        OnDayStarted?.Invoke();

        Debug.Log($"{currentDay}째 낮 시작");
    }

    // 낮 시작
    private void StartNight()
    {
        OnNightStarted?.Invoke();

        Debug.Log($"{currentDay}째 밤 시작");
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
