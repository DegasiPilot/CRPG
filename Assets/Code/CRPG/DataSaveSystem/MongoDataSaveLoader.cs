using CRPG.DataSaveSystem.SaveData;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CRPG.DataSaveSystem
{
	internal class MongoDataSaveLoader : IDataSaveLoader
	{
		public bool IsUserLogined => _activeUser != null;
		public string UserLogin => IsUserLogined ? _activeUser.Login : "Неавторизованный пользователь";
		public bool CanExit => true;
		public bool HasSaves => _activeUser?.GameSaves != null && _activeUser.GameSaves.Count > 0;

		const string connectionUri = "mongodb://Timur:TimMongoCRPG@217.199.252.220:27017/";
		const string DbName = "CRPG_DB";

		readonly static MongoClientSettings MongoSettings = new MongoClientSettings()
		{
			Server = new MongoServerAddress("217.199.252.220", 27017),
			Credential = MongoCredential.CreateCredential("admin", "Timur", "TimMongoCRPG"),
			IPv6 = true,
		};

		MongoClient client = new MongoClient(MongoSettings);
		private User _activeUser;

		private static MongoDataSaveLoader _instance;
		public static MongoDataSaveLoader Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MongoDataSaveLoader();
				}
				return _instance;
			}

		}

		private MongoDataSaveLoader()
		{
			var objectSerializer = new ObjectSerializer(ObjectSerializer.AllAllowedTypes);
			BsonSerializer.RegisterSerializer(objectSerializer);
			if (!BsonClassMap.IsClassMapRegistered(typeof(PersonageSaveInfo)))
			{
				BsonClassMap.RegisterClassMap<PersonageSaveInfo>();
				BsonClassMap.RegisterClassMap<DialogueActorSaveData>();
			}
		}

		public bool Ping()
		{
			try
			{
				var client = new MongoClient(MongoSettings);
				var db = client.GetDatabase(DbName);
				var collections = db.ListCollectionNames().ToList();
				return true;
			}
			catch(System.Exception e)
			{
				Debug.LogWarning("Mongo:" + e.Message);
			}
			Thread.Sleep(150);
			Debug.Log("No db connection");
			return false;
		}

		private IMongoCollection<T> TryGetCollection<T>(out string errors)
		{
			var database = client.GetDatabase(DbName);
			if (Ping())
			{
				errors = string.Empty;
				return database.GetCollection<T>(typeof(T).Name + "s");
			}
			else
			{
				errors = "Нет соединения с бд";
				return null;
			}
		}

		public void CreateGameSaveInfo(GameSaveInfo gameSave)
		{
			if (_activeUser == null)
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
			if (users == null)
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
			if (collection == null)
			{
				return false;
			}
			login = login.Trim().ToLower();
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

		public void ExitFromAccount()
		{
			_activeUser = null;
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