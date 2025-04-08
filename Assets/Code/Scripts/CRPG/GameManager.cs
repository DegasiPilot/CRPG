using System.Collections;
using System.Text;
using CRPG;
using CRPG.DI;
using DialogueSystem.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CanvasManager _canvasManager;

    [System.NonSerialized] public UnityEvent OnDeathEvent = new();

    [Tooltip("Выбери родительский объект персонажей и очисти список персонажей, они автоматически перезаполнятся")]
    public GameObject PersonagesRoot;
    [Tooltip("Для отчистки: нажми на любого персонажа Ctrl+A и кнопка минус")]
    [SerializeField] private Personage[] _personages;

    private Player _player => GameData.Player;
    private PlayerController PlayerController => _player.PlayerController;
    private Personage PlayerPersonage => PlayerController.Personage;
    private GameMode _gameMode = GameMode.Free;
    public GameMode GameMode => _gameMode;
    private Component _currentComponentUnderPointer;

	private void OnValidate()
	{
		if(PersonagesRoot != null && _personages != null && _personages.Length == 0)
        {
            _personages = PersonagesRoot.GetComponentsInChildren<Personage>();
        }
	}

	private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        BattleManager.OnBattleStartEvent.AddListener(() => ChangeGameMode(GameMode.Battle));
        BattleManager.OnBattleEndEvent.AddListener(() => ChangeGameMode(GameMode.Free));
        foreach(var personage in _personages)
        {
            if (personage == _player.PlayerController.Personage) continue;
            personage.PersonageInfo.Setup(GameData.GetRaceInfo);
            personage.Setup(personage.PersonageInfo);
        }
    }

    void Start()
    {
        SceneSaveLoadManager.Instance.LoadSceneFromSave(GameData.SceneSaveInfo);
        DialogueParser.Instance.Setup();
        EquipmentManager.Instance.Setup(GameData.Player.PlayerCustomizer.EquipmentCustomizer, GameData.Inventory);
        SetActivePlayer(_player);
        OnDeathEvent.AddListener(() => gameObject.SetActive(false));
        _canvasManager.OnDropItem.AddListener(GameData.Player.PlayerController.DropItem);
    }

    public void ChangeGameMode(GameMode gameMode)
    {
        if (_gameMode != gameMode)
        {
            _canvasManager.OnChangeGameMode(_gameMode, gameMode);
            if (_gameMode == GameMode.Dialogue)
            {
                CameraController.Instance.enabled = true;
                CameraController.Instance.StandartView();
            }
            
            if(gameMode == GameMode.Dialogue)
            {
                NothingUnderPointer();
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
                    _canvasManager.ShowInfoUnderPosition(personage.PersonageInfo.Name, personage.HitPoint.position);
                }
            }
            else if(_gameMode == GameMode.Battle)
            {
                if(personage.BattleTeam == BattleTeam.Allies)
                {
                    _canvasManager.ShowInfoUnderPosition(personage.PersonageInfo.Name, personage.HitPoint.position);
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
        StringBuilder attackInfoBuider = new();
		attackInfoBuider.AppendLine(personage.PersonageInfo.Name);
		float distance = Vector3.Distance(PlayerController.Personage.HitPoint.position, personage.HitPoint.position);
        attackInfoBuider.AppendLine($"Расстояние {distance.ToString("0.#")}");
        if (personage.IsDead)
        {
			attackInfoBuider.AppendLine("Мертв");
		}
        else
        {
			if (distance <= PlayerController.MaxAttackDistance && !BattleManager.AttackRaycast(PlayerPersonage.HitPoint.position, personage.HitPoint.position, PlayerController.MaxAttackDistance, personage))
			{
				attackInfoBuider.AppendLine("Цель за укрытием");
			}
			attackInfoBuider.Append($"Броня: {personage.ArmorPercent.ToString("0.#%")}");
		}
        _canvasManager.ShowInfoUnderPosition(attackInfoBuider.ToString(), personage.HitPoint.position);
    }

	public void OnPersonagePressed(PersonageController personageController)
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
				PlayerController.StartAttack(personageController);
			}
		}
	}

	public void OnItemUnderPointer(Item item)
    {
        if (_currentComponentUnderPointer != item)
        {
            _canvasManager.ShowInfoUnderPosition(item.ItemInfo.Name, item.transform.position);
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
        _canvasManager.HideInfoUnderPointer();
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
		PersonageController personageController = component as PersonageController;
		PlayerController.StartAttack(personageController);
        var waiting = new WaitWhile(() => PlayerController.IsAttacking);
        StartCoroutine(AfterAttack(waiting, personageController));
	}
    
    private IEnumerator AfterAttack(CustomYieldInstruction attackWaiting, PersonageController personageController)
    {
        yield return attackWaiting;
		if (_gameMode != GameMode.Battle)
		{
			personageController.Personage.BattleTeam = BattleTeam.Enemies;
			BattleManager.StartBattle(new PersonageController[2] { PlayerController, personageController });
		}
		else if (!BattleManager.ParticipantPersonages.Contains(personageController))
		{
			BattleManager.JoinToBattle(personageController);
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
        DI.DataSaveLoader.CreateGameSaveInfo(GameData.NewGameSave(PlayerController.Personage.PersonageInfo));
    }

    public void ExitToMainMenu()
    {
        GameData.ClearData();
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
            if (_canvasManager.IsInventoryOpen)
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
        _canvasManager.ToggleInventory(GameData.Inventory);
        if (_canvasManager.IsInventoryOpen)
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
        _canvasManager.TogglePauseMenu();
        if (_canvasManager.IsPauseMenuOpen)
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

    internal void SetActivePlayer(Player player)
    {
		GameData.Player = player;
		_canvasManager.SetActivePersonage(player.PlayerController);
	}
}