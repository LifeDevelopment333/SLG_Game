using NUnit.Framework;
using SLG.Builder;
using System.Collections.Generic;
using UnityEngine;

namespace SLG.Builder
{
    public class LoadBuildingData : MonoBehaviour
    {
        private static LoadBuildingData instance;
        public static LoadBuildingData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<LoadBuildingData>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("LoadBuildingData");
                        instance = obj.AddComponent<LoadBuildingData>();
                    }
                }
                return instance;
            }
        }

        private List<BuildingData> dataList = new List<BuildingData>();
        public IReadOnlyList<BuildingData> DataList => dataList;
        private IBuildingDataLoader loader;

        private void Awake()
        {
            loader = new ResourcesBuildingDataLoader();
            dataList = loader.LoadAll();
        }

        public List<string> GetAllBuildingNames()
        {
            List<string> names = new List<string>();
            foreach (var data in dataList)
            {
                names.Add(data.name);
            }
            return names;
        }

        public List<Sprite> GetAllBuildingIcons()
        {
            List<Sprite> icons = new List<Sprite>();
            foreach (var data in dataList)
            {
                icons.Add(data.Icon);
            }
            return icons;
        }

        public BuildingData GetBuildingDataByName(string name)
        {
            foreach (var data in dataList)
            {
                if (data.name == name)
                {
                    return data;
                }
            }
            return null;
        }
    }
}