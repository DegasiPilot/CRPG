using DegasiPilot.UniPooling;
using TMPro;
using UnityEngine;

namespace BattleSystem.Views
{
	internal class PooledNumbersRowView : MonoBehaviour
	{
		[SerializeField] protected TextMeshProUGUI TextPrefab;
		private SmartPool<TextMeshProUGUI> _textSource;
		private bool IsInitialized => _textSource != null;

		/// <summary>
		/// Prewent redraw
		/// </summary>
		private float _startValue = float.NegativeInfinity;
		private float _stepsCount = float.NegativeInfinity;
		private float _step = float.NegativeInfinity;

		public void DisplayNumbersRow(float startValue, int stepsCount, float step)
		{
			if(_startValue == startValue && _stepsCount == stepsCount && _step == step)
			{
				return; //Prewent redraw
			}
			else
			{
				_startValue = startValue; _stepsCount = stepsCount; _step = step;
			}

			if (!IsInitialized)
			{
				Init(stepsCount + 1);
			}
			else
			{
				ReleaseUsedObjects();
			}

			for (int i = 0; i <= stepsCount; i++)
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
				float newX = (float)i/stepsCount;
				anchorMin.x = newX;
				anchorMax.x = newX;
				textMeshPro.rectTransform.anchorMin = anchorMin;
				textMeshPro.rectTransform.anchorMax = anchorMax;

				textMeshPro.text = (startValue + step * i).ToString();
			}
		}

		private void Init(int capacity)
		{
			_textSource = new SmartPool<TextMeshProUGUI>(TextPrefab, capacity);
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