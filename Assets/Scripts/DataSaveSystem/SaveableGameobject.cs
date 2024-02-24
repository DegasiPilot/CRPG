using MongoDB.Bson;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class SaveableGameobject : MonoBehaviour
{
    private Item itemComponent;

    private void Awake()
    {
        TryGetComponent(out itemComponent);
    }

    public SaveObjectInfo GetSaveInfo()
    {
        SaveObjectInfo info = new SaveObjectInfo()
        {
            PosX = transform.position.x,
            PosY = transform.position.y,
            PosZ = transform.position.z,
            RotX = transform.eulerAngles.x,
            RotY = transform.eulerAngles.y,
            RotZ = transform.eulerAngles.z,
            IsActive = isActiveAndEnabled,
        };
        if(itemComponent != null)
        {
            info.IsInInventory = itemComponent.IsInInventory;
        }
        return info;
    }

    public void LoadSaveInfo(SaveObjectInfo info)
    {
        transform.position.Set(info.PosX, info.PosY, info.PosZ);
        transform.eulerAngles = new Vector3(info.RotX, info.RotY, info.RotZ);
        gameObject.SetActive(info.IsActive);
        if (info.IsInInventory)
        {
            itemComponent.IsInInventory = true;
        }
    }
}