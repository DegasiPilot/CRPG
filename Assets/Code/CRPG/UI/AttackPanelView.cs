using DegasiPilot.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BattleSystem.Views
{
	internal class AttackPanelView : MonoBehaviour
	{
		public SliderView AttackSliderView;
		[SerializeField] private TextMeshProUGUI ResultText;
		[SerializeField] private Button AttackButton;

		public UnityEvent AttackClick => AttackButton.onClick;

		private void Awake()
		{
			gameObject.SetActive(false);
		}

		public void RefreshResult(float result)
		{
			ResultText.text = result.ToString();
		}

		public void UpdateAttackBtnInteractable(bool isInteractable)
		{
			AttackButton.interactable = isInteractable;
		}
	}
}
