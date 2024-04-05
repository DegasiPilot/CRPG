using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Personage : MonoBehaviour
{
    public Item Weapon;
    public List<Item> Armor;
    public BattleTeam battleTeam;
    public PersonageInfo PersonageInfo;
    [HideInInspector] public PersonageController Controller;

    public int ArmorClass {
        get
        {
            if (Armor == null || !Armor.Any()) return 10 + PersonageInfo.GetCharacteristicBonus(Characteristics.Dexterity);
            var armorInfos = from armor in Armor select (armor.ItemInfo as ArmorInfo);
            int armorClass = (from armorInfo in armorInfos select armorInfo.ArmorClass).Sum();
            ArmorWeight maxArmorWeight = (from armorInfo in armorInfos select armorInfo.ArmorWeight).Max();
            if (maxArmorWeight == ArmorWeight.Heavy)
            {
                return 10 + armorClass;
            }
            else if (maxArmorWeight == ArmorWeight.Medium)
            {
                return 10 + armorClass + Mathf.Min(2, PersonageInfo.GetCharacteristicBonus(Characteristics.Dexterity));
            }
            else
            {
                return  10 + armorClass + PersonageInfo.GetCharacteristicBonus(Characteristics.Dexterity);
            }
        }
    }

    [HideInInspector] public int CurrentHealth;

    public int MaxHealth => PersonageInfo.MaxHealth;

    private void Awake()
    {
        Controller = GetComponent<PersonageController>();
        if(PersonageInfo != null)
        {
            PersonageInfo.Setup();
            CurrentHealth = PersonageInfo.MaxHealth;
        }
    }

    public void Setup(PersonageInfo personageInfo)
    {
        PersonageInfo = personageInfo;
        PersonageInfo.Setup();
        CurrentHealth = PersonageInfo.MaxHealth;
    }

    public void GetDamage(int damage, DamageType damageType)
    {
        float blockedDamage = 0;
        if(PersonageInfo.Race == Race.Elf && damageType == DamageType.Magic)
        {
            blockedDamage = Mathf.Max(1f, damage * 0.15f);
        }
        CurrentHealth -= (int)Mathf.Round(Mathf.Max(damage - blockedDamage));
        Debug.Log($"{PersonageInfo.Name} получил урон теперь у него {CurrentHealth} жизней");
        if(CurrentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}