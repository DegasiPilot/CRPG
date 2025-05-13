using DialogueSystem.DataContainers;
using DialogueSystem.Runtime.DialogueConditions;
using UnityEngine;

namespace DialogueSystem.Runtime.DialogueVariants
{
	public class DialogueVariant : MonoBehaviour
	{
		[field: SerializeField] public DialogueContainer DialogueContainer { get; protected set; }
		[SerializeField] private DialogueCondition _condition;
		public bool CheckCondition(DialogueActor me)
		{
			return _condition.Check(me);
		}
	}
}