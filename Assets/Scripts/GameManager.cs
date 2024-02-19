using System.Collections;
using System.Collections.Generic;
using DialogueSystem.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DialogueParser dialogueParser;
    public GameObject Player;
    public Personage SecondPersonage;
    public CameraController CameraController;
    public CanvasManager CanvasManager;

    [HideInInspector] public PlayerController PlayerController;
    
    private GameMode _gameMode;
    [HideInInspector] public Personage PlayerPersonage;

    void Start()
    {
        Instance = this;
        GameData.LoadLastGameSave();
        dialogueParser.Setup();
        PlayerPersonage = Player.GetComponent<Personage>();
        PlayerPersonage.PersonageInfo = GameData.PlayerPersonage;
        dialogueParser.SecondPersonageInfo = SecondPersonage.PersonageInfo;
        PlayerController = Player.GetComponent<PlayerController>();
        PlayerController.Setup();
        CameraController.Setup();
        CanvasManager.Setup();
    }

    public void ChangeGameMode(GameMode gameMode)
    {
        _gameMode = gameMode;
        switch (gameMode)
        {
            case GameMode.Dialogue:
                CameraController.enabled = false;
                break;
            case GameMode.Free:
                CameraController.enabled = true;
                CameraController.StandartView();
                break;
        }
    }

    public void OnDialogueActorPressed(DialogueActor dialogueActor)
    {
        PlayerController.InteractWith(dialogueActor.transform, dialogueActor.MaxDialogueDistance, StartDialogue, dialogueActor);
    }

    public void OnItemPressed(Item item)
    {
        PlayerController.InteractWith(item.transform, 1, ItemInteract, item);
    }

    public void StartDialogue(Transform focusObject, Component component)
    {
        DialogueParser.Instance.SetSecondDialogueActor(component as DialogueActor);
        CameraController.FocusOn(focusObject);
        DialogueParser.Instance.TryStartDialogue();
    }

    public void ItemInteract(Transform itemObject, Component component)
    {
        PlayerPersonage.PickupItem(itemObject.gameObject);
        itemObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (CanvasManager.ToggleInventory())
            {
                CameraController.enabled = false;
            }
            else
            {
                CameraController.enabled = true;
            }
        }
    }
}