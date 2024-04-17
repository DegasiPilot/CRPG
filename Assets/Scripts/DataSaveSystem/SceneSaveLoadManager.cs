using System;
using System.Collections.Generic;
using UnityEngine;

internal class SceneSaveLoadManager : MonoBehaviour
{
    public static SceneSaveLoadManager Instance;

    public List<SaveableGameobject> ObjectsToSave;
    public Vector3 StartPlayerPosition;
    public Vector3 StartPlayerRotation;

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