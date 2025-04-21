using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
    abstract class ItemSlotUI : MonoBehaviour
    {
		[SerializeField] protected Image _iconImage;

		public abstract ItemSlot ItemSlot { get; }
	}
}