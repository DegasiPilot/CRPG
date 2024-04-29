using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

internal static class CRUD
{
    const string connectionUri = "mongodb://localhost:27017";
    const string DbName = "CRPGDb";
    static MongoClient client = new MongoClient(connectionUri);

    private static IMongoCollection<T> GetCollection<T>()
    {
        var database = client.GetDatabase(DbName);
        return database.GetCollection<T>(typeof(T).Name + "s");
    }

    public static void CreatePersonageInfo(PersonageInfo personage)
    {
        if (personage.PersonagePortrait != null)
        {
            personage.ImageBytes = personage.PersonagePortrait.EncodeToPNG();
        }
        GetCollection<PersonageInfo>().InsertOne(personage);
    }

    public static bool IsExistPersonageInfo(PersonageInfo p)
    {
        return GetCollection<PersonageInfo>().Find(x => x.Id == p.Id).CountDocuments() > 0;
    }

    public static void RedactPersonageInfo(PersonageInfo personage)
    {
        personage.ImageBytes = ImageConversion.EncodeToPNG(personage.PersonagePortrait);
        GetCollection<PersonageInfo>().ReplaceOne(x => x.Id == personage.Id, personage);
    }

    public static PersonageInfo GetPersonageInfo(ObjectId Id)
    {
        PersonageInfo personageInfo = GetCollection<PersonageInfo>().Find(x => x.Id == Id).FirstOrDefault();
        personageInfo.PersonagePortrait = new Texture2D(256, 256);
        if (personageInfo.ImageBytes != null)
        {
            personageInfo.PersonagePortrait.LoadImage(personageInfo.ImageBytes);
        }
        return personageInfo;
    }

    public static PersonageInfo GetPersonageInfo(string name)
    {
        PersonageInfo personageInfo = GetCollection<PersonageInfo>().Find(x => x.Name == name).FirstOrDefault();
        personageInfo.PersonagePortrait ??= new Texture2D(1, 1);
        if (personageInfo.ImageBytes != null)
        {
            personageInfo.PersonagePortrait.LoadImage(personageInfo.ImageBytes);
        }
        return personageInfo;
    }

    public static bool TryDeletePersonageInfo(ObjectId id)
    {
        return GetCollection<PersonageInfo>().DeleteOne(x => x.Id == id).DeletedCount > 0;
    }

    public static void CreateGameSaveInfo(GameSaveInfo gameSave)
    {
        GetCollection<GameSaveInfo>().InsertOne(gameSave);
    }

    public static GameSaveInfo GetLastGameSave(User user)
    {
        return GetCollection<GameSaveInfo>().AsQueryable().Where(x => x.UserLogin == user.Login).OrderByDescending(x => x.DateTime).First();
    }

    public static List<GameSaveInfo> GetAllGameSaves(User user)
    {
        return GetCollection<GameSaveInfo>().AsQueryable().Where(x => x.UserLogin == user.Login).ToList();
    }

    public static bool HasAnySaves(User user)
    {
        return GetCollection<GameSaveInfo>().AsQueryable().Where(x => x.UserLogin == user.Login).Any();
    }

    public static void CreateUser(User user)
    {
        GetCollection<User>().InsertOne(user);
    }

    public static Task<User> GetUserWithLoginAsync(string login)
    {
        return GetCollection<User>().AsQueryable().FirstOrDefaultAsync(x => x.Login == login);
    }
}