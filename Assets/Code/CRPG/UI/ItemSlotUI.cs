using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
    abstract class ItemSlotUI : MonoBehaviour
    {
		protected static readonly Color _darkenedColor = new(0.34f, 0.34f, 0.34f);
		[SerializeField] protected Image _iconImage;

		public abstract ItemSlot ItemSlot { get; }

		protected virtual void EquipItem(ItemInfo itemInfo)
		{
			_iconImage.sprite = itemInfo.Icon;
			_iconImage.color = Color.white;
		}

		protected virtual void UnequipItem()
		{
			_iconImage.color = _darkenedColor;
		}
	}
}