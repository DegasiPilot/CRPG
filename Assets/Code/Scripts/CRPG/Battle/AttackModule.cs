using CRPG.DataSaveSystem;
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
			var equipmentManager = _personageController.Personage.EquipmentManager;
			if (equipmentManager.IsWeaponNeedProjectiles)
			{
				if(equipmentManager.TryReloadWeapon())
				{

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
			_enemy.GetDamage(_energy, _personageController.Personage.DamageType);
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

			var equipmentManager = _personageController.Personage.EquipmentManager;
			bool isArmed = equipmentManager.Weapon != null;
			var weaponAnimManager = equipmentManager.Weapon != null ? equipmentManager.Weapon.WeaponAnimationManager : null;
			_personageController.AnimatorManager.StartAttackAnim(
					_enemy.Personage.HitPoint,
					isArmed,
					!_personageController.Personage.IsAttackRanged,
					weaponAnimManager);
		}

		public void EndAttack()
		{
			_isAttacking = false;
			AfterAttack();
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

		private void AfterAttack()
		{
			if (GameManager.Instance.GameMode != GameMode.Battle)
			{
				if(_enemy.Personage.BattleTeam == BattleTeam.Neutrals)
				{
					BattleTeam battleTeam = BattleManager.GetOppostiteTeam(_personageController.Personage.BattleTeam);
					Debug.Log(_enemy.Personage.PersonageInfo.Name + " теперь в команде " + battleTeam);
					_enemy.Personage.BattleTeam = battleTeam;
				}
				PersonageController[] participants = new PersonageController[2 + GameData.Companions.Count];
				participants[0] = GameData.MainPlayer.PlayerController;
				participants[1] = _enemy;
				for (int i = 0; i < GameData.Companions.Count; i++)
				{
					participants[i + 2] = GameData.Companions[i];
				}
				BattleManager.StartBattle(participants);
			}
			else if (!BattleManager.ParticipantPersonages.Contains(_enemy))
			{
				BattleManager.JoinToBattle(_enemy);
			}
		}
	}
}