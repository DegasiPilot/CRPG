using UnityEngine;
using UnityEngine.Events;

namespace CRPG.Battle
{
	class PlayerChooseAttackForceModule : ChooseAttackForceModule
	{
		[SerializeField] private UnityEvent<float> _onChooseAttackForce;
		public override UnityEvent<float> OnChooseAttackForce => _onChooseAttackForce;

		public override void ChooseAttackForce(Personage personage)
		{
			BattleUIManager.Instance.ActivatePlayerActionPanel(personage);
			BattleUIManager.Instance.AfterPlayerAttack.AddListener(AttackSelected);
		}

		private void AttackSelected(float force)
		{
			BattleUIManager.Instance.AfterPlayerAttack.RemoveListener(AttackSelected);
			BattleUIManager.Instance.DeactivatePlayerActionPanel();
			OnChooseAttackForce.Invoke(force);
		}

		private void OnDestroy()
		{
			BattleUIManager.Instance?.AfterPlayerAttack.RemoveListener(AttackSelected);
		}
	}
}
