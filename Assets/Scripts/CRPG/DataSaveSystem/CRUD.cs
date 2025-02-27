using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Linq.Expressions;

namespace CRPG.DataSaveSystem
{
	internal class CRUD : IDataSaveLoader
	{
		const string connectionUri = "mongodb://localhost:27017";
		const string DbName = "CRPGDb";
		MongoClient client = new MongoClient(connectionUri);

		private IMongoCollection<T> GetCollection<T>()
		{
			var database = client.GetDatabase(DbName);
			return database.GetCollection<T>(typeof(T).Name + "s");
		}

		public void CreatePersonageInfo(PersonageInfo personage)
		{
			//if (personage.PersonagePortrait != null)
			//{
			//	personage.ImageBytes = personage.PersonagePortrait.EncodeToPNG();
			//}
			GetCollection<PersonageInfo>().InsertOne(personage);
		}

		public bool IsExistPersonageInfo(PersonageInfo p)
		{
			return GetCollection<PersonageInfo>().Find(x => x.Id == p.Id).CountDocuments() > 0;
		}

		public void RedactPersonageInfo(PersonageInfo personage)
		{
			//personage.ImageBytes = ImageConversion.EncodeToPNG(personage.PersonagePortrait);
			GetCollection<PersonageInfo>().ReplaceOne(x => x.Id == personage.Id, personage);
		}

		public bool TryDeletePersonageInfo(ObjectId id)
		{
			return GetCollection<PersonageInfo>().DeleteOne(x => x.Id == id).DeletedCount > 0;
		}

		public void CreateGameSaveInfo(GameSaveInfo gameSave)
		{
			GetCollection<GameSaveInfo>().InsertOne(gameSave);
		}

		public GameSaveInfo GetLastGameSave(User user)
		{
			return GetCollection<GameSaveInfo>().AsQueryable().Where(x => x.UserLogin == user.Login).OrderByDescending(x => x.DateTime).First();
		}

		public List<GameSaveInfo> GetAllGameSaves(User user)
		{
			return GetCollection<GameSaveInfo>().AsQueryable().Where(x => x.UserLogin == user.Login).ToList();
		}

		public bool HasAnySaves(User user)
		{
			return GetCollection<GameSaveInfo>().AsQueryable().Where(x => x.UserLogin == user.Login).Any();
		}

		public void CreateUser(User user)
		{
			GetCollection<User>().InsertOne(user);
		}

		public Task<User> GetUserWithLoginAsync(string login)
		{
			return GetCollection<User>().AsQueryable().FirstOrDefaultAsync(x => x.Login == login);
		}
	}
}