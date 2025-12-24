using UnityEngine;

public interface IBuildingSystem
{
    void Initialize(Building building);
    void Run();
    void Upgrade();
}
