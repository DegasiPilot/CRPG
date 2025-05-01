using System.Collections;
using System.Linq;
using System.Text;
using CRPG;
using CRPG.DataSaveSystem;
using DialogueSystem.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LifetimeScope LifetimeScope;

    public static GameManager Instance;
    public CanvasManager _canvasManager;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private DialogueParser _dialogueParser;
    [SerializeField] private SceneSaveLoadManager _sceneSaveLoadManager;

    [System.NonSerialized] public UnityEvent OnDeathEvent = new();

    [Tooltip("Выбери родительский объект персонажей и очисти список персонажей, они автоматически перезаполнятся")]
    public GameObject PersonagesRoot;
    [Tooltip("Для отчистки: нажми на любого персонажа, Ctrl+A и кнопка минус")]
    [SerializeField] private Personage[] _personages;

    private PlayerController _activePlayer => GameData.ActivePlayer;
    private Personage PlayerPersonage => _activePlayer.Personage;
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
        BattleManager.OnBattleStartEvent.AddListener(() => ChangeGameMode(GameMode.Battle));
        BattleManager.OnBattleEndEvent.AddListener(() => ChangeGameMode(GameMode.Free));
		using (var container = LifetimeScope.Container)
		{
			GlobalDataManager globalDataManager = container.Resolve<GlobalDataManager>();
			foreach (var personage in _personages)
			{
				if (GameData.Companions.Any(
					companion => companion.Personage.PersonageInfo.Name ==
					personage.PersonageInfo.Name))
				{
					Destroy(personage.gameObject);
					continue;
				}
				personage.PersonageInfo.Setup(globalDataManager.GetRaceInfo);
				personage.Setup(personage.PersonageInfo);
				_sceneSaveLoadManager.LoadSceneFromSave(GameData.SceneSaveInfo, globalDataManager);
			}
		}
	}

    void Start()
    {
        _dialogueParser.Setup();
        _cameraController.Setup(this);
        using (var container = LifetimeScope.Container)
        {
			_canvasManager.Setup(OnDeathEvent, container.Resolve<GlobalDataManager>().GetActionInfo);
		}
        SetActivePlayer(GameData.MainPlayer.PlayerController);
        OnDeathEvent.AddListener(() => gameObject.SetActive(false));
        _canvasManager.OnDropItem.AddListener(DropItem);
    }

    private void DropItem(Item item)
    {
        GameData.ActivePlayer.DropItem(item);
	}

    public void ChangeGameMode(GameMode gameMode)
    {
        if (_gameMode != gameMode)
        {
            _canvasManager.OnChangeGameMode(_gameMode, gameMode);
            if (_gameMode == GameMode.Dialogue)
            {
                _cameraController.enabled = true;
                _cameraController.StandartView();
            }
            
            if(gameMode == GameMode.Dialogue)
            {
                NothingUnderPointer();
                _cameraController.enabled = false;
            }
            else if(gameMode == GameMode.Battle)
            {
                _activePlayer.ForceStop();
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
                if (_activePlayer.ActiveAction == ActionType.Attack)
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
		float distance = Vector3.Distance(_activePlayer.Personage.HitPoint.position, personage.HitPoint.position);
        attackInfoBuider.AppendLine($"Расстояние {distance.ToString("0.#")}");
        if (personage.IsDead)
        {
			attackInfoBuider.AppendLine("Мертв");
		}
        else
        {
			if (distance <= _activePlayer.MaxAttackDistance && !BattleManager.AttackRaycast(PlayerPersonage.HitPoint.position, personage.HitPoint.position, _activePlayer.MaxAttackDistance, personage))
			{
				attackInfoBuider.AppendLine("Цель за укрытием");
			}
			attackInfoBuider.AppendLine($"Броня: {personage.ArmorPercent.ToString("0.#%")}");
            EquipmentManager equipmentManager = _activePlayer.Personage.EquipmentManager;
			if (equipmentManager.IsWeaponNeedProjectiles && !equipmentManager.CanReloadWeapon)
            {
                attackInfoBuider.Append("Нет предметов: " + equipmentManager.Weapon.WeaponInfo.RequiredProjectile.Name);
            }
		}
        _canvasManager.ShowInfoUnderPosition(attackInfoBuider.ToString(), personage.HitPoint.position);
    }

	public void OnPersonagePressed(PersonageController personageController)
	{
		if (_gameMode == GameMode.Free)
		{
			if (_activePlayer.ActiveAction == ActionType.Attack)
			{
				float attackDistance = _activePlayer.MaxAttackDistance;
				_activePlayer.InteractWith(attackDistance, AttackInteract, personageController);
			}
			else if (personageController.TryGetComponent(out DialogueActor dialogueActor))
			{
				_activePlayer.InteractWith(dialogueActor.MaxDialogueDistance, StartDialogue, dialogueActor);
			}
		}
		else if (_gameMode == GameMode.Battle && BattleManager.ActivePersonage == PlayerPersonage)
		{
			if (BattleManager.CanAttack(personageController.Personage))
			{
				_activePlayer.StartAttack(personageController);
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
            _activePlayer.InteractWith(1, ItemInteract, item);
        }
        else if(GameMode == GameMode.Battle)
        {
            Vector3 distVector = item.transform.position - _activePlayer.transform.position;
            Vector3 distVector2 = item.transform.position - _activePlayer.Personage.HitPoint.position;
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
        _dialogueParser.SetSecondDialogueActor(dialogueActor as DialogueActor);
        _cameraController.FocusOn(dialogueActor as DialogueActor);
        _dialogueParser.TryStartDialogue();
        ChangeGameMode(GameMode.Dialogue);
    }

    public void ItemInteract(Component item)
    {
        _activePlayer.PickupItem(item as Item);
    }

	public void AttackInteract(Component component)
	{
		PersonageController personageController = component as PersonageController;
		_activePlayer.StartAttack(personageController);
	}

	public void OnGroundPressed(Vector3 hitPoint)
    {
        if(_gameMode == GameMode.Free)
        {
            _activePlayer.OnGroundPressedInFree(hitPoint);
        }
        else if(_gameMode == GameMode.Battle && BattleManager.ActivePersonage == PlayerPersonage)
        {
            _activePlayer.OnGroundPressedInBattle(hitPoint);
        }
    }

	internal void OnPlayerPressed(PlayerController player)
	{
		if(GameData.ActivePlayer != player)
        {
            if(GameData.MainPlayer.PlayerController != player && !GameData.Companions.Contains(player))
            {
                GameData.Companions.Add(player);
                player.transform.SetParent(null);
                DontDestroyOnLoad(player);
            }
			SetActivePlayer(player);
        }
	}

	public void CreateNewGameSave()
    {
        using (var container = LifetimeScope.Container)
        {
			GlobalDataManager globalDataManager = container.Resolve<GlobalDataManager>();
			GameData.SceneSaveInfo = _sceneSaveLoadManager.GetSceneSave(globalDataManager);
			container.Resolve<IDataSaveLoader>().CreateGameSaveInfo(GameData.NewGameSave());
		}
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
        _canvasManager.ToggleInventory(GameData.Inventory, _activePlayer.Personage.EquipmentManager);
        if (_canvasManager.IsInventoryOpen)
        {
            _cameraController.enabled = false;
        }
        else
        {
            _cameraController.enabled = true;
        }
    }

    public void TogglePauseMenu()
    {
        _canvasManager.TogglePauseMenu();
        if (_canvasManager.IsPauseMenuOpen)
        {
            _cameraController.enabled = false;
            Time.timeScale = 0;
        }
        else
        {
            _cameraController.enabled = true;
            Time.timeScale = 1;
        }
    }

    internal void SetActivePlayer(PlayerController player)
    {
		GameData.ActivePlayer = player;
        _cameraController.OnSetActivePlayer(player.transform);
		_canvasManager.SetActivePersonage(player, player.Personage.EquipmentManager);
	}

	private void OnDestroy()
	{
        Time.timeScale = 1;
        GameData.SceneSaveInfo = null;
	}
}