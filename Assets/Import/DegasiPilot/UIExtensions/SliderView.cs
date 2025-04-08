using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DegasiPilot.UIExtensions
{
	internal class SliderView : MonoBehaviour
	{
		[SerializeField] private Slider _attackSlider;

		public UnityEvent<float> OnValueChanged => _attackSlider.onValueChanged;

		public virtual void Refresh(float minValue, float maxValue)
		{
			_attackSlider.minValue = minValue;
			_attackSlider.maxValue = maxValue;
		}

		public virtual void Refresh(float value)
		{
			_attackSlider.SetValueWithoutNotify(value);
		}
	}
}