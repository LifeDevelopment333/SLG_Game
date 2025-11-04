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
        [SerializeField] private Vector2Int size = Vector2Int.one;

        public GameObject Prefab => prefab;
        public Vector2Int Size => size;
        public BuildingType Type => type;
    }

    public enum BuildingType
    {
        자원,
        군사
    }
}