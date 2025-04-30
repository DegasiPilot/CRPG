using UnityEngine;

namespace CRPG
{
    static class PhysicHelper
    {
		public static Vector3 JumpToPosition(Vector3 startPos, Vector3 targetPosition, float jumpHeigth, float progress)
		{
			// Параболическая траектория (y = -4x^2 + 4x)
			float verticalOffset = jumpHeigth * (-4 * Mathf.Pow(progress, 2) + 4 * progress);
			return Vector3.Lerp(startPos, targetPosition, progress) + Vector3.up * verticalOffset;
		}

		public static float CalculateJumpDuration(float height)
		{
			// Время до вершины прыжка: t_up = √(2h/g)
			// Общее время прыжка (вверх + вниз): t_total = 2*t_up
			return 2 * Mathf.Sqrt(2 * height / Mathf.Abs(Physics.gravity.y));
		}
	}
}
