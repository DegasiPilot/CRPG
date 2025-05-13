using UnityEngine;

namespace DialogueSystem.Runtime.DialogueConditions
{
	abstract class DialogueCondition : MonoBehaviour
	{
		public abstract bool Check(DialogueActor dialogueActor);
	}
}