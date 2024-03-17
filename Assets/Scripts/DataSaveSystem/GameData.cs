using System;
using System.Collections.Generic;
using MongoDB.Bson;
using UnityEngine;

public static class GameData
{
    public static PersonageInfo PlayerPersonage;
    public static SceneSaveInfo SceneSaveInfo;
    public static List<Item> Inventory = new List<Item>();
    public static string[] InventoryAsNames { get; private set; }

    public static RaceInfo[] RaceInfos;

    public static void NewGameSave()
    {
        GameSaveInfo gameSave = new GameSaveInfo()
        {
            DateTime = BsonDateTime.Create(DateTime.Now),
            MainPersonageId = PlayerPersonage.Id,
            SceneSaveInfo = SceneSaveInfo,
        };

        gameSave.InventoryItemsNames = new string[Inventory.Count];
        for(int i = 0; i < Inventory.Count; i++)
        {
            gameSave.InventoryItemsNames[i] = Inventory[i].ItemInfo.Name;
        }

        CRUD.CreateGameSaveInfo(gameSave);
    }

    public static void LoadLastGameSave()
    {
        GameSaveInfo gameSave = CRUD.GetLastGameSave();
        LoadGameSave(gameSave);
    }

    public static void LoadGameSave(GameSaveInfo gameSave)
    {
        PlayerPersonage = CRUD.GetPersonageInfo(gameSave.MainPersonageId);
        SceneSaveInfo = gameSave.SceneSaveInfo;
        InventoryAsNames = new string[gameSave.InventoryItemsNames.Length];
        for (int i = 0; i < gameSave.InventoryItemsNames.Length; i++)
        {
            InventoryAsNames[i] = gameSave.InventoryItemsNames[i];
        }
        Inventory = new List<Item>(gameSave.InventoryItemsNames.Length);
    }
}