using SLG.SaveData;
using UnityEngine;

public class CastleSystem : MonoBehaviour, IBuildingSystem, ISaveData<CastleSaveData>
{
    private Building building;

    private int level;
    private int influenceRange;

    public void Initialize(Building building)
    {
        this.building = building;

        influenceRange = building.Data.InfluenceRange;

        AreaSystem.Instance.Register(this);
    }

    private void OnDestroy()
    {
        AreaSystem.Instance.Unregister(this);
    }

    public bool IsInArea(int x, int z)
    {
        int cx = building.PlacedBuilding.x;
        int cz = building.PlacedBuilding.z;

        int range = influenceRange;

        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                int gx = cx + i;
                int gz = cz + j;

                if (gx == x && gz == z)
                    return true;
            }
        }

        return false;
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
        CastleSaveData saveData = new CastleSaveData();

        saveData.level = level;

        return saveData;
    }

    public void LoadData(CastleSaveData data)
    {

    }
}
