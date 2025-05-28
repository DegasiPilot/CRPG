using DegasiPilot.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BattleSystem.Views
{
	internal class AttackPanelView : MonoBehaviour
	{
		[SerializeField] private SliderView AttackSliderView;
		[SerializeField] private SliderView DefendSliderView;
		[SerializeField] private TextMeshProUGUI CoefficientText;
		[SerializeField] private TextMeshProUGUI AttackResultText;
		[SerializeField] private TextMeshProUGUI DodgeResultText;
		[SerializeField] private Button AttackButton;
		[SerializeField] private Button SkipButton;

		public UnityEvent AttackClick => AttackButton.onClick;
		public UnityEvent SkipClick => SkipButton.onClick;
		public UnityEvent<float> AttackForceChanged;
		public UnityEvent<float> DefendForceChanged;

		private bool _canSkip;

		private void Awake()
		{
			AttackSliderView.OnValueChanged.AddListener(AttackChanged);
			DefendSliderView.OnValueChanged.AddListener(DefendChanged);
		}

		public void RefreshAttackResult(float result)
		{
			AttackResultText.text = result.ToString("0.##");
		}

		public void RefreshDodgeResult(float result)
		{
			DodgeResultText.text = result * 100 + "%";
		}

		public void UpdateAttackBtnInteractable(bool isInteractable)
		{
			AttackButton.interactable = isInteractable;
		}

		public void Refresh(float minAttackEnergy, float maxAttackEnergy, float minDefend, float maxDefend, bool canSkip, bool canAttack, bool needDefend, float coefficient)
		{
			CoefficientText.text = "x" + coefficient;
			_canSkip = canSkip;
			AttackSliderView.SetActive(canAttack);
			if (canAttack)
			{
				AttackSliderView.Refresh(minAttackEnergy, maxAttackEnergy, canSkip);
			}
			DefendSliderView.SetActive(needDefend);
			if (needDefend)
			{
				DefendSliderView.Refresh(minDefend, maxDefend);
			}
			SkipButton.gameObject.SetActive(canSkip);
		}

		public void RefreshAttack(float energy)
		{
			if (energy == 0) energy = AttackSliderView.MinValue;
			AttackSliderView.Refresh(energy);
		}

		public void RefreshDefend(float energy)
		{
			if (energy == 0) energy = DefendSliderView.MinValue;
			DefendSliderView.Refresh(energy);
		}

		private void AttackChanged(float energy)
		{
			if (_canSkip && energy == AttackSliderView.MinValue)
			{
				energy = 0;
			}
			AttackForceChanged.Invoke(energy);
		}

		private void DefendChanged(float energy)
		{
			DefendForceChanged.Invoke(energy);
		}
	}
}