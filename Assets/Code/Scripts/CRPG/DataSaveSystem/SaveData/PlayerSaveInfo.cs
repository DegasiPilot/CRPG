using UnityEngine;

namespace CRPG.DataSaveSystem.SaveData
{
    internal struct PlayerSaveInfo
    {
        internal string UniqueName;
        internal PersonageInfo PersonageInfo { get; set; }
        internal Vector3 Position { get; set; }
        internal Quaternion Rotation { get; set; }

        internal PersonageSaveInfo PersonageSaveInfo { get; set; }
        internal string[] EquipedItems { get; set; }
	}
}
