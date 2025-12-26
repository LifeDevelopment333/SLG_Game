using SLG.EnumTypes;
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

        public string BuildingName => buildingName;
        public GameObject Prefab => prefab;
        public int Size => size;
        public BuildingType Type => type;
        public int InfluenceRange => influenceRange;
        public float BuildTime => buildTime;
        public Material OriginMaterial => prefab.GetComponentInChildren<Renderer>().sharedMaterial;
        public bool IsAreaRestricted => isAreaRestricted;

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