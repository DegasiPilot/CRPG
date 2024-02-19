using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;

public class Personage : MonoBehaviour
{
    public string DefaultName;

    public PersonageInfo PersonageInfo;

    private void Awake()
    {
        PersonageInfo ??= CRUD.GetPersonageInfo(DefaultName);
    }

    public void PickupItem(GameObject itemObject)
    {
        PersonageInfo.Inventory.Add(itemObject);
    }
}
