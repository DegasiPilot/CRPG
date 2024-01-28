using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.DataContainers;

[RequireComponent(typeof(Personage))]
public class DialogueActor : MonoBehaviour
{
    public DialogueContainer Dialogue;
    public Personage Personage
    {
        get
        {
            if (personage == null)
                personage = GetComponent<Personage>();
            return personage;
        }
    }

    private Personage personage;
}
