using SLG.Builder;
using System.Collections.Generic;

namespace SLG.Builder
{
    public interface IBuildingDataLoader
    {
        List<BuildingData> LoadAll();
    }
}