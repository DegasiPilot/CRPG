using System.Collections.Generic;

namespace CRPG.DataSaveSystem
{
    interface ISaveableComponent
    {
        object Save();
        void Load(IReadOnlyCollection<object> componentsData);
    }
}