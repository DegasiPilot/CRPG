using BattleSystem.Views;
using System;

namespace BattleSystem.ViewModels
{
	internal class AttackPanelViewModel
	{
		public AttackPanelViewModel(AttackPanelView view)
		{
			_view = view;
			_view.AttackSliderView.OnValueChanged.AddListener(value => AttackEnergy = value);
			_view.AttackClick.AddListener(Attack);
			Deactivate();
		}

		private AttackPanelView _view;
		private Personage _selfPlayer;

		public float MinAttackEnergy => _selfPlayer.MinAttackEnergy;
		public float MaxAttackEnergy => _selfPlayer.MaxAttackEnergy;

		private float _attackEnergy = float.MinValue;
		public float AttackEnergy
		{
			get => _attackEnergy;
			set
			{
				if (value != _attackEnergy && value >= MinAttackEnergy && value <= MaxAttackEnergy)
				{
					_attackEnergy = value;
					_view.AttackSliderView.Refresh(AttackEnergy);
					_view.RefreshResult(AttackEnergy);
					_view.UpdateAttackBtnInteractable(CanAttack);
				}
			}
		}

		private void Attack()
		{
			if (_selfPlayer.Stamina >= AttackEnergy)
			{
				OnAttack?.Invoke(AttackEnergy);
			}
		}
		public Action<float> OnAttack;

		private bool CanAttack => _selfPlayer.Stamina >= AttackEnergy;

		public void Activate(Personage player)
		{
			_selfPlayer = player;
			_view.AttackSliderView.Refresh(MinAttackEnergy, MaxAttackEnergy);
			AttackEnergy = MinAttackEnergy;
			_view.gameObject.SetActive(true);
		}

		public void Deactivate()
		{
			_view.gameObject.SetActive(false);
			_selfPlayer = null;
		}
	}
}