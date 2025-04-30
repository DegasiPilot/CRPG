using CRPG.ItemSystem;
using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
	class EquipmentSlotUI : ItemSlotUI
	{
		private static readonly Color _darkenedColor = new(0.34f, 0.34f, 0.34f);

		public Sprite defaultSprite;
		public override ItemSlot ItemSlot => _equipmentSlot;

		private EquipmentSlot _equipmentSlot;

		private void OnValidate()
		{
			if (_iconImage == null) _iconImage = transform.GetChild(0).GetComponent<Image>();
		}

		public void Setup(EquipmentSlot equipmentSlot)
		{
			ReleaseSlot();
			_equipmentSlot = equipmentSlot;
			if(_equipmentSlot.EquipableItem != null)
			{
				EquipItem(_equipmentSlot.EquipableItem.ItemInfo);
			}
			_equipmentSlot.OnEquipItem += EquipItem;
			_equipmentSlot.OnUnequipItem += UnequipItemAdapter;
		}

		public void ReleaseSlot()
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
			EquipItem(item.ItemInfo,
				item is Weapon weapon &&
				weapon.WeaponInfo.IsTwoHandled &&
				bodyPart == BodyPart.LeftHand);
		}

		private void EquipItem(ItemInfo itemInfo, bool darkened = false)
		{
			EquipItem(itemInfo);
			_iconImage.color = darkened ? _darkenedColor : Color.white;
		}

		private void UnequipItemAdapter(Item item, BodyPart bodyPart)
		{
			UnequipItem();
		}

		private void UnequipItem()
		{
			_iconImage.sprite = defaultSprite;
			_iconImage.color = _darkenedColor;
		}
	}
}