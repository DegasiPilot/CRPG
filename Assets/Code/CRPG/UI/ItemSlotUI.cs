using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CRPG.UI
{
	abstract class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		protected static readonly Color _darkenedColor = new(0.34f, 0.34f, 0.34f);
		[SerializeField] protected Image _iconImage;
		[SerializeField, InspectorName("Подсветка")] protected Image _hightlightImage;

		public abstract ItemSlot ItemSlot { get; }

		protected virtual void OnValidate()
		{
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if(ItemSlot != null && ItemSlot.IsEmpty == false)
			{
				_hightlightImage.enabled = true;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_hightlightImage.enabled = false;
		}

		protected virtual void EquipItem(ItemInfo itemInfo)
		{
			_iconImage.sprite = itemInfo.Icon;
			_iconImage.color = Color.white;
		}

		protected virtual void UnequipItem()
		{
			_iconImage.color = _darkenedColor;
			_hightlightImage.enabled = false;
		}
	}
}