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
        if (itemComponent != null)
        {
            if (itemComponent.IsInInventory)
            {
                SaveObjectInfo destroyedInfo = new SaveObjectInfo();
                destroyedInfo.IsPickuped = itemComponent.IsInInventory;
                return destroyedInfo;
            }
        }
        SaveObjectInfo info = new SaveObjectInfo()
        {
            Pos = transform.position,
            Rot = transform.eulerAngles,
            IsActive = isActiveAndEnabled,
        };
        if (itemComponent != null)
        {
            info.IsPickuped = false;
        }
        return info;
    }

    public void LoadSaveInfo(SaveObjectInfo info)
    {
        if (itemComponent != null)
        {
            if (info.IsPickuped)
            {
                Destroy(gameObject);
                return;
            }
        }
        transform.SetPositionAndRotation(info.Pos, Quaternion.Euler(info.Rot));
        gameObject.SetActive(info.IsActive);
    }
}