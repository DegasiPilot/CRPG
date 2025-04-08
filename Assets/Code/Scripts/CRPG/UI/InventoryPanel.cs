using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CRPG.UI
{
	class InventoryPanel : MonoBehaviour
	{
		public ItemContextMenu ItemContextMenu;
		public ItemInfoPanel ItemInfoPanel;
		[SerializeField] private InventorySlot[] _inventorySlots;
		private ItemSlot _activeItemSlot;

		public bool IsOpen => gameObject.activeInHierarchy;

		private void OnValidate()
		{
			if (_inventorySlots == null)
			{
				_inventorySlots = GetComponentsInChildren<InventorySlot>(true);
			}
		}

		public void ProcessRaycast(List<RaycastResult> raycastResults)
		{
			if (raycastResults.Count > 0)
			{
				if (raycastResults[0].gameObject.TryGetComponent(out ItemSlot itemSlot))
				{
					if (itemSlot.Item != null)
					{
						ActivateContextMenu(itemSlot);
						return;
					}
				}
				else if (raycastResults[0].gameObject.TryGetComponent(out Button button))
				{
					if (button == ItemContextMenu.EquipmentButton)
					{
						if (button.interactable)
						{
							OnEquipButtonClick();
						}
					}
					else if (button == ItemContextMenu.InfoButton)
					{
						if (button.interactable)
						{
							OnInfoButtonClick();
						}
					}
					else if (button == ItemContextMenu.DropButton)
					{
						if (button.interactable)
						{
							OnDropButtonClick();
						}
					}
					else if (button == ItemInfoPanel.CloseButton)
					{
						ItemInfoPanel.Close();
					}
				}
			}
			ItemContextMenu.gameObject.SetActive(false);
		}

		public void ToggleInventory(List<Item> inventory)
		{
			if (!IsOpen)
			{
				gameObject.SetActive(true);
				int inventoryCount = inventory.Count;
				for (int i = 0; i < inventoryCount; i++)
				{
					if (inventory[i].IsEquiped)
					{
						continue;
					}
					_inventorySlots[i].gameObject.SetActive(true);
					_inventorySlots[i].Setup(inventory[i]);
				}
				for (int i = inventoryCount; i < _inventorySlots.Length; i++)
				{
					_inventorySlots[i].gameObject.SetActive(false);
				}
			}
			else
			{
				gameObject.SetActive(false);
			}
		}

		public void ActivateContextMenu(ItemSlot itemSlot)
		{
			ItemContextMenu.transform.position = itemSlot.transform.position;
			ItemContextMenu.gameObject.SetActive(true);
			ItemContextMenu.Setup(itemSlot.Item);
			_activeItemSlot = itemSlot;
		}

		public void OnEquipButtonClick()
		{
			if (!_activeItemSlot.Item.IsEquiped)
			{
				EquipmentManager.Instance.EquipItem(_activeItemSlot.Item, out List<Item> undressedItems);
				undressedItems.ForEach(AddToInventoryUI);
				_activeItemSlot.UnequipItem();
			}
			else
			{
				EquipmentManager.Instance.UneqipItemFromSlot((EquipmentSlot)_activeItemSlot, out Item undressedItem);
				AddToInventoryUI(undressedItem);
			}
			_activeItemSlot = null;
		}

		public void OnInfoButtonClick()
		{
			ItemInfoPanel.gameObject.SetActive(true);
			ItemInfoPanel.Setup(_activeItemSlot.Item.ItemInfo);
			_activeItemSlot = null;
		}

		public void OnDropButtonClick()
		{
			OnDropItem.Invoke(_activeItemSlot.Item);
			_activeItemSlot.gameObject.SetActive(false);
			ItemInfoPanel.Close();
			_activeItemSlot = null;
		}
		public UnityEvent<Item> OnDropItem = new();

		private void AddToInventoryUI(Item item)
		{
			foreach (var slot in _inventorySlots)
			{
				if (slot.Item == null)
				{
					slot.gameObject.SetActive(true);
					slot.Setup(item);
					break;
				}
			}
		}
	}
}
