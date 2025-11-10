using SLG.Builder;
using System.Collections.Generic;
using UnityEngine;

public class BuildingList_Dropdown : MonoBehaviour
{
    private List<string> buildingNames;
    [SerializeField] private TMPro.TMP_Dropdown dropdown;

    private void Start()
    {
        buildingNames = LoadBuildingData.Instance.GetAllBuildingNames();

        SetItem();

        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void SetItem()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(buildingNames);
    }

    public void OnValueChanged(int index)
    {
        string selectedName = buildingNames[index];
        BuildingData data = LoadBuildingData.Instance.GetBuildingDataByName(selectedName);
        BuildManager.Instance.SelectBuilding(data);
    }
}
