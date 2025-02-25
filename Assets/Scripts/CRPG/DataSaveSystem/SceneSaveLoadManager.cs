using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

internal class SceneSaveLoadManager : MonoBehaviour
{
    public static SceneSaveLoadManager Instance;

    [NonSerialized] public static UnityEvent OnSceneLoaded = new();

    public List<SaveableGameobject> ObjectsToSave;
    public Vector3 StartPlayerPosition;
    public Vector3 StartPlayerRotation;
    public Transform CameraCenter;

    private void Awake()
    {
        Instance = this;   
    }

    public void LoadSceneFromSave(SceneSaveInfo sceneSaveInfo)
    {
        OnSceneLoaded.Invoke();
        if (sceneSaveInfo == null)
        {
            GameData.PlayerController.SetPositonAndRotation(StartPlayerPosition, StartPlayerRotation);
            return;
        }

        if (sceneSaveInfo.SaveObjectInfos != null)
        {
            SaveObjectInfo[] objectInfos = sceneSaveInfo.SaveObjectInfos;
            for (int i = 0; i < Math.Min(ObjectsToSave.Count, objectInfos.Length); i++)
            {
                if (objectInfos[i] == null)
                {
                    Destroy(ObjectsToSave[i]);
                }
                else
                {
                    ObjectsToSave[i].LoadSaveInfo(objectInfos[i]);
                }
            }
        }
        GameData.PlayerController.SetPositonAndRotation(sceneSaveInfo.PlayerPos, sceneSaveInfo.PlayerRot);
        Vector3 cameraPos = sceneSaveInfo.PlayerPos;
        cameraPos.y = 0;
        CameraCenter.position = cameraPos;
    }

    public void SaveScene()
    {
        GameData.SceneSaveInfo = GetSceneSave();
    }

    public SceneSaveInfo GetSceneSave()
    {
        SceneSaveInfo sceneInfo = new SceneSaveInfo();
        sceneInfo.SaveObjectInfos = new SaveObjectInfo[ObjectsToSave.Count];
        for (int i = 0; i < ObjectsToSave.Count; i++)
        {
            if (ObjectsToSave[i] == null)
            {
                sceneInfo.SaveObjectInfos[i] = null;
            }
            else
            {
                sceneInfo.SaveObjectInfos[i] = ObjectsToSave[i].GetSaveInfo();
            }
        }
        sceneInfo.PlayerPos = GameData.PlayerController.transform.position;
        sceneInfo.PlayerRot = GameData.PlayerController.transform.eulerAngles;
        return sceneInfo;
    }
}