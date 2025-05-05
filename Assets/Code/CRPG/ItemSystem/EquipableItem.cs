using UnityEngine;

namespace CRPG.ItemSystem
{
	public abstract class EquipableItem : Item
	{
		public bool IsEquiped { get; set; }
		public override bool IsBlockSave => base.IsBlockSave || IsEquiped;
		public Vector3 RotationInHand;
	}
}