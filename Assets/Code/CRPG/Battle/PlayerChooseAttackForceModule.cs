using UnityEngine;
using UnityEngine.Events;

namespace CRPG.Battle
{
	class PlayerChooseAttackForceModule : ChooseAttackForceModule
	{
		[SerializeField] private UnityEvent<float, float> _onChooseAttackForce;
		public override UnityEvent<float, float> OnChooseAttackForce => _onChooseAttackForce;

		public override void ChooseAttackForce(Personage personage, bool canSkip, bool canAttack, bool needDefend, float coefficient)
		{
			BattleUIManager.Instance.ActivatePlayerActionPanel(personage, canSkip, canAttack, needDefend, coefficient);
			BattleUIManager.Instance.AfterPlayerEnergySelection.AddListener(AttackSelected);
		}

		private void AttackSelected(float attack, float defence)
		{
			BattleUIManager.Instance.AfterPlayerEnergySelection.RemoveListener(AttackSelected);
			BattleUIManager.Instance.DeactivatePlayerActionPanel();
			OnChooseAttackForce.Invoke(attack, defence);
		}

		private void OnDestroy()
		{
			BattleUIManager.Instance?.AfterPlayerEnergySelection.RemoveListener(AttackSelected);
		}
	}
}
