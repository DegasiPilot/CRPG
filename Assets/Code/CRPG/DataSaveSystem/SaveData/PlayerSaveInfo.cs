using UnityEngine;

namespace CRPG.DataSaveSystem.SaveData
{
    public class PlayerSaveInfo
    {
		public string UniqueName;
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }

        public PersonageSaveInfo PersonageSaveInfo { get; set; }
		public string[] EquipedItems { get; set; }
	}
}
