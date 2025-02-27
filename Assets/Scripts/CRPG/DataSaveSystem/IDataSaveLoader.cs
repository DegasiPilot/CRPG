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
	internal interface IDataSaveLoader
	{
		public void CreatePersonageInfo(PersonageInfo personage);

		public bool IsExistPersonageInfo(PersonageInfo p);

		public void RedactPersonageInfo(PersonageInfo personage);

		public bool TryDeletePersonageInfo(ObjectId id);

		public void CreateGameSaveInfo(GameSaveInfo gameSave);

		public GameSaveInfo GetLastGameSave(User user);

		public List<GameSaveInfo> GetAllGameSaves(User user);

		public bool HasAnySaves(User user);

		public void CreateUser(User user);

		public Task<User> GetUserWithLoginAsync(string login);
	}
}
