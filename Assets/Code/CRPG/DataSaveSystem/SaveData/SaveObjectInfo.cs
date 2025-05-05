using UnityEngine;

namespace CRPG.DataSaveSystem.SaveData
{
	public class SaveObjectInfo
	{
		public string Name;
		public Vector3 Pos;
		public Vector3 Rot;

		public bool IsActive;

		public object[] ComponentsInfo;
	}
}