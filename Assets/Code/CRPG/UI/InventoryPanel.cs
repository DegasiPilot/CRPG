using CRPG.ItemSystem;
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
		[SerializeField] private InventorySlotUI[] _inventorySlotsUI;
		private ItemSlot _activeItemSlot;
		private EquipmentManager _equipmentManager;

		public bool IsOpen => gameObject.activeInHierarchy;

		private void OnValidate()
		{
			if (_inventorySlotsUI != null && _inventorySlotsUI.Length == 0)
			{
				_inventorySlotsUI = GetComponentsInChildren<InventorySlotUI>(true);
			}
		}

		public void SetActivePlayer(EquipmentManager equipmentManager)
		{
			Release();
			_equipmentManager = equipmentManager;
			_equipmentManager.OnItemUnequiped += AddToInventoryUI;
			_equipmentManager.OnProjectileUnequiped += AddToInventoryUI;
		}

		public void ProcessRaycast(List<RaycastResult> raycastResults)
		{
			if (raycastResults.Count > 0)
			{
				if (raycastResults[0].gameObject.TryGetComponent(out ItemSlotUI itemSlot))
				{
					ActivateContextMenu(itemSlot);
					return;
				}
				else if (raycastResults[0].gameObject.TryGetComponent(out Button button))
				{
					if (button.interactable)
					{
						if (button == ItemContextMenu.EquipmentButton)
						{
							OnEquipButtonClick();
						}
						else if (button == ItemContextMenu.InfoButton)
						{
							OnInfoButtonClick();
						}
						else if (button == ItemContextMenu.DropButton)
						{
							OnDropButtonClick();
						}
						else if (button == ItemInfoPanel.CloseButton)
						{
							ItemInfoPanel.Close();
						}
					}
					else
					{
						return;
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
				ClearInvenotyUI();
				for (int i = 0; i < inventoryCount; i++)
				{
					if (inventory[i] is EquipableItem equipable && equipable.IsEquiped)
					{
						continue;
					}
					else
					{
						AddToInventoryUI(inventory[i]);
					}
				}
			}
			else
			{
				gameObject.SetActive(false);
			}
		}

		public void ActivateContextMenu(ItemSlotUI itemSlot)
		{
			_activeItemSlot = itemSlot.ItemSlot;
			if (_activeItemSlot.TrySetupItemContextMenu(ItemContextMenu))
			{
				RectTransform rectTransform = itemSlot.transform as RectTransform;
				ItemContextMenu.transform.position = rectTransform.TransformPoint(rectTransform.rect.center);
				ItemContextMenu.gameObject.SetActive(true);
				_activeItemSlot = itemSlot.ItemSlot;
			}
		}

		public void OnEquipButtonClick()
		{
			if (_activeItemSlot.OnEquipButtonClick(_equipmentManager))
			{
				_activeItemSlot = null;
			}
		}

		public void OnInfoButtonClick()
		{
			ItemInfoPanel.gameObject.SetActive(true);
			_activeItemSlot.SetupItemInfoPanel(ItemInfoPanel);
			_activeItemSlot = null;
		}

		public void OnDropButtonClick()
		{
			ItemInfoPanel.Close();
			_activeItemSlot.OnDropButtonClick(InvokeDropItem);
			_activeItemSlot = null;
		}

		private void InvokeDropItem(Item item) => OnDropItem.Invoke(item);
		public UnityEvent<Item> OnDropItem = new();

		private void AddToInventoryUI(Item item, BodyPart bodyPart)
		{
			AddToInventoryUI(item);
		}

		public void ClearInvenotyUI()
		{
			foreach (var slot in _inventorySlotsUI)
			{
				slot.ItemSlot.ClearSlot();
			}
		}

		public void AddToInventoryUI(Item item)
		{
			foreach (var slot in _inventorySlotsUI)
			{
				//Find first empty slot
				if (slot.InventorySlot.CanEquipItem(item))
				{
					slot.InventorySlot.EquipItem(item);
					break;
				}
			}
		}

		public void AddToInventoryUI<T>(List<T> items) where T : Item
		{
			foreach (var slot in _inventorySlotsUI)
			{
				//Find first empty slot
				if (slot.InventorySlot.CanEquipItem(items[0]))
				{
					slot.InventorySlot.EquipItems(items);
					break;
				}
			}
		}

		private void Release()
		{
			if (_equipmentManager != null)
			{
				_equipmentManager.OnItemUnequiped -= AddToInventoryUI;
				_equipmentManager.OnProjectileUnequiped -= AddToInventoryUI;
			}
		}

		private void OnDestroy()
		{
			Release();
		}
	}
}