using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "SLG/BuildingData")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public GameObject prefab;
    public Vector2Int size = Vector2Int.one;
}
