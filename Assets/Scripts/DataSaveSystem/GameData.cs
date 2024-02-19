using System;
using MongoDB.Bson;

public static class GameData
{
    public static PersonageInfo PlayerPersonage;

    public static void NewGameSave()
    {
        GameSaveInfo gameSave = new GameSaveInfo()
        {
            DateTime = BsonDateTime.Create(DateTime.Now),
            MainPersonageId = PlayerPersonage.Id
        };

        CRUD.CreateGameSaveInfo(gameSave);
    }

    public static void LoadLastGameSave()
    {
        GameSaveInfo gameSave = CRUD.GetLastGameSave();
        PlayerPersonage = CRUD.GetPersonageInfo(gameSave.MainPersonageId);
    }
}
