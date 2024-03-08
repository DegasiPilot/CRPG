using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponInfo", menuName = "ScriptableObjects/WeaponInfo")]
internal class WeaponInfo : ItemInfo
{
    public float MaxAttackDistance;
    public int MinDamage;
    public int MaxDamage;
    public Characteristics usingCharacteristic;
    public bool IsTwoHandled;

    public override ItemType ItemType => ItemType.Weapon;
}