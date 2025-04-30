using CRPG.DataSaveSystem;
using CRPG.ItemSystem;
using System;
using System.Collections;
using UnityEngine;

namespace CRPG.Battle
{
    class AttackModule : IDisposable
    {
        public AttackModule(PersonageController personageController, ChooseAttackForceModule chooseAttackForceModule)
        {
			_personageController = personageController;
            _chooser = chooseAttackForceModule;
			_chooser.OnChooseAttackForce.AddListener(Attack);
		}

		private bool _isAttacking;
		private bool _isAttackAnim;
		public bool IsAttacking => _isAttackAnim || _isAttacking;

        private ChooseAttackForceModule _chooser;
		private PersonageController _personageController;
		private PersonageController _enemy;
		private float _energy;

		public void Attack(PersonageController enemy)
		{
			var personage = _personageController.Personage;
			if (personage.Weapon != null && personage.Weapon.RequiredProjectile != null)
			{
				if(personage.Projectiles != null &&
					personage.Projectiles[0].ItemInfo == personage.Weapon.RequiredProjectile)
				{
					ProjectileItem projectile = personage.Projectiles[personage.Projectiles.Count - 1];
					_personageController.DropItem(projectile);
				}
				else
				{
					return;
				}
			}
			_enemy = enemy;
			_isAttacking = true;
			_chooser.ChooseAttackForce(_personageController.Personage);
		}

		private void Attack(float energy)
		{
			_personageController.StartCoroutine(AttackCoroutine(energy));
		}

		public void MakeDamage()
		{
			_enemy.GetDamage(_energy, _personageController.Personage.Weapon?.WeaponInfo.DamageType ?? GameData.UnarmedDamageType);
			_isAttackAnim = false;
			_personageController.Personage.RemoveStamina(_energy);
			BattleManager.AfterAttack(this);
			if (IsAttacking) _chooser.ChooseAttackForce(_personageController.Personage);

			_personageController.AnimatorManager.OnContactEnemy.RemoveListener(MakeDamage);
		}

		private IEnumerator AttackCoroutine(float energy)
		{
			_energy = energy;

			if (_isAttackAnim)
			{
				yield return new WaitWhile(() => _isAttackAnim);
			}
			_isAttackAnim = true;

			_personageController.AnimatorManager.OnContactEnemy.AddListener(MakeDamage);

			yield return _personageController.RotateTo(_enemy.Personage.HitPoint);

			bool isArmed = _personageController.Personage.Weapon != null;
			_personageController.AnimatorManager.StartAttackAnim(
					_enemy.Personage.HitPoint,
					isArmed,
					isArmed && _personageController.Personage.Weapon.WeaponInfo.MaxAttackDistance < 3f,
					_personageController.Personage.Weapon?.WeaponAnimationManager);
		}

		public void EndAttack()
		{
			_isAttacking = false;
			Debug.Log("End attack");
		}

		public void Dispose()
		{
			if(_chooser != null)
			{
				_chooser.OnChooseAttackForce.RemoveListener(Attack);
			}
			if(_personageController != null && _personageController.AnimatorManager != null)
			{
				_personageController.AnimatorManager.OnContactEnemy.RemoveListener(MakeDamage);
			}
		}
	}
}