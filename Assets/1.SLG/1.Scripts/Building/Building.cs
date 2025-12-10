using SLG.Builder;
using System.Transactions;
using UnityEngine;

public class Building : MonoBehaviour, IBuilding
{
    private BuildingData data;
    public BuildingData Data => data;
    private int level = 1;

    // 선택 시 하이라이트 강조
    //private BuildingHighLight highlighter;   

    public void Initialize(BuildingData data)
    {
        this.data = data;
        transform.tag = "Building";
        gameObject.layer = LayerMask.NameToLayer("Building");
    }

    public void ReBuild()
    {

    }

    public void Remove()
    {

    }

    public void Select()
    {
        BuildingHighlighter.Instance.ShowSelect(this);
    }

    public void DeSelect()
    {
        BuildingHighlighter.Instance.HideSelect();
    }

    public void Upgrade()
    {

    }

    public void HoverEnter()
    {
        Debug.Log("Hover Enter Building: " + data.name);
        BuildingHighlighter.Instance.ShowHover(this);
    }

    public void HoverExit()
    {
        BuildingHighlighter.Instance.HideHover();
    }
}
