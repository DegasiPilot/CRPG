using TMPro;
using UnityEngine;

namespace DegasiPilot.UIExtensions
{
	internal class LabeledProgressbar : ProgressBar
	{
		[SerializeField] private string _prefix;
		[SerializeField] private TextMeshProUGUI Label;
		[TextArea]
		[SerializeField] private string _separator = "/";

#if UNITY_EDITOR
		[SerializeField] private string _previewString = "xx";
		[SerializeField] private bool _needPreview;

		private void OnValidate()
		{
			if (_needPreview)
			{
				Label.text = _prefix + _previewString + _separator + _previewString;
			}
		}
#endif

		public override void Refresh(float value, float maxValue, bool IsPermanent = false)
		{
			Label.text = _prefix + value.ToString() + _separator + maxValue.ToString();
			base.Refresh(value, maxValue, IsPermanent);
		}
	}
}