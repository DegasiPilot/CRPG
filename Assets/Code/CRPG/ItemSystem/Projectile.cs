using System.Collections;
using UnityEngine;

namespace CRPG
{
	class Projectile : MonoBehaviour
	{
		[Tooltip("Базовый множитель высоты")]
		[SerializeField] float _distanceToHeightRatio = 0.4f;
		[SerializeField] Transform _head;
		[SerializeField] float _maxFireOffset;

		Transform _target;

		private Vector3 _headPosition
		{
			set => transform.position += value - _head.position;
		}

		public void Fire(Transform target, System.Action callback)
		{
			float distance = Vector3.Distance(_head.position, target.position);
			float height = distance * _distanceToHeightRatio;
			_target = target;
			Vector3 randomOffset = Random.insideUnitCircle * _maxFireOffset;
			StartCoroutine(Fire(target.position + randomOffset, height, PhysicHelper.CalculateJumpDuration(height), callback));
		}

		private IEnumerator Fire(Vector3 targetPosition, float jumpHeigth, float jumpDuration, System.Action callback)
		{
			Vector3 startPos = _head.position;
			float time = 0;

			while (time < jumpDuration)
			{
				time += Time.deltaTime;
				float progress = time / jumpDuration;

				Vector3 nextPosition = PhysicHelper.JumpToPosition(startPos, targetPosition, jumpHeigth, progress);
				transform.rotation = Quaternion.LookRotation(nextPosition - _head.position);
				_headPosition = nextPosition;

				yield return null;
			}

			// Фиксируем позицию после окончания
			_headPosition = targetPosition;
			if (_target != null && Vector3.Distance(_target.position, _head.position) < 1f)
			{
				transform.SetParent(_target);
			}
			callback.Invoke();
		}
	}
}
