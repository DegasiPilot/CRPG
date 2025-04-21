using UnityEngine.UI;

namespace CRPG.UI
{
    class InventorySlotUI : ItemSlotUI
    {
		private InventorySlot _inventorySlot;
		public InventorySlot InventorySlot
		{
			get
			{
				if(_inventorySlot == null)
				{
					_inventorySlot = new InventorySlot();
					_inventorySlot.OnEquipItem += EquipItem;
					_inventorySlot.OnUnequipItem += UnequipItem;
				}
				return _inventorySlot;
			}
		}
		public override ItemSlot ItemSlot => InventorySlot;

		private void OnValidate()
		{
			if(_iconImage == null) _iconImage = transform.GetChild(0).GetComponent<Image>();
		}

		private void OnDestroy()
		{
			if(_inventorySlot != null)
			{
				_inventorySlot.OnEquipItem -= EquipItem;
				_inventorySlot.OnUnequipItem -= UnequipItem;
			}
		}

		private void EquipItem(Item item)
		{
			gameObject.SetActive(true);
			_iconImage.sprite = item.ItemInfo.Icon;
		}

		private void UnequipItem(Item item)
		{
			gameObject.SetActive(false);
		}
	}
}