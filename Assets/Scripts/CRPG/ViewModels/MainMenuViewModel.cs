using CRPG.DataSaveSystem;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace CRPG.ViewModels
{
	internal class MainMenuViewModel : IStartable
	{
		private IDataSaveLoader _dataSaveLoader;
		private GameDataManager _gameDataManager;
		private List<GameSaveInfo> _saves;
		private MainMenuScript _mainMenuScript;

		public MainMenuViewModel(IDataSaveLoader dataSaveLoader, GameDataManager gameDataManager, MainMenuScript mainMenuScript)
		{
			_dataSaveLoader = dataSaveLoader;
			_gameDataManager = gameDataManager;
			_mainMenuScript = mainMenuScript;
			GameData.CurrentUser = new User() { Id = ObjectId.GenerateNewId(), Login = "Login", Password = "Password" };
		}

		public void StartNewGame()
		{
			SceneManager.LoadScene("PersonageRedactorScene");
		}

		public void LoadLastGame()
		{
			GameSaveInfo save = _dataSaveLoader.GetLastGameSave(GameData.CurrentUser);
			if (save is not null)
			{
				LoadGame(save);
			}
		}

		public void LoadGame(GameSaveInfo gameSave)
		{
			_mainMenuScript.ShowLoadScreen();
			_gameDataManager.LoadGameSave(gameSave);
			SceneManager.LoadScene("SampleScene");
		}

		public List<GameSaveInfo> GetSaves()
		{
			return _saves ??= _dataSaveLoader.GetAllGameSaves(GameData.CurrentUser);
		}

		public void ExitGame()
		{
			Application.Quit();
		}

		public void Start()
		{
			_mainMenuScript.LoadLastGameButton.onClick.AddListener(LoadLastGame);
			_mainMenuScript.LoadGameButton.onClick.AddListener(LoadGameBtnListener);
			_mainMenuScript.StartNewGameButton.onClick.AddListener(StartNewGame);
			_mainMenuScript.ExitGameButton.onClick.AddListener(ExitGame);
		}

		public void LoadGameBtnListener()
		{
			_mainMenuScript.ShowSaves(GetSaves(), LoadGame);
		}
	}
}
