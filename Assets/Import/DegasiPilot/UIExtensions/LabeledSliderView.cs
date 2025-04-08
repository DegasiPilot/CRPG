using BattleSystem.Views;
using TMPro;
using UnityEngine;

namespace DegasiPilot.UIExtensions
{
	internal class LabeledSliderView : SliderView
	{
		[SerializeField] private PooledNumbersRowView NumersView;

		public override void Refresh(float minValue, float maxValue)
		{
			NumersView.DisplayNumbersRow(minValue, (int)(maxValue - minValue), 1f);
			base.Refresh(minValue, maxValue);
		}
	}
}