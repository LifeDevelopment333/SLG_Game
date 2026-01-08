using SLG.EnumTypes;
using SLG.RuntimeData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SLG.SaveData
{
    [Serializable]
    public struct SaveData
    {
        public List<BuildingSaveData> buildings;
        public ResourceSaveData resources;
    }

    [Serializable]
    public struct BuildingSaveData
    {
        public string id;

        public int level;

        public int gridX;
        public int gridZ;
        public int size;
        public Vector3 rotate;

        public float buildTimer;
    }

    [Serializable]
    public struct ResourceSaveData
    {
        public List<ResourceCost> resources;
    }
}

namespace SLG.RuntimeData
{
    [Serializable]
    public struct GridCell
    {
        public Vector3 GridPosition;
        public bool isBuildable;
        public bool isOccupied;
    }

    public class PlacedBuilding
    {
        public GameObject Object;
        public int x;
        public int z;
        public int size;
    }

    public struct AreaSource
    {
        public int x;
        public int z;
        public int range;
    }

    [Serializable]
    public struct UpgradeCost
    {
        public List<ResourceCost> cost;
    }

    [Serializable]
    public struct ResourceCost
    {
        public ResourceType type;
        public int amount;
    }

    [Serializable]
    public struct ResourceProduceData
    {
        public float interval;
        public int amount;
    }
}

namespace SLG.EnumTypes
{
    public enum BuildingType
    {
        성,
        자원,
        방어,
        공격,
        유닛
    }

    public enum ResourceType
    {
        나무,
        돌,
        광석,
        골드,
        식량,
    }
}