using CRPG.DataSaveSystem;
using CRPG.DataSaveSystem.SaveData;
using System.Collections.Generic;
using System.Linq;

namespace CRPG.MainMenu
{
	internal class MainMenuViewModel
	{
		private IDataSaveLoader _dataSaveLoader;
		private MessageBoxManager _messageBoxManager;
		private GameDataManager _gameDataManager;
		private List<GameSaveInfo> _saves;
		private MainMenuScript _mainMenuScript;

		public MainMenuViewModel(GameDataManager gameDataManager, MainMenuScript mainMenuScript, MessageBoxManager messageBoxManager)
		{
			_messageBoxManager = messageBoxManager;
			_gameDataManager = gameDataManager;
			_mainMenuScript = mainMenuScript;
			_mainMenuScript.LoadLastGameButton.onClick.AddListener(LoadLastGame);
			_mainMenuScript.LoadGameButton.onClick.AddListener(LoadGameBtnListener);
			_mainMenuScript.StartNewGameButton.onClick.AddListener(StartNewGame);
			_mainMenuScript.ExitGameButton.onClick.AddListener(ExitGame);
			_mainMenuScript.ExitFromAccountButton.onClick.AddListener(ExitFromAccount);
			_mainMenuScript.OnAwake.AddListener(Awake);
		}

		public void Awake()
		{
			_mainMenuScript.AuthRegManager.AfterUserInitializedEvent += AfterUserInitialized;
			StartAuthorization();
		}

		private void ExitFromAccount()
		{
			_mainMenuScript.AuthRegManager.ExitFromAccount();
			StartAuthorization();
		}

		private void StartAuthorization()
		{
			MongoDataSaveLoader mongo = MongoDataSaveLoader.Instance;
			if (mongo.Ping())
			{
				_dataSaveLoader = mongo;
			}
			else
			{
				if (GlobalDataManager.DataSaveLoader != LocalSaveLoader.Instance)
				{
					_messageBoxManager.ShowMessage("Нет подключения к БД. Включен оффлайн режим");
				}
				_dataSaveLoader = LocalSaveLoader.Instance;
			}

			GlobalDataManager.DataSaveLoader = _dataSaveLoader;
			_mainMenuScript.LoadLastGameButton.interactable = false;
			_mainMenuScript.LoadGameButton.interactable = false;
			_mainMenuScript.StartNewGameButton.interactable = false;
			_mainMenuScript.AuthRegManager.Activate(_dataSaveLoader, _messageBoxManager);
		}

		private void AfterUserInitialized()
		{
			bool hasSaves = _dataSaveLoader.HasSaves;
			_mainMenuScript.LoadLastGameButton.interactable = hasSaves;
			_mainMenuScript.LoadGameButton.interactable = hasSaves;
			_mainMenuScript.StartNewGameButton.interactable = true;
		}

		public void StartNewGame()
		{
			_gameDataManager.InitNewGame();
			_mainMenuScript.LoadScene("PersonageRedactorScene");
		}

		public void LoadLastGame()
		{
			GameSaveInfo save = _dataSaveLoader.GetLastGameSave();
			if (save is not null)
			{
				LoadGame(save);
			}
		}

		public void LoadGame(GameSaveInfo gameSave)
		{
			_mainMenuScript.ShowLoadScreen();
			_gameDataManager.LoadGameSave(gameSave);
			_mainMenuScript.LoadScene("MainScene");
		}

		public List<GameSaveInfo> GetSaves()
		{
			return _saves ??= _dataSaveLoader.GetAllGameSaves().OrderByDescending(user => user.DateTime).ToList();
		}

		public void ExitGame()
		{
			_mainMenuScript.ApplicationQuit();
		}

		public void LoadGameBtnListener()
		{
			_mainMenuScript.ShowSaves(GetSaves(), LoadGame);
		}
	}
}