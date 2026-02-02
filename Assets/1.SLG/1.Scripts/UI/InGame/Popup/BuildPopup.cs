using SLG.Builder;
using SLG.EnumTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildPopup : MonoBehaviour
{
    [SerializeField] private Button Exit;
    [SerializeField] private Image Icon;
    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Description;
    [SerializeField] private Button BuildButton;
    [SerializeField] private TMP_Text Warning;

    [Header("자원")]
    [SerializeField] private TMP_Text Tree;
    [SerializeField] private TMP_Text Stone;
    [SerializeField] private TMP_Text Ore;

    private void Start()
    {
        Exit.onClick.AddListener(ClosePopup);
        BuildButton.onClick.AddListener(OnClickBuildButton);
    }

    public void OpenPopup(BuildingData data)
    {
        gameObject.SetActive(true);
        UpdateUI(data);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    private void OnClickBuildButton()
    {
        if(BuildManager.Instance.ApplyPlaceBuilding())
        {
            ClosePopup();
        }
        else
        {
            Warning.text = "자원이 부족하여 건설할 수 없습니다.";
        }
    }

    private void UpdateUI(BuildingData data)
    {
        Icon.sprite = data.Icon;
        Name.text = data.BuildingName;
        Description.text = data.Description;

        Tree.text = data.GetBuildCost().TryGetValue(ResourceType.나무, out int treeCost) ? treeCost.ToString() : "0";
        Stone.text = data.GetBuildCost().TryGetValue(ResourceType.돌, out int stoneCost) ? stoneCost.ToString() : "0";
        Ore.text = data.GetBuildCost().TryGetValue(ResourceType.광석, out int oreCost) ? oreCost.ToString() : "0";

        Warning.text = "";
    }
}
