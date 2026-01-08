using SLG.Builder;
using SLG.RuntimeData;
using SLG.SaveData;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/save.json";

    public static void SaveGame()
    {
        SaveData data = new SaveData();

        data.buildings = new List<BuildingSaveData>();
        foreach(var building in BuildManager.Instance.Buildings)
        {
            data.buildings.Add(building.SaveData());
        }

        data.resources = new ResourceSaveData();
        data.resources = ResourceManager.Instance.SaveData();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static void LoadGame()
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File path is Wrong");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 로직구현
        #region 빌딩 관련
        BuildManager.Instance.ClearAll();

        foreach (BuildingSaveData loadData in data.buildings)
        {
            // 빌디유형 찾기
            BuildingData buildingData = LoadBuildingData.Instance.GetBuildingDataByName(loadData.id);

            // 빌딩 월드포지션 찾기
            Vector3 worldPos = GridUtil.GridToWorld(loadData.gridX, loadData.gridZ, BuildManager.Instance.Mapdata);

            GameObject buildObject = buildingData.CreateBuilding(worldPos);
            buildObject.transform.localEulerAngles = loadData.rotate;
            Building building = buildObject.GetComponent<Building>();

            building.LoadData(loadData);

            BuildManager.Instance.AddBuilding(building);

            BuildManager.Instance.MarkOccupied(loadData.gridX, loadData.gridZ, buildingData.Size, true);
        }
        #endregion

        #region 리소스
        ResourceManager.Instance.LoadData(data.resources);
        #endregion

        Debug.Log("Load Compelete");
    }
}
