using System.Collections.Generic;
using UnityEngine;
using System.Linq;

internal class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }
    public Item Weapon => _weapon;
    public readonly HashSet<Item> Armor = new();
    public int ArmorClass => (from armor in Armor select (armor.ItemInfo as ArmorInfo).ArmorClass).Sum();

    [SerializeField] private EquipmentSlot HealmetSlot;
    [SerializeField] private EquipmentSlot BodySlot;
    [SerializeField] private EquipmentSlot LeftHandSlot;
    [SerializeField] private EquipmentSlot RightHandSlot;
    [SerializeField] private EquipmentSlot BootsSlot;

    private Item _weapon;

    private void Awake()
    {
        Instance = this;
        HealmetSlot.Setup(BodyPart.Head);
        BodySlot.Setup(BodyPart.Body);
        LeftHandSlot.Setup(BodyPart.LeftHand);
        RightHandSlot.Setup(BodyPart.RightHand);
        BootsSlot.Setup(BodyPart.Legs);
        foreach (var item in GameData.Inventory)
        {
            if (item.IsEquiped)
            {
                item.IsEquiped = false;
                EquipItem(item, out _);
            }
        }
    }

    public void EquipItem(Item item, out List<Item> undressedItems)
    {
        ItemType itemType = item.ItemInfo.ItemType;
        undressedItems = new List<Item>(2);

        if (itemType == ItemType.Weapon)
        {
            EquipWeapon(item, undressedItems);
            _weapon = item;
        }
        else if (itemType == ItemType.Armor)
        {
            EquipArmor(item, undressedItems);
            Armor.Add(item);
        }
        Armor.ExceptWith(undressedItems);
    }

    public void EquipWeapon(Item item, List<Item> undressedItems)
    {
        WeaponInfo newWeaponInfo = item.ItemInfo as WeaponInfo;
        if (RightHandSlot.Item != null && (RightHandSlot.Item.ItemInfo as WeaponInfo).IsTwoHandled)
        {
            LeftHandSlot.UnequipItem();
        }
        if (newWeaponInfo.IsTwoHandled)
        {
            LeftHandSlot.EquipItem(item, out Item undressedItem, true);
            if (undressedItem != null)
            {
                undressedItems.Add(undressedItem);
            }
        }

        RightHandSlot.EquipItem(item, out Item undressedWeapon);
        if (undressedWeapon != null)
        {
            undressedItems.Add(undressedWeapon);
        }
    }

    public void EquipArmor(Item item, List<Item> undressedItems)
    {
        Item undressedArmor = null;
        switch ((item.ItemInfo as ArmorInfo).wearableBodyPart)
        {
            case BodyPart.Head:
                HealmetSlot.EquipItem(item, out undressedArmor);
                break;
            case BodyPart.Body:
                BodySlot.EquipItem(item, out undressedArmor);
                break;
            case BodyPart.LeftHand:
                LeftHandSlot.EquipItem(item, out undressedArmor);
                break;
            case BodyPart.Legs:
                BootsSlot.EquipItem(item, out undressedArmor);
                break;
        }
        if (undressedArmor != null)
        {
            undressedItems.Add(undressedArmor);
        }
    }

    public void UneqipItemFromSlot(EquipmentSlot slot, out Item uneqipedItem)
    {
        if (slot.Item.ItemInfo.ItemType == ItemType.Weapon &&
           (slot.Item.ItemInfo as WeaponInfo).IsTwoHandled)
        {
            LeftHandSlot.UnequipItem();
            uneqipedItem = RightHandSlot.Item;
            RightHandSlot.UnequipItem();
        }
        else
        {
            uneqipedItem = slot.Item;
            slot.UnequipItem();
        }
    }
}