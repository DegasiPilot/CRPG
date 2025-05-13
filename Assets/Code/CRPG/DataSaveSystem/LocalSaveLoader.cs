using CRPG.DataSaveSystem.SaveData;
using System.Collections.Generic;
using UnityEngine;

namespace CRPG.DataSaveSystem
{
	internal class LocalSaveLoader : IDataSaveLoader
	{
		private static LocalSaveLoader _instance;
		public static LocalSaveLoader Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LocalSaveLoader();
				}
				return _instance;
			}

		}

		public bool IsUserLogined => true;
		public string UserLogin => _user.Login;
		public bool CanExit => false;

		private User _user = new() { Login = "Оффлайн" };

		private LocalSaveLoader()
		{

		}

		public void CreateGameSaveInfo(GameSaveInfo gameSave)
		{
			_user.AddGameSave(gameSave);
		}

		public List<GameSaveInfo> GetAllGameSaves()
		{
			return _user.GameSaves;
		}

		public GameSaveInfo GetLastGameSave()
		{
			return _user.GameSaves?[_user.GameSaves.Count - 1];
		}

		public bool TryLogin(string login, string password, out string errors)
		{
			if (_user.Login != login)
			{
				errors = "Неверный логин";
			}
			else if (_user.Password != password)
			{
				errors = "Неверный пароль";
			}
			else
			{
				errors = null;
				return true;
			}
			return false;
		}

		public bool TryRegistrate(string login, string password, out string errors)
		{
			errors = null;
			Debug.LogWarning("Try local Registrate");
			return true;
		}

		public void UpdateUser(User user)
		{
			_user.Login = user.Login;
			_user.Password = user.Password;
		}
	}
}
