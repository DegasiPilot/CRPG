using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DegasiPilot.UIExtensions
{
	internal class SliderView : MonoBehaviour
	{
		[SerializeField] private Slider _slider;
		public float MinValue => _slider.minValue;
		public float MaxValue => _slider.maxValue;

		//------------------Prewent redraw---------------------
		float _lastMinValue = float.NegativeInfinity;
		float _lastMaxValue = float.NegativeInfinity;
		bool _wasAddNumberToStart = false;
		float _lastAddedNumber = float.NegativeInfinity;
		//------------------------------------------------------

		public UnityEvent<float> OnValueChanged => _slider.onValueChanged;

		protected virtual void OnValidate()
		{
			if (_slider == null) TryGetComponent(out _slider);
		}

		public virtual void Refresh(float minValue, float maxValue, bool addNumberToStart = false, float addedNumber = 0)
		{
			if(_lastMinValue == minValue && _lastMaxValue == maxValue && _wasAddNumberToStart == addNumberToStart && _lastAddedNumber == addedNumber)
			{
				Debug.Log("Prewent redraw");
				return;
			}
			_slider.minValue = addNumberToStart ? minValue - 1 : minValue;
			_slider.maxValue = maxValue;
		}

		public virtual void Refresh(float value)
		{
			_slider.SetValueWithoutNotify(value);
		}

		public void SetActive(bool active)
		{
			gameObject.SetActive(active);
		}
	}
}