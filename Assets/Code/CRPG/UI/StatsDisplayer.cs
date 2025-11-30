using TMPro;
using UnityEngine;

namespace CRPG.UI
{
	public class StatsDisplayer : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI StrengthValueText;
		[SerializeField] private TextMeshProUGUI DexterityValueText;
		[SerializeField] private TextMeshProUGUI ConstitutionValueText;
		[SerializeField] private TextMeshProUGUI CharismaValueText;

		public void Setup(PersonageInfo personageInfo)
		{
			StrengthValueText.text = personageInfo.Strength.ToString();
			DexterityValueText.text = personageInfo.Dexterity.ToString();
			ConstitutionValueText.text = personageInfo.Constitution.ToString();
			CharismaValueText.text = personageInfo.Charisma.ToString();
		}
	}
}