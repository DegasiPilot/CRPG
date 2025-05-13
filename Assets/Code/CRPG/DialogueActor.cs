using DialogueSystem.DataContainers;
using DialogueSystem.Runtime.DialogueVariants;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PersonageController))]
public class DialogueActor : MonoBehaviour
{
	public DialogueContainer Dialogue;
	[SerializeField] private List<DialogueVariant> _dialogueVariants;
	public float MaxDialogueDistance;
	public Transform PlaceForCamera;

	public PersonageInfo PersonageInfo => _personageController.Personage.PersonageInfo;

	public int DialogueInteractionsCount { get; private set; } = 0;

	private PersonageController _personageController;

	private void Awake()
	{
		_personageController = GetComponent<PersonageController>();
	}

	public void OnStartDialogue()
	{
		_personageController.AnimatorManager.StartDialogueAnim();
	}

	public void OnEndDialogue()
	{
		_personageController.AnimatorManager.EndDialogueAnim();
		DialogueInteractionsCount++;
		if (_dialogueVariants != null)
		{
			foreach (DialogueVariant dialogueVariant in _dialogueVariants)
			{
				if (dialogueVariant.CheckCondition(this))
				{
					Dialogue = dialogueVariant.DialogueContainer;
					break;
				}
			}
		}
	}
}