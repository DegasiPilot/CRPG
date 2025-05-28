using System;

public abstract class ItemSlot
{
	public abstract void ClearSlot();
	public abstract bool TrySetupItemContextMenu(ItemContextMenu itemContextMenu);
	public abstract void SetupItemInfoPanel(ItemInfoPanel itemInfoPanel);
	public abstract bool OnEquipButtonClick(EquipmentManager equipmentManager);
	public abstract void OnDropButtonClick(Action<Item> dropItemCallback);
}