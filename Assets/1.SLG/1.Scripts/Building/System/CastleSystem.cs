using SLG.SaveData;
using UnityEngine;

public class CastleSystem : MonoBehaviour, IBuildingSystem, ISaveData<CastleSaveData>
{
    private int level;

    public void Run()
    {
        throw new System.NotImplementedException();
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
