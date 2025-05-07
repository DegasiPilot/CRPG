using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CRPG.DataSaveSystem;
using CRPG.DataSaveSystem.SaveData;
using System;

public class Personage : MonoBehaviour, ISaveableComponent, ISaveBlocker
{
    [System.NonSerialized] public UnityEvent OnDeath = new();
    [System.NonSerialized] public UnityEvent OnHealthChanged = new();
    [System.NonSerialized] public UnityEvent OnStaminaChanged = new();

	[SerializeField] private EquipmentManager _equipmentManager;
    public EquipmentManager EquipmentManager => _equipmentManager;
	public bool IsDead;
    public Transform HitPoint;

    [SerializeField] public BattleTeam BattleTeam;
    public PersonageInfo PersonageInfo;
    public ActionType[] Actions => new ActionType[] { ActionType.Jumping, ActionType.Attack };

    public int MinAttackEnergy => _equipmentManager.MinAttackEnergy;
    public int MaxAttackEnergy => _equipmentManager.MaxAttackEnergy;
    public float DodgeCoefficient => (0.1f + (PersonageInfo.Dexterity / 2 * 3)/100f) * _equipmentManager.DodgeModifier;

    public float ArmorPercent => _equipmentManager.ArmorPercent;
    public DamageType DamageType => _equipmentManager.DamageType;
    public bool IsAttackRanged => _equipmentManager.Weapon != null && _equipmentManager.Weapon.IsRanged;

    public float Health { get; private set; }
	public float Stamina { get; private set; }

    public int MaxHealth => PersonageInfo.MaxHealth;
	public int MaxStamina => PersonageInfo.MaxStamina;

	public bool IsBlockSave => IsDead;
    public AnimatorManager AnimatorManager;

	public void Setup()
    {
        Health = PersonageInfo.MaxHealth;
        Stamina = PersonageInfo.MaxStamina;
    }

	internal void Setup(PersonageSaveInfo personageSaveInfo)
	{
		Health = personageSaveInfo.Health;
		Stamina = personageSaveInfo.Stamina;
	}

	public void GetDamage(float damage, DamageType damageType)
    {
        float blockedDamage = damage * ArmorPercent;
        if(PersonageInfo.RaceInfo.Race == Race.Elf && damageType == DamageType.Magic)
        {
            blockedDamage += Mathf.Max(1f, damage * 0.15f); // some strange
        }
        float incomedDamage = damage - blockedDamage;
		Health = MathF.Round(Health - incomedDamage, 1);
        if (Health <= 0)
        {
            Health = 0;
            Death();
        }
        OnHealthChanged.Invoke();
        // Debug.Log($"{PersonageInfo.Name} получил {incomedDamage} урона теперь у него {Health} жизней");
    }

    public void RemoveStamina(float stamina)
    {
        if(stamina > 0)
        {
            Stamina = MathF.Round(Stamina - stamina, 1);
			OnStaminaChanged.Invoke();
			// Debug.Log($"{PersonageInfo.Name} потратил {stamina} энергии теперь у него {Stamina} энергии");
		}
	}

    private void Death()
    {
		OnDeath?.Invoke();
    }

	public object Save()
	{
		var info = new PersonageSaveInfo()
        {
            Health = this.Health,
            Stamina = this.Stamina
        };
        //PersonageInfo.SaveTo(info);
        return info;
	}

	public void Load(IReadOnlyCollection<object> componentsData)
	{
		foreach(var data in componentsData)
        {
            if(data is PersonageSaveInfo personageSaveInfo)
            {
                Health = personageSaveInfo.Health;
                Stamina = personageSaveInfo.Stamina;
                return;
            }
        }
        throw new System.Exception("Not save data for personage");
	}
}