using System;

namespace CRPG.Interactions
{
	class ActionInteract : Interact
	{
		private Action _action;

		public ActionInteract(Action action)
		{
			_action = action;
		}

		public override void Execute(PersonageController executor)
		{
			_action.Invoke();
		}
	}
}