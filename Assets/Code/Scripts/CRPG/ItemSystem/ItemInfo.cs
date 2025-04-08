using UnityEngine;

[CreateAssetMenu(fileName = "NewItemInfo", menuName = "ScriptableObjects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    public string Name;
    [TextArea] public string ShortDescription;
    public Sprite Icon;

    public virtual ItemType ItemType => ItemType.Other;

    public virtual string GetFullDescrition()
    {
        return ShortDescription;
    }
}