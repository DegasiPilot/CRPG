using UnityEngine;
using UnityEngine.Events;

namespace CRPG.Battle
{
	abstract class ChooseAttackForceModule : MonoBehaviour
	{
		public abstract void ChooseAttackForce(Personage personage, bool canSkip, bool canAttack, bool needDefend, float coefficient);
		public abstract UnityEvent<float, float> OnChooseAttackForce { get; }
	}
}
