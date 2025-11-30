using CRPG.ItemSystem;
using System;

public class EquipmentSlot : ItemSlot
{
	public readonly BodyPart BodyPart;

	public override bool IsEmpty => EquipableItem is null;

	public EquipableItem EquipableItem { get; private set; }
	public event Action<EquipableItem, BodyPart> OnEquipItem;
	public event Action<EquipableItem, BodyPart> OnUnequipItem;

	public EquipmentSlot(BodyPart bodyPart)
	{
		BodyPart = bodyPart;
	}

	public void EquipItem(EquipableItem item)
	{
		item.IsEquiped = true;
		if (EquipableItem != null)
		{
			ClearSlot();
		}
		EquipableItem = item;
		OnEquipItem?.Invoke(item, BodyPart);
	}

	public override void ClearSlot()
	{
		OnUnequipItem?.Invoke(EquipableItem, BodyPart);
		EquipableItem.IsEquiped = false;
		EquipableItem = null;
	}

	public override bool TrySetupItemContextMenu(ItemContextMenu itemContextMenu)
	{
		if (EquipableItem != null)
		{
			itemContextMenu.Setup(EquipableItem.ItemInfo, true, true);
			return true;
		}
		else
		{
			return false;
		}
	}

	public override void SetupItemInfoPanel(ItemInfoPanel itemInfoPanel)
	{
		itemInfoPanel.Setup(EquipableItem.ItemInfo);
	}

	public override bool OnEquipButtonClick(EquipmentManager equipmentManager)
	{
		if (EquipableItem != null)
		{
			equipmentManager.UnequipItemFromSlot(this);
			return true;
		}
		else
		{
			return false;
		}
	}

	public override void OnDropButtonClick(Action<Item> dropItemCallback)
	{
		dropItemCallback.Invoke(EquipableItem);
		ClearSlot();
	}
}