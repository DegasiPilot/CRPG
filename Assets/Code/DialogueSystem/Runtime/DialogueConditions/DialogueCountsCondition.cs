using UnityEngine;

namespace DialogueSystem.Runtime.DialogueConditions
{
	class DialogueCountsCondition : DialogueCondition
	{
		[SerializeField] private int _minDialoguesCount;

		public override bool Check(DialogueActor dialogueActor)
		{
			return dialogueActor.DialogueInteractionsCount >= _minDialoguesCount;
		}
	}
}
