using MongoDB.Bson;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static PersonageInfo PlayerPersonageInfo;
    public static SceneSaveInfo SceneSaveInfo;
    public static List<Item> Inventory;
    public static RaceInfo[] RaceInfos;
    public static PlayerCustomizer PlayerCustomizer;
    public static PlayerController PlayerController;

    public const float MaxUnarmedAttackDistance = 2;

    public static void NewGameSave()
    {
        GameSaveInfo gameSave = new GameSaveInfo()
        {
            DateTime = BsonDateTime.Create(DateTime.Now),
            MainPersonageId = PlayerPersonageInfo.Id,
            SceneSaveInfo = SceneSaveInfo,
        };

        if (Inventory != null)
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

        CRUD.CreateGameSaveInfo(gameSave);
    }
}