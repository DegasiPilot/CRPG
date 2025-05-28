using UnityEngine;
using UnityEngine.Events;

namespace CRPG.Battle
{
	class NPCChooseAttackForceModule : ChooseAttackForceModule
	{
		[SerializeField] private UnityEvent<float, float> _onChooseAttackForce;
		public override UnityEvent<float, float> OnChooseAttackForce => _onChooseAttackForce;

		public override void ChooseAttackForce(Personage personage, bool canSkip, bool canAttack, bool needDefend, float coefficient)
		{
			float minAttack = canSkip || !canAttack ? 0 : personage.MinAttackEnergy;
			float force = !canAttack ? 0 : Mathf.Round(Random.Range(minAttack, Mathf.Min(personage.MaxAttackEnergy, personage.Stamina)));
			float dodge = Mathf.Round(needDefend ? Random.Range(0, Mathf.Min(personage.Stamina - force, 4)) : 0);
			OnChooseAttackForce.Invoke(force, dodge);
		}
	}
}