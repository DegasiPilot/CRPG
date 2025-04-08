using UnityEngine;
using UnityEngine.UI;

namespace DegasiPilot.UIExtensions
{
	internal class ProgressBar : MonoBehaviour
	{
		[SerializeField] private Image _fillImage;
		[SerializeField] private float _animSpeed;

		private float _targerSize;
		private float Value 
		{
			get 
			{
				return _fillImage.rectTransform.anchorMax.x;
			}
			set
			{
				Vector2 newAnchor = _fillImage.rectTransform.anchorMax;
				newAnchor.x = value;
				_fillImage.rectTransform.anchorMax = newAnchor;
			}
		}

		private void Awake()
		{
			Debug.Assert(_animSpeed != 0, "ProgressBarView: anim speed is 0!", this);
			_targerSize = Value;
		}

		private void Update()
		{
			if (Mathf.Abs(Value - _targerSize) > 0.01f)
			{
				Value = Mathf.Lerp(Value, _targerSize, _animSpeed * Time.deltaTime);
				Debug.Log("ProgressBarView Update");
			}
		}

		public virtual void Refresh(float value, float maxValue, bool isPermanent = false)
		{
			_targerSize = CalcSize(value, maxValue);
			if (isPermanent)
			{
				Value = _targerSize;
			}
		}

		private float CalcSize(float value, float maxValue) => value / maxValue;

		public enum Direction
		{
			/// <summary>
			/// Starting position is the Left.
			/// </summary>
			LeftToRight,

			/// <summary>
			/// Starting position is the Right
			/// </summary>
			RightToLeft,

			/// <summary>
			/// Starting position is the Bottom.
			/// </summary>
			BottomToTop,

			/// <summary>
			/// Starting position is the Top.
			/// </summary>
			TopToBottom,
		}
	}
}