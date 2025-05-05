using MongoDB.Driver;
using CRPG.DataSaveSystem.SaveData;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;

namespace CRPG.DataSaveSystem
{
	internal class MongoDataSaveLoader : IDataSaveLoader
	{
		const string connectionUri = "mongodb://localhost:27017";
		const string DbName = "CRPG_DB";
		MongoClient client = new MongoClient(connectionUri);
		private User _activeUser;

		public MongoDataSaveLoader()
		{
			var objectSerializer = new ObjectSerializer(ObjectSerializer.AllAllowedTypes);
			BsonSerializer.RegisterSerializer(objectSerializer);
			if (!BsonClassMap.IsClassMapRegistered(typeof(PersonageSaveInfo)))
			{
				BsonClassMap.RegisterClassMap<PersonageSaveInfo>();
			}
		}

		private IMongoCollection<T> TryGetCollection<T>(out string errors)
		{
			var database = client.GetDatabase(DbName);
			for(int attempt = 1; attempt <= 3; attempt++)
			{
				if (client.Cluster.Description.State != MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
				{
					errors = string.Empty;
					return database.GetCollection<T>(typeof(T).Name + "s");
				}
				Thread.Sleep(150);
			}
			errors = "Нет соединения с бд";
			return null;
		}

		public void CreateGameSaveInfo(GameSaveInfo gameSave)
		{
			if(_activeUser == null)
			{
				Debug.LogError("No active user");
			}
			else
			{
				_activeUser.AddGameSave(gameSave);
				TryGetCollection<User>(out _).FindOneAndReplace(x => x.Id == _activeUser.Id, _activeUser);
			}
		}

		public bool TryLogin(string login, string password, out string errors)
		{
			var users = TryGetCollection<User>(out errors);
			if(users == null)
			{
				return false;
			}
			var user = users.Find(x => x.Login == login).SingleOrDefault();
			if (user == null)
			{
				errors = "Нет пользователя с таким логином";
				Debug.LogWarning("Login: " + login);
			}
			else if (user.Password != password)
			{
				errors = "Неверный пароль";
			}
			else
			{
				errors = null;
				_activeUser = user;
				return true;
			}
			return false;
		}

		public bool TryRegistrate(string login, string password, out string errors)
		{
			var collection = TryGetCollection<User>(out errors);
			if(collection == null)
			{
				return false;
			}
			var result = collection.Find(x => x.Login == login);
			if (result != null && result.Any())
			{
				errors = "Пользователь с таким логином уже существует";
				return false;
			}
			else
			{
				errors = null;
				User user = new User() { Login = login, Password = password };
				collection.InsertOne(user);
				_activeUser = collection.Find(x => x.Login == login).Single();
				return true;
			}
		}

		public GameSaveInfo GetLastGameSave()
		{
			return _activeUser.GameSaves[_activeUser.GameSaves.Count - 1];
		}

		public List<GameSaveInfo> GetAllGameSaves()
		{
			return _activeUser.GameSaves;
		}
	}
}