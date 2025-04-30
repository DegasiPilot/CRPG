using UnityEngine;

namespace CRPG.ItemSystem
{
	class ProjectileItem : EquipableItem
	{
		public ProjectileItemInfo ProjectileItemInfo;
		public override ItemInfo ItemInfo => ProjectileItemInfo;
		[SerializeField] private Projectile _projectile;
		public Projectile Projectile => _projectile;

		protected override void OnValidate()
		{
			base.OnValidate();
			if (_projectile == null) TryGetComponent(out _projectile);
		}

		public void Fire(Transform target, System.Action callback)
		{
			_projectile.Fire(target, callback);
		}
	}
}
