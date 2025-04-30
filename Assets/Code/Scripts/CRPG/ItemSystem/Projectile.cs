using System.Collections;
using UnityEngine;

namespace CRPG
{
    class Projectile : MonoBehaviour
    {
		[Tooltip("Базовый множитель высоты")]
		[SerializeField] float _distanceToHeightRatio = 0.4f; 
		[SerializeField] Vector3 _headOffset;
		[SerializeField] float _maxFireOffset;
		
		Transform _target;

		private Vector3 _headPosition
		{
			get => transform.position + _headOffset;
			set => transform.position = value - _headOffset;
		}

		public void Fire(Transform target, System.Action callback)
		{
			float distance = Vector3.Distance(_headPosition, target.position);
			float height = distance * _distanceToHeightRatio;
			_target = target;
			Vector3 randomOffset = Random.insideUnitCircle * _maxFireOffset;
			StartCoroutine(Fire(target.position + randomOffset, height, PhysicHelper.CalculateJumpDuration(height), callback));
		}

		private IEnumerator Fire(Vector3 targetPosition, float jumpHeigth, float jumpDuration, System.Action callback)
		{
			Vector3 startPos = _headPosition;
			float time = 0;

			while (time < jumpDuration)
			{
				time += Time.deltaTime;
				float progress = time / jumpDuration;

				Vector3 nextPosition = PhysicHelper.JumpToPosition(startPos, targetPosition, jumpHeigth, progress);
				transform.LookAt(nextPosition);
				_headPosition = nextPosition;

				yield return null;
			}

			// Фиксируем позицию после окончания
			_headPosition = targetPosition;
			if(_target != null && Vector3.Distance(_target.position, _headPosition) < 1f)
			{
				transform.SetParent(_target);
			}
			callback.Invoke();
		}
	}
}
