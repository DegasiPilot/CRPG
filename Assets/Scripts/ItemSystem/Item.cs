using UnityEngine;

[RequireComponent(typeof(SaveableGameobject))]
public class Item : MonoBehaviour
{
    public ItemInfo ItemInfo;
    public bool IsInInventory;
    public bool IsEquiped;
}