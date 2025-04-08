using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Personage : MonoBehaviour
{
    [System.NonSerialized] public UnityEvent OnDeath = new();
    [System.NonSerialized] public UnityEvent OnHealthChanged = new();
    [System.NonSerialized] public UnityEvent OnStaminaChanged = new();

    public bool IsDead;
    public Transform HitPoint;

    public Item Weapon;
    public List<Item> Armor = new List<Item>();
    public BattleTeam BattleTeam;
    public PersonageInfo PersonageInfo;
    public ActionType[] Actions => new ActionType[] { ActionType.Jumping, ActionType.Attack };

    public WeaponInfo WeaponInfo => Weapon?.ItemInfo as WeaponInfo;
    public int MinAttackEnergy => WeaponInfo == null ? GameData.MinUnarmedAttackEnergy : WeaponInfo.MinEnergy;
    public int MaxAttackEnergy => WeaponInfo == null ? GameData.MaxUnarmedAttackEnergy : WeaponInfo.MaxEnergy;

    private IEnumerable<ArmorInfo> _armorInfos
    {
        get
        {
            if (Armor == null || Armor.Count <= 0)
            {
                return null;
            }
            else
            {
				return from armor in Armor select (armor.ItemInfo as ArmorInfo);
			}
		}
    }

	public float ArmorPercent => _armorInfos == null ? 0 : _armorInfos.Sum(a => a.ArmorPercent); 

    [HideInInspector] public float Health { get; private set; }
	[HideInInspector] public float Stamina { get; private set; }

    public int MaxHealth => PersonageInfo.MaxHealth;
	public int MaxStamina => PersonageInfo.MaxStamina;

	public void Setup(PersonageInfo personageInfo)
    {
        PersonageInfo = personageInfo;
        Health = PersonageInfo.MaxHealth;
        Stamina = PersonageInfo.MaxStamina;
    }

    public void GetDamage(float damage, DamageType damageType)
    {
        float blockedDamage = damage * ArmorPercent;
        if(PersonageInfo.Race == Race.Elf && damageType == DamageType.Magic)
        {
            blockedDamage += Mathf.Max(1f, damage * 0.15f); // some strange
        }
        float incomedDamage = damage - blockedDamage;
		Health -= incomedDamage;
        if (Health <= 0)
        {
            Health = 0;
            Death();
        }
        OnHealthChanged.Invoke();
        Debug.Log($"{PersonageInfo.Name} получил {incomedDamage} урона теперь у него {Health} жизней");
    }

    public void RemoveStamina(float stamina)
    {
        if(stamina > 0)
        {
			Stamina -= stamina;
			OnStaminaChanged.Invoke();
			Debug.Log($"{PersonageInfo.Name} потратил {stamina} энергии теперь у него {Stamina} энергии");
		}
	}

    private void Death()
    {
        OnDeath?.Invoke();
    }
}