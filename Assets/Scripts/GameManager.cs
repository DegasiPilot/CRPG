using System.Collections;
using System.Collections.Generic;
using DialogueSystem.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DialogueParser dialogueParser;
    public Personage PlayerPersonage;
    public Personage SecondPersonage;

    void Start()
    {
        Instance = this;
        dialogueParser.Setup();
        dialogueParser.PlayerPersonage = PlayerPersonage;
        dialogueParser.SecondPersonage = SecondPersonage;
        dialogueParser.StartDialogue();
    }
}
