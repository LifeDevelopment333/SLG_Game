using SLG.Builder;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    [SerializeField] private BuildPopup buildPopup;
    [SerializeField] private List<Button> buildingSelectButtons = new List<Button>();
    [SerializeField] private Button PrevButton;
    [SerializeField] private Button NextButton;
    [SerializeField] private RectTransform ProgressbarRect;

    private List<string> buildingNames;
    private List<Sprite> buildingIcons;
    private int page = 0;

    readonly private int itemsPerPage = 5;
    readonly private float progressBarMaxWidth = 707f;

    void Start()
    {
        buildingNames = LoadBuildingData.Instance.GetAllBuildingNames();
        buildingIcons = LoadBuildingData.Instance.GetAllBuildingIcons();

        for (int i = 0; i < buildingSelectButtons.Count; i++)
        {
            int index = i;
            buildingSelectButtons[i].onClick.AddListener(() => SeletectBuildingMenu(index));
        }

        PrevButton.onClick.AddListener(OnClickPrevPageButton);
        NextButton.onClick.AddListener(OnClickNextPageButton);

        UpdateBuildingMenu();
    }

    private void OnClickPrevPageButton()
    {
        if(page > 0)
        {
            page--;
            UpdateBuildingMenu();
        }
    }

    private void OnClickNextPageButton()
    {
        int currentPage = page + 1;
        if((currentPage + 1) * itemsPerPage < buildingNames.Count)
        {
            page++;
            UpdateBuildingMenu();
        }
        else if(currentPage * itemsPerPage < buildingNames.Count)
        {
            page++;
            UpdateBuildingMenu();
        }
    }

    private void UpdateBuildingMenu()
    {
        for (int i = 0; i < buildingSelectButtons.Count; i++)
        {
            int buildingIndex = page * itemsPerPage + i;

            if (buildingIndex < buildingNames.Count)
            {
                buildingSelectButtons[i].gameObject.SetActive(true);
                buildingSelectButtons[i].GetComponentInChildren<TMP_Text>().text = buildingNames[buildingIndex];
                buildingSelectButtons[i].GetComponent<Image>().sprite = buildingIcons[buildingIndex];
            }
            else
            {
                buildingSelectButtons[i].gameObject.SetActive(false);
            }
        }

        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        float progress = (float)(page + 1) * itemsPerPage / buildingNames.Count;
        float newWidth = Mathf.Min(progress * progressBarMaxWidth, progressBarMaxWidth);
        ProgressbarRect.sizeDelta = new Vector2(newWidth, ProgressbarRect.sizeDelta.y);
    }

    // 빌딩 아이콘 클릭 시
    private void SeletectBuildingMenu(int index)
    {
        int selectedIndex = page * itemsPerPage + index;

        // 빌딩 선택
        BuildingData data = LoadBuildingData.Instance.GetBuildingDataByName(buildingNames[selectedIndex]);
        BuildManager.Instance.SelectBuilding(data);

        buildPopup.OpenPopup(data);
    }
}
