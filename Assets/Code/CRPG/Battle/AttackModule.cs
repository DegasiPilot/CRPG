using CRPG.DataSaveSystem;
using System.Collections;
using UnityEngine;

namespace CRPG.Battle
{
	class AttackModule : System.IDisposable
	{
		public AttackModule(PersonageController personageController, ChooseAttackForceModule chooseAttackForceModule)
		{
			_personageController = personageController;
			_chooser = chooseAttackForceModule;
			_chooser.OnChooseAttackForce.AddListener(AfterEnergyChoosed);
		}

		private bool _isAttacking;
		private bool _isAttackAnim;
		public bool IsAttacking => _isAttackAnim || _isAttacking;

		private ChooseAttackForceModule _chooser;
		private PersonageController _personageController;
		private PersonageController _enemy;
		private float _attack;
		private float _damageCoefficient;

		public void Attack(PersonageController enemy, bool canSkip, bool needDefend)
		{
			if (_personageController.Personage.Stamina <= 0) return;
			var equipmentManager = _personageController.Personage.EquipmentManager;
			if (equipmentManager.IsWeaponNeedProjectiles)
			{
				if (equipmentManager.TryReloadWeapon())
				{

				}
				else
				{
					return;
				}
			}
			_enemy = enemy;
			_isAttacking = true;
			_damageCoefficient = RandomizeDamageCoef();
			_chooser.ChooseAttackForce(_personageController.Personage, canSkip, needDefend, _damageCoefficient);
		}

		private void AfterEnergyChoosed(float attack, float defend)
		{
			_attack = attack;
			BattleManager.EnergyChoosed(_personageController, attack, defend);
		}

		public void MakeDamage()
		{
			_personageController.Personage.Stamina -= _attack;
			_enemy.GetDamage(_attack * _damageCoefficient, _personageController.Personage.DamageType);
			_isAttackAnim = false;

			EndAttack();
			_personageController.AnimatorManager.OnContactEnemy.RemoveListener(MakeDamage);
		}

		public void StartAttackCoroutine()
		{
			_personageController.StartCoroutine(AttackCoroutine());
		}

		private IEnumerator AttackCoroutine()
		{
			if (_isAttackAnim)
			{
				yield return new WaitWhile(() => _isAttackAnim);
			}
			_isAttackAnim = true;

			_personageController.AnimatorManager.OnContactEnemy.AddListener(MakeDamage);

			yield return _personageController.RotateTo(_enemy.Personage.HitPoint.position);

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
			_personageController.EndAttack();
			AfterAttack();
		}

		public void Dispose()
		{
			if (_chooser != null)
			{
				_chooser.OnChooseAttackForce.RemoveListener(AfterEnergyChoosed);
			}
			if (_personageController != null && _personageController.AnimatorManager != null)
			{
				_personageController.AnimatorManager.OnContactEnemy.RemoveListener(MakeDamage);
			}
		}

		private void AfterAttack()
		{
			if (_enemy.Personage.Health <= 0)
			{
				return;
			}
			if (GameManager.Instance.GameMode != GameMode.Battle)
			{
				if (_enemy.Personage.BattleTeam == BattleTeam.Neutrals)
				{
					BattleTeam battleTeam = BattleManager.GetOppostiteTeam(_personageController.Personage.BattleTeam);
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

		private float RandomizeDamageCoef()
		{
			float energy =
				_personageController.Personage.EquipmentManager.Weapon == null ?
				Random.Range(1f, 2f) :
				Random.Range(2f, 3f);
			return System.MathF.Round(energy, 1);
		}
	}
}