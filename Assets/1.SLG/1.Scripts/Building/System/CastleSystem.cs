using SLG.SaveData;
using UnityEngine;

public class CastleSystem : MonoBehaviour, IBuildingSystem
{
    private Building building;
    private int influenceRange;

    public Building Building => building;
    public int InfluenceRange => influenceRange;

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
        if (building.IsConstruction == true) return false;

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

    public void Upgrade(int level)
    {
        switch(level)
        {
            case 1:
                influenceRange = 20;
                break;
            case 2:
                influenceRange = 50;
                break;
            case 3:
                influenceRange = 80;
                break;
        }
    }
}
