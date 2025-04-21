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
		}

		public void ProcessRaycast(List<RaycastResult> raycastResults)
		{
			if (raycastResults.Count > 0)
			{
				if (raycastResults[0].gameObject.TryGetComponent(out ItemSlotUI itemSlot))
				{
					if(itemSlot.ItemSlot.Item != null)
					{
						ActivateContextMenu(itemSlot);
					}
					return;
				}
				else if (raycastResults[0].gameObject.TryGetComponent(out Button button))
				{
					if (button == ItemContextMenu.EquipmentButton)
					{
						if (button.interactable)
						{
							OnEquipButtonClick();
						}
						else
						{
							return;
						}
					}
					else if (button == ItemContextMenu.InfoButton)
					{
						if (button.interactable)
						{
							OnInfoButtonClick();
						}
						else
						{
							return;
						}
					}
					else if (button == ItemContextMenu.DropButton)
					{
						if (button.interactable)
						{
							OnDropButtonClick();
						}
						else
						{
							return;
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
					if (inventory[i] is EquipableItem equipable && equipable.IsEquiped)
					{
						_inventorySlotsUI[i].InventorySlot.UnEquipItem();
					}
					else
					{
						_inventorySlotsUI[i].InventorySlot.EquipItem(inventory[i]);
					}
				}
				for (int i = inventoryCount; i < _inventorySlotsUI.Length; i++)
				{
					_inventorySlotsUI[i].InventorySlot.UnEquipItem();
				}
			}
			else
			{
				gameObject.SetActive(false);
			}
		}

		public void ActivateContextMenu(ItemSlotUI itemSlot)
		{
			ItemContextMenu.transform.position = itemSlot.transform.position;
			ItemContextMenu.gameObject.SetActive(true);
			ItemContextMenu.Setup(itemSlot.ItemSlot.Item);
			_activeItemSlot = itemSlot.ItemSlot;
		}

		public void OnEquipButtonClick()
		{
			if (_activeItemSlot.Item is EquipableItem) {
				if (_activeItemSlot is EquipmentSlot equipmentSlot)
				{
					_equipmentManager.UnequipItemFromSlot(equipmentSlot);
				}
				else
				{
					_equipmentManager.EquipItem(_activeItemSlot.Item);
					_activeItemSlot.UnEquipItem();
				}
				_activeItemSlot = null;
			}
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
			_activeItemSlot.UnEquipItem();
			ItemInfoPanel.Close();
			_activeItemSlot = null;
		}
		public UnityEvent<Item> OnDropItem = new();

		public void AddToInventoryUI(Item item, BodyPart bodyPart)
		{
			foreach (var slot in _inventorySlotsUI)
			{
				//Find first empty slot
				if (slot.InventorySlot.Item == null)
				{
					slot.InventorySlot.EquipItem(item);
					break;
				}
			}
		}

		private void Release()
		{
			if (_equipmentManager != null)
			{
				_equipmentManager.OnItemUnequiped -= AddToInventoryUI;
			}
		}

		private void OnDestroy()
		{
			Release();
		}
	}
}