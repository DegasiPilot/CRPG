using MongoDB.Bson;
using System;
using System.Collections.Generic;

public static class GameData
{
    public static PersonageInfo PlayerPersonageInfo;
    public static SceneSaveInfo SceneSaveInfo;
    public static List<Item> Inventory;
    public static RaceInfo[] RaceInfos;
    public static PlayerCustomizer PlayerCustomizer;
    public static PlayerController PlayerController;

    public static User CurrentUser;

    public const float MaxUnarmedAttackDistance = 0.8f;
    public const float MaxJumpDistance = 10;

    public static void NewGameSave()
    {
        GameSaveInfo gameSave = new GameSaveInfo()
        {
            DateTime = BsonDateTime.Create(DateTime.Now),
            MainPersonageId = PlayerPersonageInfo.Id,
            SceneSaveInfo = SceneSaveInfo,
        };

        if (Inventory != null && Inventory.Count > 0)
        {
            gameSave.InventoryItems = new GameSaveInfo.InventoryItem[Inventory.Count];
            for (int i = 0; i < Inventory.Count; i++)
            {
                gameSave.InventoryItems[i] = new GameSaveInfo.InventoryItem()
                {
                    ItemId = Inventory[i].ItemInfo.Id,
                    IsEquiped = Inventory[i].IsEquiped,
                };
            }
        }
        gameSave.UserLogin = CurrentUser.Login;

        CRUD.CreateGameSaveInfo(gameSave);
    }

    public static void InitializeNewGame(PersonageInfo personageInfo)
    {
        PlayerPersonageInfo = personageInfo;
        PlayerController.Personage.Setup(PlayerPersonageInfo);
        Inventory = new List<Item>();
        PlayerCustomizer.ApplyAppearance();
    }
}