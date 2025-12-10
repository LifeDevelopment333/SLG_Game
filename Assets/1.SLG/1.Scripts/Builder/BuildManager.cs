using UnityEngine;
using UnityEngine.EventSystems;

namespace SLG.Builder
{
    public class BuildManager : MonoBehaviour
    {
        private static BuildManager instance;
        public static BuildManager Instance => instance;

        [Header("프리뷰 머터리얼")]
        [SerializeField] private Material previewMaterial;

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

        private int curX;
        private int curZ;

        private int buildingRotate = 0;
        private Quaternion originalRotation;

        private void Awake()
        {
            instance = this;
            cam = Camera.main;
        }

        private void Update()
        {
            if (isBuildMode == false || selectBuilding == null)
                return;

            #region 키 맵핑
            // 회전 역방향
            if (Input.GetKeyDown(KeyCode.Q))
            {
                buildingRotate = (buildingRotate - 90) % 360;
            }

            // 회전 정방향
            if (Input.GetKeyDown(KeyCode.E))
            {
                buildingRotate = (buildingRotate + 90) % 360;
            }
            #endregion

            UpdatePreview();
        }

        private void UpdatePreview()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, 500f, terrainLayer)) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // 월드 → 그리드 변환
            if (!GridUtil.WorldToGrid(hit.point, out curX, out curZ, mapData))
                return;

            // 그리드 → 월드 위치 (Terrain 중심)
            Vector3 worldPos = GridUtil.GridToWorld(curX, curZ, mapData);

            // 프리뷰 오브젝트 생성
            if (previewObject == null)
            {
                previewObject = Instantiate(selectBuilding.Prefab);
                previewRenderer = previewObject.GetComponentInChildren<Renderer>();
                originalRotation = previewObject.transform.rotation;
            }

            Vector3 rot = originalRotation.eulerAngles;
            previewObject.transform.position = worldPos;
            previewObject.transform.rotation = Quaternion.Euler(rot.x, rot.y + buildingRotate, rot.z);

            // 건설 가능 여부 확인
            bool canBuild = PlacementChecker.CanBuild(curX, curZ, mapData, selectBuilding.Size, buildingRotate);

            ApplyPreviewMaterial(canBuild ? Color.green : Color.red);

            // 좌클릭 시 건설
            if (Input.GetMouseButtonDown(0) && canBuild)
            {
                PlaceBuilding(worldPos);
            }

            // 그리드 보여주기
            buildGridRenderer.ShowPreviewGrid(curX, curZ, selectBuilding.Size, buildingRotate);
        }

        private void PlaceBuilding(Vector3 pos)
        {
            GameObject obj = selectBuilding.CreateBuilding(pos);
            Vector3 rot = originalRotation.eulerAngles;
            obj.transform.parent = transform;
            obj.transform.rotation = Quaternion.Euler(rot.x, rot.y + buildingRotate, rot.z);

            // 점유 처리
            MarkOccupied(curX, curZ, selectBuilding.Size);

            // 건설 후 그리드 비활성화
            buildGridRenderer.HidePreviewGrid();
        }

        private void MarkOccupied(int x, int z, int size)
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
                    cell.isOccupied = true;
                    mapData.Cells[index] = cell;
                }
            }
        }

        private void ApplyPreviewMaterial(Color color)
        {
            if (previewRenderer == null) return;

            previewRenderer.material = previewMaterial;
            previewRenderer.material.SetColor("_TintColor", color);
            previewRenderer.material.SetFloat("_Alpha", 0.5f);
        }

        public void SelectBuilding(BuildingData data)
        {
            selectBuilding = data;
            isBuildMode = true;

            if (previewObject != null)
                Destroy(previewObject);

            // 건물선택 시 그리드 비활성화
            buildGridRenderer.HidePreviewGrid();

            previewObject = null;

            Debug.Log("Select Building: " + data.name);
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
    }
}
