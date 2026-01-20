using SLG.RuntimeData;
using System;
using System.Collections.Generic;
using UnityEngine;

// 유닛 배치 시스템
public class UnitSystem : MonoBehaviour, IBuildingSystem
{
    [SerializeField] private int maxSlot;
    [SerializeField] private List<UnitSlot> slots = new List<UnitSlot>();

    public IReadOnlyList<UnitSlot> Slots => slots;

    // 유닛 슬롯 변경 이벤트
    public event Action<List<UnitSlot>> OnUnitSlotsChanged;

    public void Initialize(Building building)
    {
        slots.Clear();

        // 현재 건물 배치되는 유닛은 하나로 고정
        maxSlot = 1;
        slots.Add(new UnitSlot());
    }

    public void Run()
    {
        // 아직 미정
    }

    public void Upgrade(int level)
    {
        // 아직 미정
    }

    // 유닛 배치
    public bool AssignUnit(int slotIndex, UnitData unit)
    {
        if(unit == null)
        {
            Debug.LogError("할당하려는 유닛이 없습니다.");
            return false;
        }

        if(slotIndex < 0 || slotIndex >= maxSlot)
        {
            Debug.LogError("유닛 슬롯 인덱스가 잘못되었습니다.");
            return false;
        }

        if (slots.Count == 0)
        {
            Debug.LogError("유닛 슬롯이 존재하지 않습니다.");
            return false;
        }

        slots[slotIndex].unit = unit;
        OnUnitSlotsChanged(slots);

        return true;
    }

    // 유닛 해제
    public bool RemoveUnit(int slotIndex)
    {
        if(slotIndex < 0 || slotIndex >= maxSlot)
        {
            Debug.LogError("유닛 슬롯 인덱스가 잘못되었습니다.");
            return false;
        }

        if (slots.Count == 0)
        {
            Debug.LogError("유닛 슬롯이 존재하지 않습니다.");
            return false;
        }

        slots[slotIndex].unit = null;
        OnUnitSlotsChanged(slots);

        return true;
    }

    public bool HasWorkingUnit()
    {
        foreach(var slot in slots)
        {
            if (!slot.isEmpty)
                return true;
        }
        return false;
    }

    // 모든 슬롯이 꽉 찼는지 확인
    public bool IsFull()
    {
        foreach(var slot in slots)
        {
            if (slot.isEmpty)
                return false;
        }
        return true;
    }
}
