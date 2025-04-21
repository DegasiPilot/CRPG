using CRPG.ItemSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public event Action<Item, BodyPart> OnItemEquiped;
    public event Action<Item, BodyPart> OnItemUnequiped;

    public readonly EquipmentSlot HealmetSlot = new(BodyPart.Head);
	public readonly EquipmentSlot BodySlot = new(BodyPart.Body);
    public readonly EquipmentSlot LeftHandSlot = new(BodyPart.LeftHand);
    public readonly EquipmentSlot RightHandSlot = new(BodyPart.RightHand);
    public readonly EquipmentSlot BootsSlot = new(BodyPart.Legs);

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

		HealmetSlot.OnUnequipItem += InvokeUnequipItem;
		BodySlot.OnUnequipItem += InvokeUnequipItem;
		LeftHandSlot.OnUnequipItem += InvokeUnequipItem;
		RightHandSlot.OnUnequipItem += InvokeUnequipItem;
		BootsSlot.OnUnequipItem += InvokeUnequipItem;
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

	private void InvokeEquipItem(Item item, BodyPart bodyPart)
	{
		OnItemEquiped.Invoke(item, bodyPart);
	}

	private void InvokeUnequipItem(Item item, BodyPart bodyPart)
    {
        OnItemUnequiped.Invoke(item, bodyPart);
    }

    public void EquipItem(Item item)
    {
        if (item is Weapon weapon)
        {
            EquipWeapon(weapon);
            Weapon = weapon;
        }
        else if (item is Armor armor)
        {
            EquipArmor(armor);
            _armor.Add(armor);
        }
	}

    internal void EquipWeapon(Weapon item)
    {
        Weapon = item;
        WeaponInfo newWeaponInfo = item.WeaponInfo;
        if (RightHandSlot.Item != null && (RightHandSlot.Item.ItemInfo as WeaponInfo).IsTwoHandled)
        {
            LeftHandSlot.UnEquipItem();
        }
        if (newWeaponInfo.IsTwoHandled)
        {
            LeftHandSlot.EquipItem(item, out EquipableItem undressedItem, true);
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

    public void UnequipItemFromSlot(EquipmentSlot slot)
    {
        Item uneqipedItem;
		if (slot.Item is Weapon weapon &&
            weapon.WeaponInfo.IsTwoHandled)
        {
            LeftHandSlot.UnEquipItem();
            uneqipedItem = RightHandSlot.Item;
            RightHandSlot.UnEquipItem();
        }
        else
        {
            uneqipedItem = slot.Item;
            slot.UnEquipItem();
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
	}

	private void OnDestroy()
	{
        Release();
	}
}