using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CRPG.DataSaveSystem
{
	internal class LocalSaveLoader : IDataSaveLoader
	{
		private List<GameSaveInfo> _gameSaves = new();
		private List<PersonageInfo> _personages = new();
		private List<User> _users = new();

		public void CreateGameSaveInfo(GameSaveInfo gameSave)
		{
			_gameSaves.Add(gameSave);
		}

		public void CreatePersonageInfo(PersonageInfo personage)
		{
			_personages.Add(personage);
		}

		public void CreateUser(User user)
		{
			_users.Add(user);
		}

		public List<GameSaveInfo> GetAllGameSaves(User user)
		{
			return _gameSaves.Where(save => IsSaveBelongUser(save, user)).ToList();
		}

		public GameSaveInfo GetLastGameSave(User user)
		{
			return _gameSaves.LastOrDefault(save => IsSaveBelongUser(save, user));
		}

		private bool IsSaveBelongUser(GameSaveInfo save, User user)
		{
			return save.UserLogin == user.Login;
		}

		public Task<User> GetUserWithLoginAsync(string login)
		{
			return Task.FromResult(_users.First(u => u.Login == login));
		}

		public bool HasAnySaves(User user)
		{
			return _gameSaves.Any(save => IsSaveBelongUser(save, user));
		}

		public bool IsExistPersonageInfo(PersonageInfo p)
		{
			return _personages.FirstOrDefault(personage => personage.Id == p.Id) != null;
		}

		public void RedactPersonageInfo(PersonageInfo personage)
		{
			//cringe
		}

		public bool TryDeletePersonageInfo(ObjectId id)
		{
			return _personages.RemoveAll(personage => personage.Id == id) > 0;
		}
	}
}
