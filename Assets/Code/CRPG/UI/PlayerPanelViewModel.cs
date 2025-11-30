using System;

namespace CRPG.UI
{
	class PlayerPanelViewModel : IDisposable
	{
		internal PlayerPanelViewModel(PlayerPanel view, Func<ActionType, PersonageActionInfo> getActionInfo)
		{
			_view = view;
			_actionsViewModel = new PlayerActionsViewModel(_view.PersonageActionsView);
			_actionsViewModel.OnTogglePlayerAction += TogglePlayerAction;
			_getActionInfo = getActionInfo;
		}

		private PlayerPanel _view;
		private PlayerController _personageController;
		private Personage _personage => _personageController?.Personage;
		private PlayerActionsViewModel _actionsViewModel;
		private Func<ActionType, PersonageActionInfo> _getActionInfo;

		private void UpdateHealthBar()
		{
			_view.UpdateHealthBar(_personage.Health, _personage.MaxHealth);
		}

		private void UpdateStaminaBar()
		{
			_view.UpdateStaminaBar(_personage.Stamina, _personage.MaxStamina);
		}

		private void UpdateRemainActions()
		{
			_view.UpdateActions(_personage.RemainActions, _personage.MaxActions);
		}

		public void SetActivePersonage(PlayerController personageController)
		{
			Personage personage = personageController.Personage;
			if (_personage != null)
			{
				_personage.OnHealthChanged.RemoveListener(UpdateHealthBar);
				_personage.OnStaminaChanged.RemoveListener(UpdateStaminaBar);
				_personage.OnRemainActionsChanged.RemoveListener(UpdateRemainActions);
				_personageController.OnSetAction.RemoveListener(_actionsViewModel.SetActiveAction);
			}
			_personageController = personageController;

			_view.PlayerName = personage.PersonageInfo.Name;
			UpdateHealthBar();
			UpdateStaminaBar();
			UpdateRemainActions();
			_view.PlayerPortrait = personage.PersonageInfo.PersonagePortrait;
			_personageController = personageController;
			personage.OnHealthChanged.AddListener(UpdateHealthBar);
			personage.OnStaminaChanged.AddListener(UpdateStaminaBar);
			_personage.OnRemainActionsChanged.AddListener(UpdateRemainActions);
			_personageController.OnSetAction.AddListener(_actionsViewModel.SetActiveAction);
			_actionsViewModel.Setup(personage.Actions, _getActionInfo);
		}

		public void TogglePlayerAction(ActionType actionType, bool activate)
		{
			if (activate)
			{
				_personageController.SetActiveAction(actionType);
			}
			else if (_personageController.ActiveAction == actionType)
			{
				_personageController.SetDefaultAction();
			}
		}

		public void OnChangeGameMode(GameMode lastGameMode, GameMode currentGameMode)
		{
			if (lastGameMode == GameMode.Dialogue)
			{
				_view.gameObject.SetActive(true);
			}
			else if (currentGameMode == GameMode.Dialogue)
			{
				_view.gameObject.SetActive(false);
			}
		}

		public void Dispose()
		{
			_actionsViewModel.OnTogglePlayerAction -= TogglePlayerAction;
		}
	}
}