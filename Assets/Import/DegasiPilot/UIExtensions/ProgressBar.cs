using UnityEngine;
using UnityEngine.UI;

namespace DegasiPilot.UIExtensions
{
	internal class ProgressBar : MonoBehaviour
	{
		[SerializeField] private Image _fillImage;
		private const float _animSpeed = 0.5f;
		[SerializeField] protected Direction _direction;

		private float _targerSize;
		private float Value
		{
			get
			{
				if (_fillImage.type != Image.Type.Filled)
				{
					return _fillImage.rectTransform.anchorMax.x;
				}
				else
				{
					return _fillImage.fillAmount;
				}
			}
			set
			{
				if (_fillImage.type != Image.Type.Filled)
				{
					Vector2 newAnchor = _fillImage.rectTransform.anchorMax;
					if (_direction == Direction.Horizontal)
					{
						newAnchor.x = value;
					}
					else if (_direction == Direction.Vertical)
					{
						newAnchor.y = value;
					}
					else
					{
						Debug.LogError("Unhandled direction", this);
					}
					_fillImage.rectTransform.anchorMax = newAnchor;
				}
				else
				{
					_fillImage.fillAmount = value;
				}
			}
		}

		private void Awake()
		{
			Debug.Assert(_animSpeed != 0, "ProgressBarView: anim speed is 0!", this);
			_targerSize = Value;
		}

		private void Update()
		{
			if(Value != _targerSize)
			{
				float difference = _targerSize - Value;
				if(Mathf.Abs(difference) <= _animSpeed * Time.deltaTime)
				{
					Value = _targerSize;
				}
				else
				{
					Value += _animSpeed * Time.deltaTime * Mathf.Sign(difference);
					//Debug.Log("Update");
				}
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
			Horizontal,
			Vertical,
		}
	}
}