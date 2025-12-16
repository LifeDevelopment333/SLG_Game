using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SLG.Builder.BuildManager;

namespace SLG.Builder
{
    public class BuildManager : MonoBehaviour
    {
        private static BuildManager instance;
        public static BuildManager Instance => instance;

        [Header("프리뷰 머터리얼")]
        [SerializeField] private Material previewMaterial;
        private Material originMaterial;

        [Header("지형 데이터 (ScriptableObject)")]
        [SerializeField] private GridData mapData;

        [Header("그리드 렌더러")]
        [SerializeField] private BuildGridRenderer buildGridRenderer;

        [Header("지형 레이어")]
        [SerializeField] private LayerMask terrainLayer;

        private Camera cam;
        private GameObject previewObject;
        private Renderer previewRenderer;

        private BuildingData selectBuilding;
        private bool isBuildMode = false;
        public bool IsBuildMode => isBuildMode;

        private Vector3 worldPos;
        private int curX;
        private int curZ;

        private int buildingRotate = 0;
        private Quaternion originalRotation;

        private bool canBuild;

        private List<PlacedBuilding> placedPreviewList = new List<PlacedBuilding>();

        public struct PlacedBuilding
        {
            public GameObject Object;
            public int x;
            public int z;
            public int size;
            public Material Material;
        }

        private void Awake()
        {
            instance = this;
            cam = Camera.main;
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (isBuildMode == false || selectBuilding == null)
                return;

            UpdatePreview();

            #region 키 맵핑
            KeyRotateBuilding();
            TryPlacedPreview();
            #endregion
        }

        private void KeyRotateBuilding()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                buildingRotate = (buildingRotate - 90) % 360;

            if (Input.GetKeyDown(KeyCode.E))
                buildingRotate = (buildingRotate + 90) % 360;
        }

        private void UpdatePreview()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, 500f, terrainLayer)) return;

            // 월드 → 그리드 변환
            if (!GridUtil.WorldToGrid(hit.point, out curX, out curZ, mapData))
                return;

            // 그리드 → 월드 위치 (Terrain 중심)
            worldPos = GridUtil.GridToWorld(curX, curZ, mapData);

            // 프리뷰 오브젝트 생성
            if (previewObject == null)
            {
                previewObject = Instantiate(selectBuilding.Prefab);
                previewRenderer = previewObject.GetComponentInChildren<Renderer>();
                originMaterial = previewRenderer.material;
                originalRotation = previewObject.transform.rotation;
            }

            Vector3 rot = originalRotation.eulerAngles;
            previewObject.transform.position = worldPos;
            previewObject.transform.rotation = Quaternion.Euler(rot.x, rot.y + buildingRotate, rot.z);

            // 건설 가능 여부 확인
            canBuild = PlacementChecker.CanBuild(curX, curZ, mapData, selectBuilding.Size, buildingRotate);

            ApplyPreviewMaterial(canBuild ? Color.green : Color.red, previewRenderer);

            // 그리드 보여주기
            buildGridRenderer.ShowPreviewGrid(curX, curZ, selectBuilding.Size, buildingRotate);
        }

        private void TryPlacedPreview()
        {
            if(Input.GetMouseButtonDown(0) && canBuild)
            {
                PlacedBuilding previewBuilding = PlaceBuildingPreview(worldPos);
                placedPreviewList.Add(previewBuilding);
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlaceBuilding();
            }

            if(Input.GetKeyDown(KeyCode.B))
            {
                ApplyPlaceBuilding();
            }
        }

        private PlacedBuilding PlaceBuildingPreview(Vector3 pos)
        {
            PlacedBuilding building = new PlacedBuilding();

            GameObject obj = selectBuilding.CreateBuilding(pos);
            Vector3 rot = originalRotation.eulerAngles;
            obj.transform.parent = transform;
            obj.transform.rotation = Quaternion.Euler(rot.x, rot.y + buildingRotate, rot.z);

            building.Object = obj;
            building.x = curX;
            building.z = curZ;
            building.size = selectBuilding.Size;

            // 프리뷰 머터리얼 적용
            Renderer renderer = obj.GetComponentInChildren<Renderer>();
            ApplyPreviewMaterial(Color.white, renderer);

            // 점유 처리
            MarkOccupied(curX, curZ, selectBuilding.Size, true);

            // 건설 후 그리드 비활성화
            buildGridRenderer.HidePreviewGrid();

            return building;
        }

        private void ApplyPlaceBuilding()
        {
            for(int i = 0; i < placedPreviewList.Count; i++)
            {
                PlacedBuilding placedBuilding = placedPreviewList[i];
                Renderer renderer = placedBuilding.Object.GetComponentInChildren<Renderer>();
                ApplyOriginMaterial(renderer);
            }

            placedPreviewList.Clear();
        }

        private void CancelPlaceBuilding()
        {
            for(int i = 0; i < placedPreviewList.Count; i++)
            {
                PlacedBuilding placedBuilding = placedPreviewList[i];

                MarkOccupied(placedBuilding.x, placedBuilding.z, placedBuilding.size, false);
                Destroy(placedBuilding.Object);
            }

            placedPreviewList.Clear();
        }

        // 점유처리
        private void MarkOccupied(int x, int z, int size, bool isOccupied)
        {
            int startX = GridUtil.GetStartX(x, size);
            int startZ = GridUtil.GetStartZ(z, size);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int px = startX + i;
                    int pz = startZ + j;

                    int index = mapData.Index(px, pz);
                    GridCell cell = mapData.GetCell(px,pz);
                    cell.isOccupied = isOccupied;
                    mapData.Cells[index] = cell;
                }
            }
        }

        private void ApplyPreviewMaterial(Color color, Renderer renderer)
        {
            if (renderer == null) return;

            renderer.material = previewMaterial;
            renderer.material.SetColor("_TintColor", color);
            renderer.material.SetFloat("_Alpha", 0.5f);
        }

        private void ApplyOriginMaterial(Renderer renderer)
        {
            if (renderer == null || originMaterial == null) return;
            renderer.material = originMaterial;
        }

        #region UI 관련 기능들
        public void SelectBuilding(BuildingData data)
        {
            selectBuilding = data;
            isBuildMode = true;

            if (previewObject != null)
                Destroy(previewObject);

            // 건물선택 시 그리드 비활성화
            buildGridRenderer.HidePreviewGrid();

            previewObject = null;

            if(placedPreviewList.Count > 0)
            {
                CancelPlaceBuilding();
            }
        }

        /// <summary>
        /// 빌드모드 바꾸기
        /// </summary>
        /// <param name="mode"> true = 빌드 모드 | false = 디스플레이 모드 </param>
        public void ChangeMode(bool mode)
        {
            isBuildMode = mode;

            if(IsBuildMode == false)
            {
                if (previewObject != null)
                    Destroy(previewObject);
                // 빌드모드 해제 시 그리드 비활성화
                buildGridRenderer.HidePreviewGrid();
                previewObject = null;
            }
        }

        public void RemoveBuilding(Building building)
        {
            GridUtil.WorldToGrid(building.transform.position, out int x, out int z, mapData);

            MarkOccupied(x, z, building.Data.Size, false);

            Destroy(building.gameObject);
        }
        #endregion
    }
}
