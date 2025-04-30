using TMPro;
using UnityEngine;

namespace DegasiPilot.UIExtensions
{
	internal class LabeledProgressbar : ProgressBar
	{
		[SerializeField] private string _prefix;
		[SerializeField] private TextMeshProUGUI Label;
		[SerializeField] private bool _needPreview;

		private void OnValidate()
		{
			if (_needPreview)
			{
				Label.text = _prefix + "x/x";
			}
		}

		public override void Refresh(float value, float maxValue, bool IsPermanent = false)
		{
			Label.text = _prefix + value.ToString() + '/' + maxValue.ToString();
			base.Refresh(value, maxValue, IsPermanent);
		}
	}
}