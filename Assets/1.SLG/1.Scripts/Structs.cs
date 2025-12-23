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
}

namespace SLG.RuntimeData
{
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