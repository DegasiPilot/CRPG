using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponInfo", menuName = "ScriptableObjects/WeaponInfo")]
public class WeaponInfo : ItemInfo
{
    public float MaxAttackDistance;
    public int MinEnergy;
    public int MaxEnergy;
    public bool IsTwoHandled;
    [Min(0)] public float Weight;
    public DamageType DamageType;

    public override ItemType ItemType => ItemType.Weapon;

    public override string GetFullDescrition()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(base.GetFullDescrition());
        stringBuilder.AppendLine($"Дальность: {MaxAttackDistance}");
        stringBuilder.AppendLine($"Энергия: {MinEnergy} - {MaxEnergy}");
        if (IsTwoHandled)
        {
            stringBuilder.AppendLine("Двуручное");
        }
        return stringBuilder.ToString();
    }
}