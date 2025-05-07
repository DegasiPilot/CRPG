using BattleSystem.Views;
using UnityEngine;

namespace DegasiPilot.UIExtensions
{
	internal class LabeledSliderView : SliderView
	{
		[SerializeField] private PooledRowView NumersView;

		public override void Refresh(float minValue, float maxValue, bool addNumberToStart = false, float addedNumber = 0)
		{
			base.Refresh(minValue, maxValue, addNumberToStart, addedNumber);
			int steps = Mathf.RoundToInt(maxValue - minValue) + 1;
			if (addNumberToStart) steps++;
			string[] row = new string[steps];

			if (addNumberToStart)
			{
				row[0] = addedNumber.ToString();
			}
			else
			{
				row[0] = minValue.ToString();
			}

			for (int i = 1; i < row.Length; i++)
			{
				float value = minValue + i;
				if (addNumberToStart) value--;
				row[i] = value.ToString();
			}
			NumersView.DisplayRow(row);
		}
	}
}