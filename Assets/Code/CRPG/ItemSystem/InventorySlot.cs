using CRPG.ItemSystem;
using System;
using System.Collections.Generic;

internal class InventorySlot : ItemSlot
{
	public event Action<ItemInfo> OnEquipItem;
	public event Action OnClearSlot;

	public List<Item> Items { get; private set; }

	public override bool IsEmpty => Items is null;

	private IEnumerable<T> ItemsAs<T>() where T : Item
	{
		foreach (var item in Items)
		{
			if (item is T requiredItem)
			{
				yield return requiredItem;
			}
		}
	}

	public void EquipItem(Item item)
	{
		if (Items == null)
		{
			Items = new List<Item>() { item };
		}
		else
		{
			Items.Add(item);
		}
		OnEquipItem.Invoke(item.ItemInfo);
	}

	public void EquipItems(IEnumerable<Item> items)
	{
		if (Items == null)
		{
			Items = new List<Item>(items);
		}
		else
		{
			Items.AddRange(items);
		}
		OnEquipItem.Invoke(Items[0].ItemInfo);
	}

	public override void ClearSlot()
	{
		OnClearSlot?.Invoke();
		Items = null;
	}

	public override bool TrySetupItemContextMenu(ItemContextMenu itemContextMenu)
	{
		if (Items != null)
		{
			itemContextMenu.Setup(Items[0].ItemInfo, Items[0] is EquipableItem equipableItem, false);
			return true;
		}
		else
		{
			return false;
		}
	}

	public override bool OnEquipButtonClick(EquipmentManager equipmentManager)
	{
		if (Items[0] is EquipableItem equipableItem)
		{
			if (Items.Count > 1)
			{
				equipmentManager.EquipProjectiles(ItemsAs<ProjectileItem>());
			}
			else
			{
				equipmentManager.EquipItem(equipableItem);
			}
			ClearSlot();
			return true;
		}
		else
		{
			return false;
		}
	}

	public override void OnDropButtonClick(Action<Item> dropItemCallback)
	{
		foreach (var item in Items)
		{
			dropItemCallback.Invoke(item);
		}
		ClearSlot();
	}

	public override void SetupItemInfoPanel(ItemInfoPanel itemInfoPanel)
	{
		itemInfoPanel.Setup(Items[0].ItemInfo);
	}

	public bool CanEquipItem(Item item)
	{
		return Items == null || Items[0].ItemInfo.IsStackable && Items[0].ItemInfo == item.ItemInfo;
	}
}