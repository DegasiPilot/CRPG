using BattleSystem.Views;
using UnityEngine;

namespace BattleSystem.ViewModels
{
	internal class AttackPanelViewModel
	{
		public AttackPanelViewModel(AttackPanelView view)
		{
			_view = view;
			_view.AttackForceChanged.AddListener(value => AttackEnergy = value);
			_view.DefendForceChanged.AddListener(value => DefendEnergy = value);
			_view.AttackClick.AddListener(Attack);
			_view.SkipClick.AddListener(Skip);
			Deactivate();
		}

		private AttackPanelView _view;
		private Personage _selfPlayer;

		public float MinAttackEnergy => _selfPlayer.MinAttackEnergy;
		public float MaxAttackEnergy => Mathf.Min(_selfPlayer.MaxAttackEnergy, _selfPlayer.Stamina);
		public float MinDefendEnergy => 0;
		public float MaxDefendEnergy => Mathf.Min(_selfPlayer.Stamina, Mathf.Ceil(1 / _selfPlayer.DodgeCoefficient));

		private bool _canSkip;
		private float _coefficient;

		private float _attackEnergy = float.MinValue;
		public float AttackEnergy
		{
			get => _attackEnergy;
			set
			{
				if  (value != _attackEnergy &&
					(value >= MinAttackEnergy) &&
					 value <= MaxAttackEnergy)
				{
					_attackEnergy = value;
				}
				else if (value < MinAttackEnergy && _canSkip)
				{
					_attackEnergy = 0;
				}
				else
				{
					return;
				}
				_view.RefreshAttack(AttackEnergy);
				_view.RefreshAttackResult(AttackEnergy * _coefficient);
				_view.UpdateAttackBtnInteractable(CanAttack);
			}
		}
		private float _defendEnergy = 0;
		public float DefendEnergy
		{
			get => _defendEnergy;
			set
			{
				if (value != _defendEnergy &&
					value >= MinDefendEnergy &&
					value <= MaxDefendEnergy)
				{
					_defendEnergy = value;
					_view.RefreshDefend(DefendEnergy);
					_view.RefreshDodgeResult(DefendEnergy * _selfPlayer.DodgeCoefficient);
					_view.UpdateAttackBtnInteractable(CanAttack);
				}
			}
		}

		public System.Action<float, float> OnSelectEnd;

		private void Attack()
		{
			if (_selfPlayer.Stamina >= AttackEnergy)
			{
				OnSelectEnd?.Invoke(AttackEnergy, DefendEnergy);
			}
		}

		private void Skip()
		{
			OnSelectEnd?.Invoke(0, 0);
		}

		private bool CanAttack => AttackEnergy + DefendEnergy > 0 && _selfPlayer.Stamina >= AttackEnergy + DefendEnergy;

		public void Activate(Personage player, bool canSkip, bool needDefend, float coefficient)
		{
			_canSkip = canSkip;
			_coefficient = coefficient;
			_selfPlayer = player;
			_view.Refresh(MinAttackEnergy, MaxAttackEnergy, MinDefendEnergy, MaxDefendEnergy, canSkip, needDefend, coefficient);
			AttackEnergy = MinAttackEnergy;
			DefendEnergy = 0;
			_view.gameObject.SetActive(true);
		}

		public void Deactivate()
		{
			_view.gameObject.SetActive(false);
			_selfPlayer = null;
		}
	}
}