using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
    abstract class ItemSlotUI : MonoBehaviour
    {
		[SerializeField] protected Image _iconImage;

		public abstract ItemSlot ItemSlot { get; }

		protected virtual void EquipItem(ItemInfo itemInfo)
		{
			_iconImage.sprite = itemInfo.Icon;
		}
	}
}