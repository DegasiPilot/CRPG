using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject Player;
    public GameObject Personages;

    [HideInInspector] public PlayerController PlayerController;
    [HideInInspector] public Personage PlayerPersonage;
    
    private GameMode _gameMode;
    private List<Personage> _personages;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }

    void Start()
    {
        SceneSaveLoadManager.Instance.LoadSceneFromSave(GameData.SceneSaveInfo);
        DialogueParser.Instance.Setup();
        PlayerPersonage = Player.GetComponent<Personage>();
        PlayerPersonage.Setup(GameData.PlayerPersonage);
        SetActivePersonage(PlayerPersonage);
        PlayerController = Player.GetComponent<PlayerController>();
        PlayerController.Setup();
        CameraController.Instance.Setup();
        CanvasManager.Instance.Setup();
        _personages = Personages.GetComponentsInChildren<Personage>().ToList();
        _personages.ForEach(p => p.Setup());
    }

    public void ChangeGameMode(GameMode gameMode)
    {
        _gameMode = gameMode;
        switch (gameMode)
        {
            case GameMode.Dialogue:
                CameraController.Instance.enabled = false;
                break;
            case GameMode.Free:
                CameraController.Instance.enabled = true;
                CameraController.Instance.StandartView();
                break;
        }
    }

    public void OnDialogueActorPressed(DialogueActor dialogueActor)
    {
        PlayerController.InteractWith(dialogueActor.gameObject, dialogueActor.MaxDialogueDistance, StartDialogue, dialogueActor);
    }

    public void OnItemPressed(Item item)
    {
        PlayerController.InteractWith(item.gameObject, 1, ItemInteract, item);
    }

    public void StartDialogue(GameObject focusObject, Component component)
    {
        DialogueParser.Instance.SetSecondDialogueActor(component as DialogueActor);
        CameraController.Instance.FocusOn(focusObject);
        DialogueParser.Instance.TryStartDialogue();
    }

    public void ItemInteract(GameObject itemObject, Component component)
    {
        PlayerController.PickupItem(component as Item);
    }

    public void CreateNewGameSave()
    {
        SceneSaveLoadManager.Instance.SaveScene();
        GameData.NewGameSave();
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInvenoty();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    
    public void ToggleInvenoty()
    {
        CanvasManager.Instance.ToggleInventory();
        if (CanvasManager.Instance.IsInventoryOpen)
        {
            CameraController.Instance.enabled = false;
        }
        else
        {
            CameraController.Instance.enabled = true;
        }
    }

    public void TogglePauseMenu()
    {
        CanvasManager.Instance.TogglePauseMenu();
        if (CanvasManager.Instance.IsPauseMenuOpen)
        {
            CameraController.Instance.enabled = false;
            Time.timeScale = 0;
        }
        else
        {
            CameraController.Instance.enabled = true;
            Time.timeScale = 1;
        }
    }

    public void SetActivePersonage(Personage personage)
    {
        PlayerPersonage = personage;
        CanvasManager.Instance.SetActivePersonage(personage);
    }
}