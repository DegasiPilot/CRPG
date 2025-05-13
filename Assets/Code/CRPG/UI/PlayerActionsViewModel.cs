using System;

namespace CRPG.UI
{
	class PlayerActionsViewModel : IDisposable
	{
		public PlayerActionsViewModel(PersonageActionsView view)
		{
			_view = view;
		}

		private PersonageActionsView _view;
		private ActionButton[] _actionButtons;

		public event Action<ActionType, bool> OnTogglePlayerAction;

		public void Setup(ActionType[] personageActions, Func<ActionType, PersonageActionInfo> getActionInfo)
		{
			Clear();
			_actionButtons = new ActionButton[personageActions.Length];
			for (int i = 0; i < personageActions.Length; i++)
			{
				ActionButton actionButton = _view.InstantiateActionButton();
				actionButton.Setup(getActionInfo.Invoke(personageActions[i]));
				actionButton.OnToggle.AddListener(TogglePlayerAction);
				_actionButtons[i] = actionButton;
			}
		}

		public void Clear()
		{
			if (_actionButtons != null)
			{
				foreach (var action in _actionButtons)
				{
					action.OnToggle.RemoveListener(TogglePlayerAction);
					action.Dispose();
				}
			}
		}

		public void TogglePlayerAction(ActionType actionType, bool activate)
		{
			OnTogglePlayerAction.Invoke(actionType, activate);
		}

		public void SetActiveAction(ActionType actionType)
		{
			foreach (ActionButton actionButton in _actionButtons)
			{
				actionButton.SetWithoutNotify(actionButton.MyAction == actionType);
			}
		}

		public void DeactivateAllActions()
		{
			_view.DeactivateAllActions();
		}

		public void Dispose()
		{
			Clear();
		}
	}
}