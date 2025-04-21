using CRPG.Customization;
using CRPG.DataSaveSystem.SaveData;
using CRPG.ItemSystem;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CRPG.DataSaveSystem
{
	/// <summary>
	/// Текущие данные, все что нужно для загрузки/сохранения
	/// </summary>
	public static class GameData
    {
        public static SceneSaveInfo SceneSaveInfo;
        public static List<Item> Inventory = new List<Item>();
        internal static MainPlayer MainPlayer;
        internal static List<Player> Companions = new List<Player>();
        internal static Player ActivePlayer;
        internal static AppearanceStruct MainPersonageAppearance { get; set; }

        public const float MaxUnarmedAttackDistance = 0.8f;
        public const float MaxJumpDistance = 5;
        public const int MinUnarmedAttackEnergy = 1;
        public const int MaxUnarmedAttackEnergy = 4;
        public const DamageType UnarmedDamageType = DamageType.Physical;

        public const float DefaultUIInterpolationSpeed = 2f;

        internal static GameSaveInfo NewGameSave()
        {
            PersonageSaveInfo mainPersonageSaveInfo; 
            if(MainPlayer.PlayerController.Personage.Save() is PersonageSaveInfo personageSaveInfo)
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
					PersonageInfo = Clone(MainPlayer.PlayerController.Personage.PersonageInfo),
					Position = MainPlayer.transform.position,
				    Rotation = MainPlayer.transform.rotation,
                    EquipedItems = ItemNames(MainPlayer.EquipmentManager.EquipableItems),
                    PersonageSaveInfo = mainPersonageSaveInfo,
				},
                MainPersonageAppearance = MainPersonageAppearance,
                SceneSaveInfo = SceneSaveInfo,
				InventoryItems = ItemNames(notEquipedItems),
                CompanionsInfo = SaveCompanions(),
			};

            return gameSave;
        }

        private static PlayerSaveInfo[] SaveCompanions()
        {
            if(Companions.Count <= 0)
            {
                return null;
            }
			var CompanionsInfo = new PlayerSaveInfo[Companions.Count];
			for (int i = 0; i < CompanionsInfo.Length; i++)
			{
                PersonageSaveInfo personageSaveInfo;
				if (Companions[i].PlayerController.Personage.Save() is PersonageSaveInfo saveInfo)
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
					PersonageInfo = Clone(Companions[i].PlayerController.Personage.PersonageInfo),
					Position = Companions[i].transform.position,
					Rotation = Companions[i].transform.rotation,
					EquipedItems = ItemNames(Companions[i].EquipmentManager.EquipableItems),
                    PersonageSaveInfo = personageSaveInfo,
				};
			}
            return CompanionsInfo;
		}

        public static void InitializeNewGame(PersonageInfo personageInfo)
        {
            MainPlayer.PlayerController.Personage.Setup(personageInfo);
            Inventory = new List<Item>();
        }

        private static T Clone<T>(T original) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(original);
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