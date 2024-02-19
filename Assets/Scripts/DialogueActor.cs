using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.DataContainers;

[RequireComponent(typeof(Personage))]
public class DialogueActor : MonoBehaviour
{
    public DialogueContainer Dialogue;
    public float MaxDialogueDistance;

    public PersonageInfo PersonageInfo
    {
        get
        {
            if (_personageInfo == null)
                _personageInfo = GetComponent<Personage>().PersonageInfo;
            return _personageInfo;
        }
    }

    private PersonageInfo _personageInfo;
}
