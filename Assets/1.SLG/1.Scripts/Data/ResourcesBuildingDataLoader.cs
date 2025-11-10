using System.Collections.Generic;
using UnityEngine;

namespace SLG.Builder
{
    public class ResourcesBuildingDataLoader : IBuildingDataLoader
    {
        [SerializeField] private string folderPath = "Building"; // Resources/Building

        public List<BuildingData> LoadAll()
        {
            var result = new List<BuildingData>();
            var loaded = Resources.LoadAll<BuildingData>(folderPath);

            if (loaded.Length == 0)
            {
                Debug.LogWarning($"[ResourcesBuildingDataLoader] '{folderPath}' 경로에서 BuildingData를 찾지 못했습니다.");
            }
            else
            {
                result.AddRange(loaded);
                Debug.Log($"[ResourcesBuildingDataLoader] {result.Count}개의 BuildingData 로드 완료");
            }

            return result;
        }
    }
}