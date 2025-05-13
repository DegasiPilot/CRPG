using CRPG.Customization;
using CRPG.DataSaveSystem.SaveData;
using CRPG.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRPG.DataSaveSystem
{
	/// <summary>
	/// Текущие данные, используются для загрузки/сохранения
	/// </summary>
	public static class GameData
	{
		public static SceneSaveInfo SceneSaveInfo;
		public static List<Item> Inventory = new List<Item>();
		internal static MainPlayer MainPlayer;
		internal static List<PlayerController> Companions = new List<PlayerController>();
		internal static PlayerController ActivePlayer;
		internal static AppearanceStruct MainPersonageAppearance { get; set; }

		internal static GameSaveInfo NewGameSave()
		{
			PersonageSaveInfo mainPersonageSaveInfo;
			if (MainPlayer.PlayerController.Personage.Save() is PersonageSaveInfo personageSaveInfo)
			{
				mainPersonageSaveInfo = personageSaveInfo;
			}
			else
			{
				throw new Exception("Personage.Save() return not PersonageSaveInfo");
			}
			IEnumerable<Item> notEquipedItems = Inventory.
				Where(x => !(x is EquipableItem equipableItem) || !equipableItem.IsEquiped);

			GameSaveInfo gameSave = new GameSaveInfo()
			{
				DateTime = DateTime.Now,
				MainPlayerInfo = new PlayerSaveInfo()
				{
					UniqueName = "MainPlayer",
					Position = MainPlayer.transform.position,
					Rotation = MainPlayer.transform.rotation,
					EquipedItems = ItemNames(MainPlayer.PlayerController.Personage.EquipmentManager.EquipableItems),
					PersonageSaveInfo = mainPersonageSaveInfo,
				},
				MainPersonageInfo = MainPlayer.PlayerController.Personage.PersonageInfo.Save(),
				MainPersonageAppearance = MainPersonageAppearance,
				SceneSaveInfo = SceneSaveInfo,
				InventoryItems = ItemNames(notEquipedItems),
				CompanionsInfo = SaveCompanions(),
			};

			return gameSave;
		}

		private static PlayerSaveInfo[] SaveCompanions()
		{
			if (Companions.Count <= 0)
			{
				return null;
			}
			var CompanionsInfo = new PlayerSaveInfo[Companions.Count];
			for (int i = 0; i < CompanionsInfo.Length; i++)
			{
				PersonageSaveInfo personageSaveInfo;
				if (Companions[i].Personage.Save() is PersonageSaveInfo saveInfo)
				{
					personageSaveInfo = saveInfo;
				}
				else
				{
					throw new Exception("Personage.Save() return not PersonageSaveInfo");
				}
				CompanionsInfo[i] = new PlayerSaveInfo()
				{
					UniqueName = Companions[i].GetComponent<SaveableGameobject>().UniqueName,
					Position = Companions[i].transform.position,
					Rotation = Companions[i].transform.rotation,
					EquipedItems = ItemNames(Companions[i].Personage.EquipmentManager.EquipableItems),
					PersonageSaveInfo = personageSaveInfo,
				};
			}
			return CompanionsInfo;
		}

		public static void InitializeNewGame(PersonageInfo personageInfo)
		{
			MainPlayer.PlayerController.Personage.PersonageInfo = personageInfo;
			MainPlayer.PlayerController.Personage.Setup();
			Inventory = new List<Item>();
		}

		private static string[] ItemNames(IEnumerable<Item> items)
		{
			int count = items.Count();
			if (count > 0)
			{
				string[] itemNames = new string[count];
				int i = 0;
				foreach (var item in items)
				{
					itemNames[i] = item.GetComponent<SaveableGameobject>().UniqueName;
				}
				return itemNames;
			}
			else
			{
				return null;
			}
		}
	}
}