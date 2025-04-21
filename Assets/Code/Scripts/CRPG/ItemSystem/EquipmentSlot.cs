using CRPG.ItemSystem;
using System;

public class EquipmentSlot : ItemSlot
{
    public readonly BodyPart BodyPart;

    public EquipableItem EquipableItem { get; private set; }
	public override Item Item => EquipableItem;
	public event Action<EquipableItem, BodyPart> OnEquipItem;
	public event Action<EquipableItem, BodyPart> OnUnequipItem;

    public EquipmentSlot(BodyPart bodyPart)
    {
        BodyPart = bodyPart;
    }

	public void EquipItem(EquipableItem item, out EquipableItem lastItem, bool darkened = false)
    {
        item.IsEquiped = true;
        if(EquipableItem != null)
        {
            lastItem = EquipableItem;
            UnEquipItem();
        }
        else
        {
            lastItem = null;
        }
		EquipableItem = item;
		OnEquipItem?.Invoke(item, BodyPart);
    }

    public override void UnEquipItem()
    {
        OnUnequipItem?.Invoke(EquipableItem, BodyPart);
        EquipableItem.IsEquiped = false;
		EquipableItem = null;
    }
}