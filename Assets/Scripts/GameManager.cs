using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DialogueSystem.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject Player;
    public GameObject Personages;

    [HideInInspector] public PlayerController PlayerController;
    public List<PlayerController> PlayerControllers;
    public Personage PlayerPersonage => PlayerController.Personage;
    public PersonageController[] PersonageControllers;
    
    private GameMode _gameMode = GameMode.Free;
    public GameMode GameMode => _gameMode;
    private Component _currentComponentUnderPointer;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }

    void Start()
    {
        SceneSaveLoadManager.Instance.LoadSceneFromSave(GameData.SceneSaveInfo);
        DialogueParser.Instance.Setup();
        PlayerController = Player.GetComponent<PlayerController>();
        PlayerControllers = new() { PlayerController }; // TODO: More player controllers
        PersonageControllers = Personages.GetComponentsInChildren<PersonageController>();

        foreach(var controller in PersonageControllers)
        {
            controller.Setup();
        }

        PlayerPersonage.Setup(GameData.PlayerPersonage);
        SetActivePersonage(PlayerPersonage);
    }

    public void ChangeGameMode(GameMode gameMode)
    {
        if (_gameMode != gameMode)
        {
            CanvasManager.Instance.OnChangeGameMode(_gameMode, gameMode);
            if (_gameMode == GameMode.Dialogue)
            {
                CameraController.Instance.enabled = true;
            }
            
            if(gameMode == GameMode.Dialogue)
            {
                CameraController.Instance.enabled = false;
            }
            else if(gameMode == GameMode.Battle)
            {
                PlayerController.ForceStop();
            }
            _gameMode = gameMode;
        }
    }

    public void OnPersonageUnderPointer(Personage personage)
    {
        if (_currentComponentUnderPointer != personage)
        {
            Debug.Log(_gameMode.ToString());
            if (_gameMode == GameMode.Free)
            {
                if (PlayerController.ActiveAction == ActionType.Attack)
                {
                    ShowAttackInfo(personage);
                }
                else
                {
                    CanvasManager.Instance.ShowInfoUnderPosition(personage.PersonageInfo.Name, personage.transform.position);
                }
            }
            else if(_gameMode == GameMode.Battle)
            {
                if(personage.battleTeam == BattleTeam.Allies)
                {
                    CanvasManager.Instance.ShowInfoUnderPosition(personage.PersonageInfo.Name, personage.transform.position);
                }
                else
                {
                    ShowAttackInfo(personage);
                }
            }
            _currentComponentUnderPointer = personage;
        }
    }

    private void ShowAttackInfo(Personage personage)
    {
        PlayerController.GetAttackInfo(personage, out int bonus, out int difficulty, out Characteristics characteristic);
        StringBuilder attackInfoBuider = new();
        attackInfoBuider.Append(bonus >= 0 ? "Бонус от " : "Штраф от ");
        attackInfoBuider.Append(Translator.Translate(characteristic));
        attackInfoBuider.AppendLine(bonus >= 0 ? $" +{bonus}" : $" {bonus}");
        attackInfoBuider.Append($"Броня: {difficulty}");
        CanvasManager.Instance.ShowInfoUnderPosition(attackInfoBuider.ToString(), personage.transform.position);
    }

    public void OnPersonagePressed(Personage personage)
    {
        if (_gameMode == GameMode.Free)
        {
            if (PlayerController.ActiveAction == ActionType.Attack)
            {
                WeaponInfo weaponInfo = PlayerController.WeaponInfo;
                float attackDistance = weaponInfo ? weaponInfo.MaxAttackDistance : GameData.MaxUnarmedAttackDistance;
                PlayerController.InteractWith(personage.gameObject, attackDistance, Attack, personage);
            }
            else if(personage.TryGetComponent(out DialogueActor dialogueActor))
            {
                PlayerController.InteractWith(dialogueActor.gameObject, dialogueActor.MaxDialogueDistance, StartDialogue, dialogueActor);
            }
        }
    }

    public void OnItemUnderPointer(Item item)
    {
        if (_currentComponentUnderPointer != item)
        {
            CanvasManager.Instance.ShowInfoUnderPosition(item.ItemInfo.Name, item.transform.position);
            _currentComponentUnderPointer = item;
        }
    }

    public void OnItemPressed(Item item)
    {
        PlayerController.InteractWith(item.gameObject, 1, ItemInteract, item);
    }

    public void NothingUnderPointer()
    {
        CanvasManager.Instance.HideInfoUnderPointer();
        _currentComponentUnderPointer = null;
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

    public void Attack(GameObject attackingObject, Component component)
    {
        PlayerController.Attack(component as Personage);
        if(_gameMode != GameMode.Battle)
        {
            BattleManager.StartBattle(new Personage[2] { PlayerPersonage, component as Personage}); // TODO: add more personages in battle
        }
    }

    public void OnGroundPressed(Vector3 hitPoint)
    {
        if(_gameMode == GameMode.Free)
        {
            PlayerController.OnGroundPressedInFree(hitPoint);
        }
        else if(_gameMode == GameMode.Battle && BattleManager.ActivePersonage == PlayerPersonage)
        {
            PlayerController.OnGroundPressedInBattle(hitPoint);
        }
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
        CanvasManager.Instance.SetActivePersonage(personage);
    }
}