using System;
using System.Collections.Generic;
using UnityEngine;

namespace SLG.SaveData
{
    [Serializable]
    public struct SaveData
    {
        public List<BuildingSaveData> buildings;
    }

    [Serializable]
    public struct BuildingSaveData
    {
        public string id;

        public int gridX;
        public int gridZ;
        public int size;
        public Vector3 rotate;

        public float buildTimer;
    }

    public struct CastleSaveData
    {
        public int level;
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
}