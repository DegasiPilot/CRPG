using UnityEngine;
using UnityEngine.Events;

namespace CRPG.Battle
{
	class NPCChooseAttackForceModule : ChooseAttackForceModule
	{
		[SerializeField] private UnityEvent<float> _onChooseAttackForce;
		public override UnityEvent<float> OnChooseAttackForce => _onChooseAttackForce;

		public override void ChooseAttackForce(Personage personage)
		{
			float force = Random.Range(personage.MinAttackEnergy, personage.MaxAttackEnergy);
			OnChooseAttackForce.Invoke(force);
		}
	}
}