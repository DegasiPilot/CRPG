using CRPG;
using CRPG.DataSaveSystem;
using CRPG.DataSaveSystem.SaveData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

internal class SceneSaveLoadManager : MonoBehaviour
{
    [NonSerialized] public static UnityEvent OnSceneLoaded = new();

    public List<SaveableGameobject> ObjectsToSave;
    public Vector3 StartPlayerRotation;
    public Transform CameraCenter;

    public void LoadSceneFromSave(SceneSaveInfo sceneSaveInfo, GlobalDataManager globalDataManager)
    {
        OnSceneLoaded.Invoke();
        if (sceneSaveInfo == null)
        {
            GameData.MainPlayer.PlayerController.SetPositonAndRotation(CameraCenter.position, StartPlayerRotation);
            Debug.Log("Loaded empty game save", this);
            return;
        }

		if (sceneSaveInfo.SaveObjectInfos != null)
        {
			foreach (var item in ObjectsToSave)
			{
				Destroy(item.gameObject);
			}
			ObjectsToSave.Clear();
            SaveObjectInfo[] objectInfos = sceneSaveInfo.SaveObjectInfos;
            for (int i = 0; i < objectInfos.Length; i++)
            {
                var saveableObject = globalDataManager.GetClone(objectInfos[i].Name).GetComponent<SaveableGameobject>();
                ObjectsToSave.Add(saveableObject);
                saveableObject.LoadSaveInfo(objectInfos[i]);
			}
        }
        Vector3 cameraPos = GameData.MainPlayer.transform.position;
        cameraPos.y = 0;
        CameraCenter.position = cameraPos;
    }

    public SceneSaveInfo GetSceneSave(GlobalDataManager globalDataManager)
    {
        SceneSaveInfo sceneInfo = new SceneSaveInfo();

        List<SaveObjectInfo> saveObjectInfos = new(ObjectsToSave.Count);
        for (int i = 0; i < ObjectsToSave.Count; i++)
        {
            if (ObjectsToSave[i] != null)
            {
                var info = ObjectsToSave[i].GetSaveInfo(globalDataManager);
                if(info != null)
                {
                    saveObjectInfos.Add(info);
				}
            }
        }
		sceneInfo.SaveObjectInfos = saveObjectInfos.ToArray();
        return sceneInfo;
    }
}