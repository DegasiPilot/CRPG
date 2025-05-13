using CRPG.DataSaveSystem;
using CRPG.DataSaveSystem.SaveData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Personage : MonoBehaviour, ISaveableComponent, ISaveBlocker
{
	public UnityEvent OnDeath = new();
	[System.NonSerialized] public UnityEvent OnHealthChanged = new();
	[System.NonSerialized] public UnityEvent OnStaminaChanged = new();
	[System.NonSerialized] public UnityEvent OnRemainActionsChanged = new();

	[SerializeField] private EquipmentManager _equipmentManager;
	public EquipmentManager EquipmentManager => _equipmentManager;
	public bool IsDead;
	public Transform HitPoint;

	[SerializeField] public BattleTeam BattleTeam;
	public PersonageInfo PersonageInfo;
	public ActionType[] Actions => new ActionType[] { ActionType.Jumping, ActionType.Attack };

	public int MinAttackEnergy => _equipmentManager.MinAttackEnergy;
	public int MaxAttackEnergy => _equipmentManager.MaxAttackEnergy;
	public float DodgeCoefficient => (0.1f + (PersonageInfo.Dexterity / 2 * 3) / 100f) * _equipmentManager.DodgeModifier;

	public float ArmorPercent => _equipmentManager.ArmorPercent;
	public DamageType DamageType => _equipmentManager.DamageType;
	public bool IsAttackRanged => _equipmentManager.Weapon != null && _equipmentManager.Weapon.IsRanged;

	private float _health;
	public float Health
	{
		get => _health;
		private set
		{
			_health = value;
			OnHealthChanged.Invoke();
		}
	}

	private float _stamina;
	public float Stamina
	{
		get => _stamina;
		set
		{
			if (value <= 0)
			{
				if (_stamina <= 0)
				{
					return;
				}
				else
				{
					value = 0;
				}
			}
			else if (value > MaxStamina)
			{
				value = MaxStamina;
			}
			else if (value == _stamina)
			{
				return;
			}
			else
			{
				var oldValue = _stamina;
				_stamina = MathF.Round(value, 1);
				OnStaminaChanged.Invoke();
			}
		}
	}

	private int _remainActions;
	public int RemainActions
	{
		get => _remainActions;
		set
		{
			_remainActions = value;
			OnRemainActionsChanged.Invoke();
		}
	}

	public int MaxHealth => PersonageInfo.MaxHealth;
	public int MaxStamina => PersonageInfo.MaxStamina;
	public int MaxActions => PersonageInfo.ActionsPerTurn;

	public bool IsBlockSave => IsDead;

	public AnimatorManager AnimatorManager;

	public void Setup()
	{
		Health = PersonageInfo.MaxHealth;
		Stamina = PersonageInfo.MaxStamina;
		RemainActions = PersonageInfo.ActionsPerTurn;
	}

	internal void Setup(PersonageSaveInfo personageSaveInfo)
	{
		Health = personageSaveInfo.Health;
		Stamina = personageSaveInfo.Stamina;
		RemainActions = PersonageInfo.ActionsPerTurn;
	}

	public void GetDamage(float damage, DamageType damageType)
	{
		float blockedDamage = damage * ArmorPercent;
		if (PersonageInfo.RaceInfo.Race == Race.Elf && damageType == DamageType.Magic)
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
		// Debug.Log($"{PersonageInfo.Name} получил {incomedDamage} урона теперь у него {Health} жизней");
	}

	private void Death()
	{
		IsDead = true;
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
		foreach (var data in componentsData)
		{
			if (data is PersonageSaveInfo personageSaveInfo)
			{
				Health = personageSaveInfo.Health;
				Stamina = personageSaveInfo.Stamina;
				return;
			}
		}
		throw new System.Exception("Not save data for personage");
	}
}