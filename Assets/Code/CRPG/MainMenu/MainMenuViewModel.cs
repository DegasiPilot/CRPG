using CRPG.DataSaveSystem;
using CRPG.DataSaveSystem.SaveData;
using System.Collections.Generic;

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
			_mainMenuScript.OnAwake.AddListener(Awake);
		}

		public void Awake()
		{
			MongoDataSaveLoader mongo = MongoDataSaveLoader.Instance;
			if (mongo.Ping())
			{
				_dataSaveLoader = mongo;
			}
			else
			{
				//if (GlobalDataManager.DataSaveLoader != LocalSaveLoader.Instance)
				//{
					_messageBoxManager.ShowMessage("Нет подключения к БД. Включен оффлайн режим");
				//}
				_dataSaveLoader = LocalSaveLoader.Instance;
			}

			GlobalDataManager.DataSaveLoader = _dataSaveLoader;
			_mainMenuScript.AuthRegManager.Activate(_dataSaveLoader, _messageBoxManager);
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
			_mainMenuScript.LoadScene("SampleScene");
		}

		public List<GameSaveInfo> GetSaves()
		{
			return _saves ??= _dataSaveLoader.GetAllGameSaves();
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