using System;

internal class InventorySlot : ItemSlot
{
	public override Item Item => _item;
	public event Action<Item> OnEquipItem;
	public event Action<Item> OnUnequipItem;

	private Item _item;

	public void EquipItem(Item item)
    {
		_item = item;
		OnEquipItem.Invoke(item);
	}

    public override void UnEquipItem()
    {
		OnUnequipItem?.Invoke(_item);
        _item = null;
    }
}