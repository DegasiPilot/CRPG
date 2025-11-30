using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
	class InventorySlotUI : ItemSlotUI
	{
		[SerializeField] private TextMeshProUGUI _text;

		private InventorySlot _inventorySlot;
		public InventorySlot InventorySlot
		{
			get
			{
				if (_inventorySlot == null)
				{
					_inventorySlot = new InventorySlot();
					_inventorySlot.OnEquipItem += EquipItem;
					_inventorySlot.OnClearSlot += UnequipItem;
				}
				return _inventorySlot;
			}
		}
		public override ItemSlot ItemSlot => InventorySlot;

		protected override void OnValidate()
		{
			base.OnValidate();
			if (_iconImage == null) _iconImage = transform.GetChild(0).GetComponent<Image>();
		}

		private void OnDestroy()
		{
			if (_inventorySlot != null)
			{
				_inventorySlot.OnEquipItem -= EquipItem;
				_inventorySlot.OnClearSlot -= UnequipItem;
			}
		}

		protected override void EquipItem(ItemInfo item)
		{
			base.EquipItem(item);
			_iconImage.enabled = true;
			if (item.IsStackable)
			{
				_text.text = _inventorySlot.Items.Count.ToString();
			}
			else
			{
				_text.text = string.Empty;
			}
		}

		protected override void UnequipItem()
		{
			_iconImage.enabled = false;
			_text.text = string.Empty;
		}
	}
}