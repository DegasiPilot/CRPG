using MongoDB.Bson;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemInfo", menuName = "ScriptableObjects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public GameObject Prefab;
}