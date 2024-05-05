using System;
using UnityEngine;

[Serializable]
public class SaveableGameobject : MonoBehaviour
{
    private Item itemComponent;
    private Personage personageComponent;

    private void Awake()
    {
        TryGetComponent(out itemComponent);
    }

    public SaveObjectInfo GetSaveInfo()
    {
        TryGetComponent(out itemComponent);
        if (itemComponent != null)
        {
            if (itemComponent.IsInInventory)
            {
                SaveObjectInfo destroyedInfo = new SaveObjectInfo();
                destroyedInfo.IsDestroyed = itemComponent.IsInInventory;
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
            info.IsDestroyed = false;
        }
        if(personageComponent != null)
        {
            info.IsDestroyed = personageComponent.IsDead;
        }
        return info;
    }

    public void LoadSaveInfo(SaveObjectInfo info)
    {
        if (itemComponent != null || personageComponent != null)
        {
            if (info.IsDestroyed)
            {
                Destroy(gameObject);
                return;
            }
        }
        transform.SetPositionAndRotation(info.Pos, Quaternion.Euler(info.Rot));
        gameObject.SetActive(info.IsActive);
    }
}