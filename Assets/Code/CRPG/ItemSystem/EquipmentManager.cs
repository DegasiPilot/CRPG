using CRPG;
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

    internal Weapon Weapon
    {
        get
        {
			if (RightHandSlot.EquipableItem is Weapon Rweapon)
			{
				return Rweapon;
			}
			else if (LeftHandSlot.EquipableItem is Weapon Lweapon)
            {
                return Lweapon;
            }
            else
            {
                return null;
            }
        }
    }

    internal DamageType DamageType => Weapon != null ? Weapon.WeaponInfo.DamageType : GlobalRules.UnarmedDamageType;

    internal IEnumerable<Armor> Armor
    {
        get
        {
            if(HealmetSlot.EquipableItem is Armor healmetArmor)
            {
                yield return healmetArmor;
            }
			if (BodySlot.EquipableItem is Armor bodyArmor)
			{
				yield return bodyArmor;
			}
			if (BootsSlot.EquipableItem is Armor boorsArmor)
			{
				yield return boorsArmor;
			}
		}
    }

    internal List<ProjectileItem> Projectiles => ProjectileSlot.ProjectileItems;
	
    public float ArmorPercent
    {
        get
        {
            float result = 0;
            foreach(var armor in Armor)
            {
                result += armor.ArmorInfo.ArmorPercent;
            }
            return result;
        }
    }

	public float DodgeModifier
	{
		get
		{
			float result = 1;
			foreach (var armor in Armor)
			{
				result -= armor.ArmorInfo.DodgePenalty;
			}
			return result;
		}
	}

	public IEnumerable<EquipableItem> EquipableItems
    {
        get
        {
            if(Weapon != null)
            {
				yield return Weapon;
			}
            foreach(var item in Armor)
            {
                yield return item;
            }
            if(Projectiles != null)
            {
				foreach (var projectile in Projectiles)
				{
					yield return projectile;
				}
			}
        }
    }

	public int MinAttackEnergy => Weapon == null ? GlobalRules.MinUnarmedAttackEnergy : Weapon.WeaponInfo.MinEnergy;
	public int MaxAttackEnergy => Weapon == null ? GlobalRules.MaxUnarmedAttackEnergy : Weapon.WeaponInfo.MaxEnergy;
	public float MaxAttackDistance => Weapon == null ? GlobalRules.MaxUnarmedAttackDistance : Weapon.WeaponInfo.MaxAttackDistance;

	private void Awake()
	{
		HealmetSlot.OnEquipItem += InvokeEquipItem;
		BodySlot.OnEquipItem += InvokeEquipItem;
		LeftHandSlot.OnEquipItem += InvokeEquipItem;
		RightHandSlot.OnEquipItem += InvokeEquipItem;
		BootsSlot.OnEquipItem += InvokeEquipItem;
        ProjectileSlot.OnEquipProjectile += InvokeEquipProjectile;

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
		OnItemEquiped?.Invoke(item, bodyPart);
	}

	private void InvokeUnequipItem(EquipableItem item, BodyPart bodyPart)
    {
        if(bodyPart == BodyPart.RightHand && item is Weapon weapon && weapon.WeaponInfo.IsTwoHandled)
        {
            return;
        }
        OnItemUnequiped?.Invoke(item, bodyPart);
    }

    private void InvokeEquipProjectile()
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
        WeaponInfo newWeaponInfo = item.WeaponInfo;
        if (RightHandSlot.EquipableItem != null && RightHandSlot.EquipableItem is Weapon equipedWeapon
            && equipedWeapon.WeaponInfo.IsTwoHandled)
        {
            LeftHandSlot.ClearSlot();
        }
        if (newWeaponInfo.IsTwoHandled)
        {
            LeftHandSlot.EquipItem(item);
        }

        RightHandSlot.EquipItem(item);
    }

    internal void EquipArmor(Armor item)
    {
        switch (item.ArmorInfo.WearableBodyPart)
        {
            case BodyPart.Head:
                HealmetSlot.EquipItem(item);
                break;
            case BodyPart.Body:
                BodySlot.EquipItem(item);
                break;
            case BodyPart.LeftHand:
                LeftHandSlot.EquipItem(item);
                break;
            case BodyPart.Legs:
                BootsSlot.EquipItem(item);
                break;
        }
    }

    internal void EquipProjectile(ProjectileItem projectileItem)
    {
        ProjectileSlot.EquipProjectile(projectileItem);
    }

	internal void EquipProjectiles(IEnumerable<ProjectileItem> projectileItems)
	{
		ProjectileSlot.EquipProjectiles(projectileItems);
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
    }

    internal void UnequipProjectile(ProjectileSlot projectileSlot)
    {
        projectileSlot.ClearSlot();
    }

    internal bool IsWeaponNeedProjectiles => Weapon != null && Weapon.WeaponInfo.RequiredProjectile != null;

    internal bool CanReloadWeapon => Projectiles != null && Projectiles[0].ItemInfo == Weapon.WeaponInfo.RequiredProjectile;


	internal bool TryReloadWeapon()
    {
		if(CanReloadWeapon)
        {
            var projectileItem = ProjectileSlot.GetOne();
            Debug.Log("Wepon Reloaded");
            projectileItem.IsEquiped = false;
            projectileItem.OnDropped();
            projectileItem.Rigidbody.isKinematic = true;
            Weapon.WeaponAnimationManager.ActiveProjectile = projectileItem;
            return true;
        }
        else
        {
            Debug.Log("No projectiles to reload!");
            return false;
        }
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

		ProjectileSlot.OnEquipProjectile -= InvokeEquipProjectile;
		ProjectileSlot.OnUnequipItems -= InvokeUnequipProjectile;
	}

	private void OnDestroy()
	{
        Release();
	}
}