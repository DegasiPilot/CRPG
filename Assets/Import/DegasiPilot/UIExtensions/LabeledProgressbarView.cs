using TMPro;
using UnityEngine;

namespace DegasiPilot.UIExtensions
{
	internal class LabeledProgressbarView : ProgressBar
	{
		[SerializeField] private TextMeshProUGUI Label;

		public override void Refresh(float value, float maxValue, bool IsPermanent = false)
		{
			Label.text = value.ToString() + '/' + maxValue.ToString();
			base.Refresh(value, maxValue, IsPermanent);
		}
	}
}