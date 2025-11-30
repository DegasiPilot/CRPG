using CRPG;
using CRPG.DataSaveSystem;
using CRPG.Interactions;
using DialogueSystem.Runtime;
using System.Linq;
using System.Text;
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
	internal SceneSaveLoadManager SceneSaveLoadManager => _sceneSaveLoadManager;

	[System.NonSerialized] public UnityEvent OnDeathEvent = new();

	[Tooltip("Выбери родительский объект персонажей и очисти список персонажей, они автоматически перезаполнятся")]
	public GameObject PersonagesRoot;
	[Tooltip("Для отчистки: нажми на любого персонажа, Ctrl+A и кнопка минус")]
	[SerializeField] private Personage[] _personages;

	private PlayerController ActivePlayer => GameData.ActivePlayer;
	private GameMode _gameMode = GameMode.Free;
	public GameMode GameMode => _gameMode;
	private Component _currentComponentUnderPointer;

	private void OnValidate()
	{
		if (PersonagesRoot != null && _personages != null && _personages.Length == 0)
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
			_sceneSaveLoadManager.LoadSceneFromSave(GameData.SceneSaveInfo, globalDataManager);
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

			if (gameMode == GameMode.Dialogue)
			{
				NothingUnderPointer();
				_cameraController.enabled = false;
			}
			else if (gameMode == GameMode.Battle)
			{
				ActivePlayer.ForceStop();
			}
			_gameMode = gameMode;
		}
	}

	public void OnPlayerUnderPointer(PlayerController playerController)
	{
		Personage personage = playerController.Personage;
		if (_currentComponentUnderPointer != personage)
		{
			_canvasManager.ShowInfoUnderPosition(personage.PersonageInfo.Name + " (это вы)", personage.HitPoint.position);
			_currentComponentUnderPointer = personage;
		}
	}

	public void OnPersonageUnderPointer(PersonageController personageController)
	{
		Personage personage = personageController.Personage;
		if (_currentComponentUnderPointer != personage)
		{
			if (_gameMode == GameMode.Free)
			{
				if (ActivePlayer.ActiveAction == ActionType.Attack)
				{
					ShowAttackInfo(personageController);
				}
				else
				{
					_canvasManager.ShowInfoUnderPosition(personage.PersonageInfo.Name, personage.HitPoint.position);
				}
			}
			else if (_gameMode == GameMode.Battle)
			{
				if (personage.BattleTeam == BattleTeam.Allies)
				{
					_canvasManager.ShowInfoUnderPosition(personage.PersonageInfo.Name, personage.HitPoint.position);
				}
				else
				{
					ShowAttackInfo(personageController);
				}
			}
			_currentComponentUnderPointer = personage;
		}
	}

	private void ShowAttackInfo(PersonageController personageController)
	{
		Personage personage = personageController.Personage;
		StringBuilder attackInfoBuider = new();
		attackInfoBuider.AppendLine(personage.PersonageInfo.Name);
		float distance = Vector3.Distance(ActivePlayer.transform.position, personage.transform.position) - personageController.Radius;
		attackInfoBuider.AppendLine($"Расстояние {distance.ToString("0.#")}");
		if (personage.IsDead)
		{
			attackInfoBuider.AppendLine("Мертв");
		}
		else
		{
			attackInfoBuider.AppendLine($"Броня: {personage.ArmorPercent.ToString("0.#%")}");
			EquipmentManager equipmentManager = ActivePlayer.Personage.EquipmentManager;
			if (equipmentManager.IsWeaponNeedProjectiles && !equipmentManager.CanReloadWeapon)
			{
				attackInfoBuider.AppendLine("Нет предметов: " + equipmentManager.Weapon.WeaponInfo.RequiredProjectile.Name);
			}
			if (ActivePlayer.Personage.Stamina < ActivePlayer.Personage.MinAttackEnergy)
			{
				attackInfoBuider.AppendLine("Недостаточно энергии!");
			}
		}
		_canvasManager.ShowInfoUnderPosition(attackInfoBuider.ToString(), personage.HitPoint.position);
	}

	public void OnPersonagePressed(PersonageController personageController)
	{
		if (_gameMode == GameMode.Free)
		{
			if (ActivePlayer.ActiveAction == ActionType.Attack)
			{
				float attackDistance = ActivePlayer.MaxAttackDistance;
				ActivePlayer.InteractWith(attackDistance, personageController.transform.position, new AttackInteract(personageController));
			}
			else if (personageController.TryGetComponent(out DialogueActor dialogueActor))
			{
				ActivePlayer.InteractWith(dialogueActor.MaxDialogueDistance, dialogueActor.transform.position, new DialogueInteract(dialogueActor, this));
			}
		}
		else if (_gameMode == GameMode.Battle && BattleManager.ActivePersonageController == ActivePlayer)
		{
			if (BattleManager.CanAttack(personageController.Personage))
			{
				ActivePlayer.StartAttack(personageController);
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
			ActivePlayer.InteractWith(GlobalRules.MaxItemTakeDistance, item.transform.position, new PickupItemInteract(item));
		}
		else if (GameMode == GameMode.Battle)
		{
			Vector3 distVector = item.transform.position - ActivePlayer.transform.position;
			Vector3 distVector2 = item.transform.position - ActivePlayer.Personage.HitPoint.position;
			if (Vector3.SqrMagnitude(distVector) <= 1 || Vector3.SqrMagnitude(distVector2) <= 1)
			{
				ActivePlayer.PickupItem(item);
			}
			else
			{
				LifetimeScope.Container.Resolve<MessageBoxManager>().ShowMessage("Предмет слишком далеко!");
			}
		}
	}

	public void NothingUnderPointer()
	{
		_canvasManager.HideInfoUnderPointer();
		_currentComponentUnderPointer = null;
	}

	public void StartDialogue(DialogueActor dialogueActor)
	{
		_dialogueParser.SetSecondDialogueActor(dialogueActor);
		_cameraController.FocusOn(dialogueActor);
		_dialogueParser.TryStartDialogue();
		ChangeGameMode(GameMode.Dialogue);
	}

	public void OnGroundPressed(Vector3 hitPoint)
	{
		if (_gameMode == GameMode.Free)
		{
			ActivePlayer.OnGroundPressedInFree(hitPoint);
		}
		else if (_gameMode == GameMode.Battle && BattleManager.ActivePersonageController == ActivePlayer)
		{
			ActivePlayer.OnGroundPressedInBattle(hitPoint);
		}
	}

	public void CreateNewGameSave()
	{
		if (CanSaveGame)
		{
			using (var container = LifetimeScope.Container)
			{
				GlobalDataManager globalDataManager = container.Resolve<GlobalDataManager>();
				GameData.SceneSaveInfo = _sceneSaveLoadManager.GetSceneSave(globalDataManager);
				GlobalDataManager.DataSaveLoader.CreateGameSaveInfo(GameData.NewGameSave());
			}
			LifetimeScope.Container.Resolve<MessageBoxManager>().ShowMessage("Игра сохранена!");
		}
	}

	public bool CanSaveGame => GameMode == GameMode.Free && GameData.ActivePlayer.IsFree;

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
		_canvasManager.ToggleInventory(GameData.Inventory, ActivePlayer.Personage.EquipmentManager, ActivePlayer.Personage.PersonageInfo);
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

	internal void ShowAttackResult(string result)
	{
		LifetimeScope.Container.Resolve<MessageBoxManager>().ShowMessage(result);
	}

	private void OnDestroy()
	{
		Time.timeScale = 1;
		GameData.SceneSaveInfo = null;
	}
}