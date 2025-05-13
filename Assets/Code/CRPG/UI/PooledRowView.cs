using DegasiPilot.UniPooling;
using TMPro;
using UnityEngine;

namespace BattleSystem.Views
{
	internal class PooledRowView : MonoBehaviour
	{
		[SerializeField] protected TextMeshProUGUI TextPrefab;
		private SmartPool<TextMeshProUGUI> _textSource;
		private bool IsInitialized => _textSource != null;

		public void DisplayRow(string[] row)
		{
			if (!IsInitialized)
			{
				Init(row.Length);
			}
			else
			{
				ReleaseUsedObjects();
			}

			for (int i = 0; i < row.Length; i++)
			{
				var textMeshPro = GetText();
				textMeshPro.transform.SetParent(transform, false);

				Vector2 pivot = textMeshPro.rectTransform.pivot;
				pivot.y = 1f;
				textMeshPro.rectTransform.pivot = pivot;

				//Vector2 position = textMeshPro.rectTransform.localPosition;
				//position.y = 0;
				//textMeshPro.rectTransform.localPosition = position;

				Vector2 anchorMin = textMeshPro.rectTransform.anchorMin;
				Vector2 anchorMax = textMeshPro.rectTransform.anchorMax;
				float newX = (float)i / (row.Length - 1);
				anchorMin.x = newX;
				anchorMax.x = newX;
				textMeshPro.rectTransform.anchorMin = anchorMin;
				textMeshPro.rectTransform.anchorMax = anchorMax;

				textMeshPro.text = row[i];
			}
		}

		private void Init(int capacity)
		{
			_textSource = new SmartComponentPool<TextMeshProUGUI>(TextPrefab, capacity);
		}

		protected TextMeshProUGUI GetText()
		{
			return _textSource.Get();
		}

		private void ReleaseUsedObjects()
		{
			_textSource?.Dispose();
		}

		private void OnDestroy()
		{
			ReleaseUsedObjects();
		}
	}
}