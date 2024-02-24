using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class SceneSaveLoadManager : MonoBehaviour
{
    public static SceneSaveLoadManager Instance;

    public List<ItemInfo> Itemsinfos;
    public List<SaveableGameobject> ObjectsToSave;

    private void Awake()
    {
        Instance = this;   
    }

    public void LoadSceneFromSave(SceneSaveInfo sceneSaveInfo)
    {
        if(sceneSaveInfo == null || sceneSaveInfo.saveObjectInfos == null)
        {
            return;
        }

        List<SaveObjectInfo> objectInfos = sceneSaveInfo.saveObjectInfos;
        for (int i = 0; i < Math.Min(ObjectsToSave.Count, objectInfos.Count); i++)
        {
            ObjectsToSave[i].LoadSaveInfo(objectInfos[i]);
        }

        List<Item> ItemsInLevel = new ();
        foreach(SaveableGameobject SGO in ObjectsToSave)
        {
            if(SGO.TryGetComponent(out Item item))
            {
                ItemsInLevel.Add(item);
            }
        }

        for (int i = 0; i < GameData.InventoryAsNames.Length; i++)
        {
            string itemName = GameData.InventoryAsNames[i];
            if (ItemsInLevel.Exists(x => x.IsInInventory && x.ItemInfo.Name == itemName))
            {
                GameData.Inventory.Add(ItemsInLevel.First(x => x.IsInInventory && x.ItemInfo.Name == itemName));
            }
            else
            {
                Instantiate(Itemsinfos.First(x => x.Name == itemName).Prefab);
            }
        }
    }

    public void SaveScene()
    {
        GameData.SceneSaveInfo = GetSceneSave();
    }

    public SceneSaveInfo GetSceneSave()
    {
        SceneSaveInfo sceneInfo = new SceneSaveInfo();
        sceneInfo.saveObjectInfos = new List<SaveObjectInfo>(ObjectsToSave.Count);
        for (int i = 0; i < ObjectsToSave.Count; i++)
        {
            sceneInfo.saveObjectInfos.Add(ObjectsToSave[i].GetSaveInfo());
        }
        return sceneInfo;
    }
}