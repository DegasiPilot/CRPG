using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataManager
{
    private IEnumerable<Item> _allItems;
    private PlayerController _playerController;
    private PlayerCustomizer _playerCustomizer;

    public GameDataManager(PlayerController playerController, PlayerCustomizer playerCustomizer, IEnumerable<Item> AllItems)
    {
		_playerController = playerController;
        _playerCustomizer = playerCustomizer;
        _allItems = AllItems;
	}

    public void LoadGameSave(GameSaveInfo gameSave)
    {
        gameSave.MainPersonageInfo.Setup(GameData.GetRaceInfo);
        _playerController.Personage.Setup(gameSave.MainPersonageInfo);
        _playerCustomizer.ApplyAppearance(gameSave.MainPersonageInfo.Appearance);
        GameData.SceneSaveInfo = gameSave.SceneSaveInfo;
        if (gameSave.InventoryItems != null)
        {
            GameData.Inventory = new List<Item>(gameSave.InventoryItems.Length);
            for (int i = 0; i < gameSave.InventoryItems.Length; i++)
            {
                Item item = _allItems.First(item => item.ItemInfo.Name == gameSave.InventoryItems[i].ItemName);
                item = Object.Instantiate(item, _playerController.Inventory.transform);
                item.IsInInventory = true;
                item.IsEquiped = gameSave.InventoryItems[i].IsEquiped;
                item.gameObject.SetActive(false);
                item.OnTaked();
                GameData.Inventory.Add(item);
            }
        }
        else
        {
            GameData.Inventory = new List<Item>();
        }
    }
}