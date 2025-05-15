using CRPG.DataSaveSystem;
using DialogueSystem.DataContainers;
using DialogueSystem.Runtime.DialogueVariants;
using System.Collections.Generic;
using UnityEngine;
using CRPG.DataSaveSystem.SaveData;

[RequireComponent(typeof(PersonageController))]
public class DialogueActor : MonoBehaviour, ISaveableComponent
{
	[SerializeField] private DialogueContainer _defaultDialogue;
	[SerializeField] private List<DialogueVariant> _dialogueVariants;

	public DialogueContainer Dialogue
	{
		get
		{
			if (_dialogueVariants != null)
			{
				foreach (DialogueVariant dialogueVariant in _dialogueVariants)
				{
					if (dialogueVariant.CheckCondition(this))
					{
						return dialogueVariant.DialogueContainer;
					}
				}
			}
			return _defaultDialogue;
		}
	}

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
	}

	public object Save()
	{
		return new DialogueActorSaveData() { DialogueInteractionsCount = this.DialogueInteractionsCount };
	}

	public void Load(IReadOnlyCollection<object> componentsData)
	{
		foreach(var componentData in componentsData)
		{
			if(componentData is DialogueActorSaveData data)
			{
				DialogueInteractionsCount = data.DialogueInteractionsCount;
				return;
			}
		}
	}
}