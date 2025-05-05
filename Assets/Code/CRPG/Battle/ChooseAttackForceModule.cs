using UnityEngine;
using UnityEngine.Events;

namespace CRPG.Battle
{
    abstract class ChooseAttackForceModule : MonoBehaviour
    {
		public abstract void ChooseAttackForce(Personage personage);
		public abstract UnityEvent<float> OnChooseAttackForce { get; }
	}
}
