using SLG.EnumTypes;
using SLG.RuntimeData;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SLG.Builder
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
    public class BuildingData : ScriptableObject
    {
        [Header("건물 이름")]
        [SerializeField] private string buildingName;

        [Header("건물 설명")]
        [SerializeField] private string description;

        [Header("건물 아이콘")]
        public Sprite Icon;

        [Header("건물 타입")]
        [SerializeField] private BuildingType type;

        [Header("건물 프리팹")]
        [SerializeField] private GameObject prefab;

        [Header("건물 크기")]
        [SerializeField] private int size = 1;

        [Header("건물 영향범위")]
        [SerializeField] private int influenceRange = 1;

        [Header("건설 기본 시간")]
        [SerializeField] private float buildTime = 10f;

        [Header("건설 시 영향")]
        [SerializeField] private bool isAreaRestricted = true;

        [Header("건설 시 비용")]
        [SerializeField] private List<ResourceCost> buildCost = new List<ResourceCost>();

        [Header("업그레이드 비용")]
        [SerializeField] private List<UpgradeCost> upgradeCost = new List<UpgradeCost>();

        [Header("자원건물용")]
        [SerializeField] private List<ResourceProduceData> resourceProduceDatas = new List<ResourceProduceData>();

        private Material originMaterial;

        public string BuildingName => buildingName;
        public GameObject Prefab => prefab;
        public int Size => size;
        public BuildingType Type => type;
        public int InfluenceRange => influenceRange;
        public float BuildTime => buildTime;
        public Material OriginMaterial
        {
            get
            {
                if (originMaterial == null)
                    originMaterial = prefab.GetComponentInChildren<Renderer>().sharedMaterial;
                return originMaterial;
            }
        }
        public bool IsAreaRestricted => isAreaRestricted;

        public Dictionary<ResourceType, int> GetBuildCost()
        {
            Dictionary<ResourceType, int> dic = new();
            foreach (var cost in buildCost)
                dic[cost.type] = cost.amount;

            return dic;
        }

        public Dictionary<ResourceType, int> GetUpgradeCost(int currentLevel)
        {
            if (currentLevel < 0 || currentLevel >= upgradeCost.Count)
                return null;

            Dictionary<ResourceType, int> dic = new();
            foreach(var cost in upgradeCost[currentLevel].cost)
                dic[cost.type] = cost.amount;

            return dic;
        }

        public ResourceProduceData GetResourceProduceData(int level)
        {
            int index = level - 1;

            if(resourceProduceDatas == null || 
                index > resourceProduceDatas.Count || index < 0)
            {
                throw new System.Exception($"건물 {buildingName}의 레벨 {level}에 해당하는 자원 생산 데이터가 없습니다.");
            }

            return resourceProduceDatas[index];
        }

        /// <summary>
        /// 건물 생성
        /// </summary>
        public GameObject CreateBuilding(Vector3 pos)
        {
            GameObject building = Instantiate(prefab, pos, Quaternion.identity);

            Building build = building.GetOrAddComponent<Building>();
            build.Initialize(this);

            return building;
        }
    }
}