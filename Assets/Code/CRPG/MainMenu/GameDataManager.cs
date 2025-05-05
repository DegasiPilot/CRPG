using CRPG;
using CRPG.DataSaveSystem;
using CRPG.DataSaveSystem.SaveData;
using CRPG.ItemSystem;
using System.Collections.Generic;

public class GameDataManager
{
    private MainPlayer _playerPrefab;
	private GlobalDataManager _globalDataManager;

    internal GameDataManager(MainPlayer playerPrefab, GlobalDataManager globalDataManager)
    {
		_playerPrefab = playerPrefab;
		_globalDataManager = globalDataManager;
		if(GameData.MainPlayer != null)
		{
			UnityEngine.Object.Destroy(GameData.MainPlayer.gameObject);
		}
		if (GameData.Companions != null)
		{
			foreach(var companion in GameData.Companions)
			{
				UnityEngine.Object.Destroy(companion.gameObject);
			}
		}
	}

	public void InitNewGame()
	{
		var player = UnityEngine.Object.Instantiate(_playerPrefab);
		InitPlayer(player);
	}

    public void LoadGameSave(GameSaveInfo gameSave)
    {
		var player = UnityEngine.Object.Instantiate(_playerPrefab,
			gameSave.MainPlayerInfo.Position,
			gameSave.MainPlayerInfo.Rotation);
		InitPlayer(player);
		player.PlayerController.Personage.PersonageInfo.Setup(gameSave.MainPersonageInfo, _globalDataManager.GetRaceInfo);
		player.PlayerController.Personage.Setup(gameSave.MainPlayerInfo.PersonageSaveInfo);
		player.PlayerCustomizer.ApplyAppearance(gameSave.MainPersonageAppearance);
		LoadEquipedItems(gameSave.MainPlayerInfo.EquipedItems, player.PlayerController.Personage.EquipmentManager);
		GameData.SceneSaveInfo = gameSave.SceneSaveInfo;

        if (gameSave.InventoryItems != null)
        {
            LoadInventory(gameSave.InventoryItems, player.PlayerController.Inventory.transform);
        }

		if (gameSave.CompanionsInfo != null)
		{
			LoadCompanions(gameSave.CompanionsInfo);
		}
	}

	private void InitPlayer(MainPlayer player)
	{
		UnityEngine.Object.DontDestroyOnLoad(player);
		GameData.MainPlayer = player;
	}

    private void LoadInventory(string[] inventoryItems, UnityEngine.Transform inventory)
    {
		GameData.Inventory = new List<Item>(inventoryItems.Length);
		for (int i = 0; i < inventoryItems.Length; i++)
		{
			Item item = _globalDataManager.GetClone(inventoryItems[i]).GetComponent<Item>();
			item.transform.SetParent(inventory);
			item.gameObject.SetActive(false);
			item.OnTaked();
			GameData.Inventory.Add(item);
		}
	}

	private void LoadCompanions(PlayerSaveInfo[] playerInfos)
	{
		GameData.Companions = new List<PlayerController>(playerInfos.Length);
		for (int i = 0; i < playerInfos.Length; i++)
		{
			PlayerController companion = _globalDataManager.GetClone(playerInfos[i].UniqueName).GetComponent<PlayerController>();
			UnityEngine.Object.DontDestroyOnLoad(companion);
			companion.transform.SetPositionAndRotation(playerInfos[i].Position, playerInfos[i].Rotation);
			companion.Personage.Setup(playerInfos[i].PersonageSaveInfo);
			GameData.Companions.Add(companion);
			LoadEquipedItems(playerInfos[i].EquipedItems, companion.Personage.EquipmentManager);
		}
	}

	private void LoadEquipedItems(string[] itemsNames, EquipmentManager equipmentManager)
	{
		if (itemsNames != null && itemsNames.Length > 0)
		{
			EquipableItem[] items = new EquipableItem[itemsNames.Length];
			for (int i = 0; i < items.Length; i++)
			{
				items[i] = _globalDataManager.GetClone(itemsNames[i]).GetComponent<EquipableItem>();
				items[i].OnTaked();
			}
			equipmentManager.Setup(items);
		}
	}
}