using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.DataContainers;

[RequireComponent(typeof(PersonageController))]
public class DialogueActor : MonoBehaviour
{
    public DialogueContainer Dialogue;
    public float MaxDialogueDistance;
    public Transform PlaceForCamera;

    public PersonageInfo PersonageInfo => _personageController.Personage.PersonageInfo;

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
    }
}
