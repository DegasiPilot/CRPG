using CRPG.ItemSystem;
using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
	class EquipmentSlotUI : ItemSlotUI
	{
		public Sprite defaultSprite;
		public override ItemSlot ItemSlot => _equipmentSlot;

		private EquipmentSlot _equipmentSlot;

		protected override void OnValidate()
		{
			base.OnValidate();
			if (_iconImage == null) _iconImage = transform.GetChild(0).GetComponent<Image>();
		}

		public void Setup(EquipmentSlot equipmentSlot)
		{
			ReleaseSlot();
			_equipmentSlot = equipmentSlot;
			if (_equipmentSlot.EquipableItem != null)
			{
				EquipItem(_equipmentSlot.EquipableItem.ItemInfo);
			}
			_equipmentSlot.OnEquipItem += EquipItem;
			_equipmentSlot.OnUnequipItem += UnequipItemAdapter;
		}

		internal void ReleaseSlot()
		{
			if (_equipmentSlot != null)
			{
				_equipmentSlot.OnEquipItem -= EquipItem;
				_equipmentSlot.OnUnequipItem -= UnequipItemAdapter;
				UnequipItem();
			}
		}

		private void OnDestroy()
		{
			ReleaseSlot();
		}

		private void EquipItem(EquipableItem item, BodyPart bodyPart)
		{
			EquipItem(item.ItemInfo);
		}

		private void UnequipItemAdapter(Item item, BodyPart bodyPart)
		{
			UnequipItem();
		}

		protected override void UnequipItem()
		{
			base.UnequipItem();
			_iconImage.sprite = defaultSprite;
		}
	}
}