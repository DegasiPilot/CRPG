using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DialogueSystem.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [System.NonSerialized] public UnityEvent OnHealthChanged = new();

    public GameObject Personages;

    [HideInInspector] public PlayerController PlayerController;
    [HideInInspector] public List<PlayerController> PlayerControllers;
    public Personage PlayerPersonage => PlayerController.Personage;
    public PersonageController[] PersonageControllers;
    private GameMode _gameMode = GameMode.Free;
    public GameMode GameMode => _gameMode;
    private Component _currentComponentUnderPointer;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        BattleManager.OnBattleStartEvent.AddListener(() => ChangeGameMode(GameMode.Battle));
        BattleManager.OnBattleEndEvent.AddListener(() => ChangeGameMode(GameMode.Free));
    }

    void Start()
    {
        SceneSaveLoadManager.Instance.LoadSceneFromSave(GameData.SceneSaveInfo);
        DialogueParser.Instance.Setup();
        PlayerController = GameData.PlayerController;
        PlayerControllers = new() { PlayerController }; // TODO: More player controllers
        PersonageControllers = Personages.GetComponentsInChildren<PersonageController>();
        foreach(var controller in PersonageControllers)
        {
            if(controller == PlayerController)
            {
                continue;
            }
            controller.Setup();
        }
        SetActivePersonage(PlayerController);
    }

    public void ChangeGameMode(GameMode gameMode)
    {
        if (_gameMode != gameMode)
        {
            CanvasManager.Instance.OnChangeGameMode(_gameMode, gameMode);
            if (_gameMode == GameMode.Dialogue)
            {
                CameraController.Instance.enabled = true;
                CameraController.Instance.StandartView();
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
                if(personage.BattleTeam == BattleTeam.Allies)
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
        float distance = Vector3.Distance(PlayerController.transform.position, personage.transform.position);
        if (distance > PlayerController.MaxAttackDistance)
        {
            attackInfoBuider.AppendLine("Цель слишком далеко!");
        }
        else if(!BattleManager.AttackRaycast(PlayerPersonage.HitPoint.position, personage.HitPoint.position, PlayerController.MaxAttackDistance, personage))
        {
            attackInfoBuider.AppendLine("Цель за укрытием");
        }
        attackInfoBuider.AppendLine($"Расстояние до цели {(int)distance}");
        attackInfoBuider.Append(bonus >= 0 ? "Бонус от " : "Штраф от ");
        attackInfoBuider.Append(Translator.Translate(characteristic));
        attackInfoBuider.AppendLine(bonus >= 0 ? $" +{bonus}" : $" {bonus}");
        attackInfoBuider.AppendLine($"Броня: {difficulty}");
        attackInfoBuider.AppendLine($"Шанс попадания {(20 + bonus - difficulty)*100/20}%");
        CanvasManager.Instance.ShowInfoUnderPosition(attackInfoBuider.ToString(), personage.transform.position);
    }

    public void OnPersonagePressed(PersonageController personageController)
    {
        StartCoroutine(OnGroundPressed());

        IEnumerator OnGroundPressed()
        {
            if (_gameMode == GameMode.Free)
            {
                if (PlayerController.ActiveAction == ActionType.Attack)
                {
                    float attackDistance = PlayerController.MaxAttackDistance;
                    PlayerController.InteractWith(attackDistance, AttackInteract, personageController);
                }
                else if (personageController.TryGetComponent(out DialogueActor dialogueActor))
                {
                    PlayerController.InteractWith(dialogueActor.MaxDialogueDistance, StartDialogue, dialogueActor);
                }
            }
            else if (_gameMode == GameMode.Battle && BattleManager.ActivePersonage == PlayerPersonage)
            {
                if (BattleManager.CanAttack(personageController.Personage))
                {
                    BattleManager.HasAction = false;
                    do
                    {
                        yield return PlayerController.Attack(personageController);
                    } 
                    while (PlayerController.IsAttacking);
                }
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
        if (GameMode == GameMode.Free)
        {
            PlayerController.InteractWith(1, ItemInteract, item);
        }
        else if(GameMode == GameMode.Battle)
        {
            Vector3 distVector = item.transform.position - PlayerController.transform.position;
            Vector3 distVector2 = item.transform.position - PlayerController.Personage.HitPoint.position;
            if (Vector3.SqrMagnitude(distVector) <= 1 || Vector3.SqrMagnitude(distVector2) <= 1)
            {
                ItemInteract(item);
            }
            else
            {
                MessageBoxManager.ShowMessage("Предмет слишком далеко!");
            }
        }
    }

    public void NothingUnderPointer()
    {
        CanvasManager.Instance.HideInfoUnderPointer();
        _currentComponentUnderPointer = null;
    }

    public void StartDialogue(Component dialogueActor)
    {
        DialogueParser.Instance.SetSecondDialogueActor(dialogueActor as DialogueActor);
        CameraController.Instance.FocusOn(dialogueActor as DialogueActor);
        DialogueParser.Instance.TryStartDialogue();
        ChangeGameMode(GameMode.Dialogue);
    }

    public void ItemInteract(Component item)
    {
        PlayerController.PickupItem(item as Item);
    }

    public void AttackInteract(Component component)
    {
        StartCoroutine(AttackInteract());

        IEnumerator AttackInteract()
        {
            PersonageController personageController = component as PersonageController;
            do
            {
                yield return PlayerController.Attack(personageController);
            }
            while (PlayerController.IsAttacking);
            if (_gameMode != GameMode.Battle)
            {
                personageController.Personage.BattleTeam = BattleTeam.Enemies;
                BattleManager.StartBattle(new Personage[2] { PlayerPersonage, personageController.Personage }); // TODO: add more 
            }
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
        Destroy(GameData.PlayerController.Inventory);
        Destroy(GameData.PlayerController.gameObject);
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
            if (CanvasManager.Instance.IsInventoryOpen)
            {
                ToggleInvenoty();
            }
            else
            {
                TogglePauseMenu();
            }
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

    public void SetActivePersonage(PlayerController playerController)
    {
        PlayerController.Personage.OnHealthChanged.RemoveListener(OnHealthChanged.Invoke);
        PlayerController = playerController;
        PlayerController.Personage.OnHealthChanged.AddListener(OnHealthChanged.Invoke);
        CanvasManager.Instance.SetActivePersonage(playerController.Personage);
    }
}