using UnityEngine;

[CreateAssetMenu(fileName = "NewArmorInfo", menuName = "ScriptableObjects/ArmorInfo")]
internal class ArmorInfo : ItemInfo
{
    public int ArmorClass;
    public BodyPart WearableBodyPart;
    public ArmorWeight ArmorWeight;

    public override ItemType ItemType => ItemType.Armor;
}