using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponInfo", menuName = "ScriptableObjects/WeaponInfo")]
public class WeaponInfo : ItemInfo
{
    public float MaxAttackDistance;
    public int MinDamage;
    public int MaxDamage;
    public bool IsTwoHandled;
    public Characteristics usingCharacteristic;

    public override ItemType ItemType => ItemType.Weapon;

    public override string GetFullDescrition()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(base.GetFullDescrition());
        stringBuilder.AppendLine($"Дальность: {MaxAttackDistance}");
        stringBuilder.AppendLine($"Урон {MinDamage} - {MaxDamage}");
        if (IsTwoHandled)
        {
            stringBuilder.AppendLine("Двуручное");
        }
        stringBuilder.AppendLine(Translator.Translate(usingCharacteristic));
        return stringBuilder.ToString();
    }
}