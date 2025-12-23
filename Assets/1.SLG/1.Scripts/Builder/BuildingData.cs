using SLG.EnumTypes;
using Unity.VisualScripting;
using UnityEngine;

namespace SLG.Builder
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
    public class BuildingData : ScriptableObject
    {
        [SerializeField] private string buildingName;
        [SerializeField] private string description;
        [SerializeField] private BuildingType type;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int size = 1;
        [SerializeField] private int influenceRange = 1;
        [SerializeField] private float buildTime = 10f;        

        public string BuildingName => buildingName;
        public GameObject Prefab => prefab;
        public int Size => size;
        public BuildingType Type => type;
        public int InfluenceRange => influenceRange;
        public float BuildTime => buildTime;
        public Material OriginMaterial => prefab.GetComponentInChildren<Renderer>().sharedMaterial;

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