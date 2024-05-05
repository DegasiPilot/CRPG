using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public RaceInfo[] RaceInfos;
    public List<Item> AllItems;
    public GameObject Inventory;
    public PlayerCustomizer PlayerCustomizer;
    public PlayerController PlayerController;

    private void Awake()
    {
        GameData.RaceInfos = RaceInfos;
        DontDestroyOnLoad(Inventory);
        DontDestroyOnLoad(PlayerCustomizer.gameObject);
        PlayerController.Inventory = Inventory;
        GameData.PlayerCustomizer = PlayerCustomizer;
        GameData.PlayerController = PlayerController;
        PlayerController.Setup();
    }

    public void LoadLastGameSave()
    {
        GameSaveInfo gameSave = CRUD.GetLastGameSave(GameData.CurrentUser);
        LoadGameSave(gameSave);
    }

    public void LoadGameSave(GameSaveInfo gameSave)
    {
        GameData.PlayerPersonageInfo = CRUD.GetPersonageInfo(gameSave.MainPersonageId);
        PlayerController.Personage.Setup(GameData.PlayerPersonageInfo);
        PlayerCustomizer.ApplyAppearance();
        GameData.SceneSaveInfo = gameSave.SceneSaveInfo;
        if (gameSave.InventoryItems != null)
        {
            GameData.Inventory = new List<Item>(gameSave.InventoryItems.Length);
            for (int i = 0; i < gameSave.InventoryItems.Length; i++)
            {
                Item item = AllItems.First(item => item.ItemInfo.Id == gameSave.InventoryItems[i].ItemId);
                item = Instantiate(item, Inventory.transform);
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