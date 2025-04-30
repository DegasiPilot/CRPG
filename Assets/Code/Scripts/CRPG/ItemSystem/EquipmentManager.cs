using CRPG.ItemSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public event Action<EquipableItem, BodyPart> OnItemEquiped;
    public event Action<EquipableItem, BodyPart> OnItemUnequiped;
    internal event Action OnProjectileEquiped;
    internal event Action<List<ProjectileItem>> OnProjectileUnequiped;

    public readonly EquipmentSlot HealmetSlot = new(BodyPart.Head);
	public readonly EquipmentSlot BodySlot = new(BodyPart.Body);
    public readonly EquipmentSlot LeftHandSlot = new(BodyPart.LeftHand);
    public readonly EquipmentSlot RightHandSlot = new(BodyPart.RightHand);
    public readonly EquipmentSlot BootsSlot = new(BodyPart.Legs);
    internal readonly ProjectileSlot ProjectileSlot = new();

    [SerializeField] private Personage _personage;

    private Weapon Weapon { 
        get => _personage.Weapon;
        set => _personage.Weapon = value;
    }
    private List<Armor> _armor => _personage.Armor;
    public IEnumerable<EquipableItem> EquipableItems
    {
        get
        {
            if(Weapon != null)
            {
				yield return Weapon;
			}
            foreach(var item in _armor)
            {
                yield return item;
            }
            foreach (var projectile in _personage.Projectiles)
            {
                yield return projectile;
            }
        }
    }

	private void OnValidate()
	{
		if(_personage == null)
        {
            Debug.LogWarning("Personage = null", this);
        }
	}

	private void Awake()
	{
		HealmetSlot.OnEquipItem += InvokeEquipItem;
		BodySlot.OnEquipItem += InvokeEquipItem;
		LeftHandSlot.OnEquipItem += InvokeEquipItem;
		RightHandSlot.OnEquipItem += InvokeEquipItem;
		BootsSlot.OnEquipItem += InvokeEquipItem;
        ProjectileSlot.OnEquipItems += InvokeEquipProjectile;

		HealmetSlot.OnUnequipItem += InvokeUnequipItem;
		BodySlot.OnUnequipItem += InvokeUnequipItem;
		LeftHandSlot.OnUnequipItem += InvokeUnequipItem;
		RightHandSlot.OnUnequipItem += InvokeUnequipItem;
		BootsSlot.OnUnequipItem += InvokeUnequipItem;
		ProjectileSlot.OnUnequipItems += InvokeUnequipProjectile;
	}

	public void Setup(EquipableItem[] equipedItems)
    {
		if (equipedItems != null)
		{
			foreach (var item in equipedItems)
			{
				EquipItem(item);
			}
		}
	}

	private void InvokeEquipItem(EquipableItem item, BodyPart bodyPart)
	{
		OnItemEquiped.Invoke(item, bodyPart);
	}

	private void InvokeUnequipItem(EquipableItem item, BodyPart bodyPart)
    {
        OnItemUnequiped.Invoke(item, bodyPart);
    }

    private void InvokeEquipProjectile(ProjectileItemInfo projectileItemInfo)
    {
        OnProjectileEquiped.Invoke();
    }

    private void InvokeUnequipProjectile(List<ProjectileItem> projectileItems)
    {
        OnProjectileUnequiped.Invoke(projectileItems);
    }


	public void EquipItem(EquipableItem item)
    {
        if (item is Weapon weapon)
        {
            EquipWeapon(weapon);
        }
        else if (item is Armor armor)
        {
            EquipArmor(armor);
        }
        else if (item is ProjectileItem projectileItem)
        {
            EquipProjectile(projectileItem);
        }
	}

    internal void EquipWeapon(Weapon item)
    {
        Weapon = item;
        WeaponInfo newWeaponInfo = item.WeaponInfo;
        if (RightHandSlot.EquipableItem != null && RightHandSlot.EquipableItem is Weapon equipedWeapon && equipedWeapon.WeaponInfo.IsTwoHandled)
        {
            LeftHandSlot.ClearSlot();
        }
        if (newWeaponInfo.IsTwoHandled)
        {
            LeftHandSlot.EquipItem(item, out EquipableItem undressedItem);
        }

        RightHandSlot.EquipItem(item, out EquipableItem undressedWeapon);
    }

    internal void EquipArmor(Armor item)
    {
        _armor.Add(item);
        EquipableItem undressedItem = null;
        switch (item.ArmorInfo.WearableBodyPart)
        {
            case BodyPart.Head:
                HealmetSlot.EquipItem(item, out undressedItem);
                break;
            case BodyPart.Body:
                BodySlot.EquipItem(item, out undressedItem);
                break;
            case BodyPart.LeftHand:
                LeftHandSlot.EquipItem(item, out undressedItem);
                break;
            case BodyPart.Legs:
                BootsSlot.EquipItem(item, out undressedItem);
                break;
        }
        if (undressedItem != null && undressedItem is Armor undressedArmor)
        {
            _armor.Remove(undressedArmor);
        }
    }

    internal void EquipProjectile(ProjectileItem projectileItem)
    {
        ProjectileSlot.EquipProjectile(projectileItem);
        _personage.Projectiles = ProjectileSlot.ProjectileItems;
    }

    public void UnequipItemFromSlot(EquipmentSlot slot)
    {
        EquipableItem uneqipedItem;
		if (slot.EquipableItem is Weapon weapon &&
            weapon.WeaponInfo.IsTwoHandled)
        {
            LeftHandSlot.ClearSlot();
            uneqipedItem = RightHandSlot.EquipableItem;
            RightHandSlot.ClearSlot();
        }
        else
        {
            uneqipedItem = slot.EquipableItem;
            slot.ClearSlot();
        }
        if(uneqipedItem is Weapon)
        {
            Weapon = null;
        }
        else if(uneqipedItem is Armor armor)
        {
            _armor.Remove(armor);
        }
    }

    internal void UnequipProjectile(ProjectileSlot projectileSlot)
    {
        projectileSlot.ClearSlot();
        _personage.Projectiles = null;
    }

    private void Release()
    {
		HealmetSlot.OnEquipItem -= InvokeEquipItem;
		BodySlot.OnEquipItem -= InvokeEquipItem;
		LeftHandSlot.OnEquipItem -= InvokeEquipItem;
		RightHandSlot.OnEquipItem -= InvokeEquipItem;
		BootsSlot.OnEquipItem -= InvokeEquipItem;

		HealmetSlot.OnUnequipItem -= InvokeUnequipItem;
		BodySlot.OnUnequipItem -= InvokeUnequipItem;
		LeftHandSlot.OnUnequipItem -= InvokeUnequipItem;
		RightHandSlot.OnUnequipItem -= InvokeUnequipItem;
		BootsSlot.OnUnequipItem -= InvokeUnequipItem;

		ProjectileSlot.OnEquipItems -= InvokeEquipProjectile;
		ProjectileSlot.OnEquipItems -= InvokeEquipProjectile;
	}

	private void OnDestroy()
	{
        Release();
	}
}