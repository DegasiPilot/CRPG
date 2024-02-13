using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personage : MonoBehaviour
{
    public PersonageInfo personageInfo;
    public List<GameObject> Inventory;

    public void PickupItem(GameObject itemObject)
    {
        Inventory.Add(itemObject);
    }
}
