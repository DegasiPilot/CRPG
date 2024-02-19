using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System.Collections.Generic;

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
        GetCollection<PersonageInfo>().InsertOne(personage);
    }

    public static bool IsExistPersonageInfo(PersonageInfo p)
    {
        return GetCollection<PersonageInfo>().Find(x => x.Id == p.Id).CountDocuments() > 0;
    }

    public static void RedactPersonageInfo(PersonageInfo personage)
    {
        GetCollection<PersonageInfo>().ReplaceOne(x => x.Id == personage.Id, personage);
    }

    public static PersonageInfo GetPersonageInfo(ObjectId Id)
    {
        return GetCollection<PersonageInfo>().Find(x => x.Id == Id).FirstOrDefault();
    }

    public static PersonageInfo GetPersonageInfo(string name)
    {
        return GetCollection<PersonageInfo>().Find(x => x.Name == name).FirstOrDefault();
    }

    public static bool TryDeletePersonageInfo(ObjectId id)
    {
        return GetCollection<PersonageInfo>().DeleteOne(x => x.Id == id).DeletedCount > 0;
    }

    public static void CreateGameSaveInfo(GameSaveInfo gameSave)
    {
        GetCollection<GameSaveInfo>().InsertOne(gameSave);
    }

    public static GameSaveInfo GetLastGameSave()
    {
        return GetCollection<GameSaveInfo>().AsQueryable().OrderByDescending(x => x.DateTime).First();
    }

    public static List<GameSaveInfo> GetAllGameSaves()
    {
        return GetCollection<GameSaveInfo>().AsQueryable().ToList();
    }
}