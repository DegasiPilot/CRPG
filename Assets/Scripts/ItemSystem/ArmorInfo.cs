using UnityEngine;

[CreateAssetMenu(fileName = "NewArmorInfo", menuName = "ScriptableObjects/ArmorInfo")]
internal class ArmorInfo : ItemInfo
{
    public int ArmorClass;
    public BodyPart wearableBodyPart;
    public override ItemType ItemType => ItemType.Armor;
}