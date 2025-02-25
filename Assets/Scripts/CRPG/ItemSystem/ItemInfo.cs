using UnityEngine;

[CreateAssetMenu(fileName = "NewItemInfo", menuName = "ScriptableObjects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    public int Id;
    public string Name;
    public string ShortDescription;
    public Sprite Icon;
    public GameObject Prefab;

    public virtual ItemType ItemType => ItemType.Other;

    public virtual string GetFullDescrition()
    {
        return ShortDescription;
    }
}