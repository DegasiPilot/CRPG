using System;
using System.Collections.Generic;
using UnityEngine;

internal class SceneSaveLoadManager : MonoBehaviour
{
    public static SceneSaveLoadManager Instance;

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
        if(sceneSaveInfo == null)
        {
            GameData.PlayerController.transform.position = StartPlayerPosition;
            GameData.PlayerController.transform.eulerAngles = StartPlayerRotation;
            return;
        }

        if (sceneSaveInfo.saveObjectInfos != null)
        {
            SaveObjectInfo[] objectInfos = sceneSaveInfo.saveObjectInfos;
            for (int i = 0; i < Math.Min(ObjectsToSave.Count, objectInfos.Length); i++)
            {
                ObjectsToSave[i].LoadSaveInfo(objectInfos[i]);
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
        sceneInfo.saveObjectInfos = new SaveObjectInfo[ObjectsToSave.Count];
        for (int i = 0; i < ObjectsToSave.Count; i++)
        {
            sceneInfo.saveObjectInfos[i] = ObjectsToSave[i].GetSaveInfo();
        }
        sceneInfo.PlayerPos = GameData.PlayerController.transform.position + Vector3.up;
        sceneInfo.PlayerRot = GameData.PlayerController.transform.eulerAngles;
        return sceneInfo;
    }
}