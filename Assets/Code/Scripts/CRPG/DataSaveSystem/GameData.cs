using CRPG;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameData
{
    public static SceneSaveInfo SceneSaveInfo;
    public static List<Item> Inventory;
    public static RaceInfo[] RaceInfos { private get; set; }
    internal static PersonageActionInfo[] PersonageActionInfos { private get; set; }
    internal static Player Player;

    public const float MaxUnarmedAttackDistance = 0.8f;
    public const float MaxJumpDistance = 5;
	public const int MinUnarmedAttackEnergy = 1;
	public const int MaxUnarmedAttackEnergy = 4;
	public const DamageType UnarmedDamageType = DamageType.Physical;

    public const float DefaultUIInterpolationSpeed = 2f;

    public static GameSaveInfo NewGameSave(PersonageInfo playerPersonageInfo)
    {
        GameSaveInfo gameSave = new GameSaveInfo()
        {
            DateTime = DateTime.Now,
            MainPersonageInfo = UnityEngine.Object.Instantiate(playerPersonageInfo), //clone
            SceneSaveInfo = SceneSaveInfo,
        };

        if (Inventory != null && Inventory.Count > 0)
        {
            gameSave.InventoryItems = new GameSaveInfo.InventoryItem[Inventory.Count];
            for (int i = 0; i < Inventory.Count; i++)
            {
                gameSave.InventoryItems[i] = new GameSaveInfo.InventoryItem()
                {
                    ItemName = Inventory[i].ItemInfo.Name,
                    IsEquiped = Inventory[i].IsEquiped,
                };
            }
        }
        return gameSave;
    }

    public static void InitializeNewGame(PersonageInfo personageInfo)
    {
        Player.PlayerController.Personage.Setup(personageInfo);
        Inventory = new List<Item>();
    }

    public static void ClearData()
    {
        Player.PlayerController.ResetPlayer();
	}

    internal static PersonageActionInfo GetActionInfo(ActionType actionType)
    {
        return PersonageActionInfos.First(x => x.ActionType == actionType);
    }

	internal static RaceInfo GetRaceInfo(Race race)
	{
		return RaceInfos.First(x => x.Race == race);
	}
}