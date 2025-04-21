using UnityEngine;

[CreateAssetMenu(fileName = "NewArmorInfo", menuName = "ScriptableObjects/ArmorInfo")]
internal class ArmorInfo : ItemInfo
{
    public int SkinIndex;
    [SerializeField, Range(0,100)] private int _armorPercent;
    public float ArmorPercent => _armorPercent / 100f;
    public BodyPart WearableBodyPart;
    [Min(0)] public float ArmorWeight;
}