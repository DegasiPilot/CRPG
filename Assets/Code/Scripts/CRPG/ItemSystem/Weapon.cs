using UnityEngine;

namespace CRPG.ItemSystem
{
	class Weapon : EquipableItem
	{
		public WeaponInfo WeaponInfo;
		public override ItemInfo ItemInfo => WeaponInfo;
		public WeaponAnimationManager WeaponAnimationManager;
		public Vector3 TargetingOffset;
		public ProjectileItemInfo RequiredProjectile;
	}
}
