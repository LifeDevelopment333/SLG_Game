using SLG.SaveData;
using UnityEngine;

public class CastleSystem : MonoBehaviour, IBuildingSystem, ISaveData<CastleSaveData>
{
    private Building building;

    private int level;

    public void Initialize(Building building)
    {
        this.building = building;

        AreaSystem.Instance.Register(this);
    }

    private void OnDestroy()
    {
        AreaSystem.Instance.Unregister(this);
    }

    public bool IsInArea(int x, int z)
    {
        return true;
    }

    public void Run()
    {
        Debug.Log("캐슬 작동중");
    }

    public void Upgrade()
    {
        throw new System.NotImplementedException();
    }

    public CastleSaveData SaveData()
    {
        throw new System.NotImplementedException();
    }

    public void LoadData(CastleSaveData data)
    {
        throw new System.NotImplementedException();
    }
}
