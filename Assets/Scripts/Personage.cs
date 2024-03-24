using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Personage : MonoBehaviour
{
    public string DefaultName;
    public Item Weapon;
    public List<Item> Armor;
    public int ArmorClass => (from armor in Armor select (armor.ItemInfo as ArmorInfo).ArmorClass).Sum();

    [HideInInspector] public int CurrentHealth;

    public int MaxHealth { get; private set; }
    public PersonageInfo PersonageInfo => _personageInfo;

    private PersonageInfo _personageInfo;

    public void Setup()
    {
        _personageInfo ??= CRUD.GetPersonageInfo(DefaultName);
        _personageInfo.Setup();
        CurrentHealth = _personageInfo.MaxHealth;
    }

    public void Setup(PersonageInfo personageInfo)
    {
        _personageInfo = personageInfo;
        _personageInfo.Setup();
        CurrentHealth = _personageInfo.MaxHealth;
    }

    public void GetDamage(int damage, DamageType damageType)
    {
        float blockedDamage = 0;
        if(_personageInfo.Race == Race.Elf)
        {
            blockedDamage = Mathf.Max(1f, damage * 0.15f);
        }
        CurrentHealth -= (int)Mathf.Round(Mathf.Max(damage - blockedDamage));
        Debug.Log($"{DefaultName} получил урон теперь у него {CurrentHealth} жизней");
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
