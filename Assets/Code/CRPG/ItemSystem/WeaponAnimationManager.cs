using UnityEngine;

namespace CRPG.ItemSystem
{
    class WeaponAnimationManager : MonoBehaviour
    {
		public Transform Target { get; set; }
		public ItemSkin RightHand { get; set; }

		[Tooltip("Место за которое держат тетиву")]
        [SerializeField] private Transform _bowKeyTransform;

		private Vector3 _defaultLocalPosition;
		private bool _isAiming;
		public ProjectileItem ActiveProjectile { get; set; }

		public void SpawnArrow()
		{
			RightHand.SetSkin(ActiveProjectile);
		}

		public void AttackAnim()
		{
			_defaultLocalPosition = _bowKeyTransform.localPosition;
			_isAiming = true;
		}

		private void LateUpdate()
		{
			if (RightHand != null && _isAiming)
			{
				_bowKeyTransform.position = RightHand.transform.position;
			}
		}

		public void EndAttackAnim(System.Action callback)
		{
			_bowKeyTransform.localPosition = _defaultLocalPosition;
			ActiveProjectile.transform.SetParent(null);
			ActiveProjectile.Projectile.Fire(Target, callback);
			RightHand = null;
			ActiveProjectile = null;
			_isAiming = false;
		}
	}
}