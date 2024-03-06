using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    public GameObject InventoryPanel;
    public GameObject ItemContextMenu;
    public GameObject PauseMenuPanel;

    public bool IsInventoryOpen { get; private set; } = false;
    public bool IsPauseMenuOpen { get; private set; } = false;

    private List<ItemSlot> _inventorySlots;
    private ItemSlot _activeItemSlot;

    public void Awake()
    {
        Instance = this;
    }

    public void Setup()
    {
        _inventorySlots = InventoryPanel.GetComponentsInChildren<ItemSlot>(true).ToList();
    }

    public void ToggleInventory()
    {
        if (!IsInventoryOpen)
        {
            InventoryPanel.SetActive(true);
            for(int i = 0; i < GameData.Inventory.Count; i++)
            {
                _inventorySlots[i].gameObject.SetActive(true);
                _inventorySlots[i].Setup(GameData.Inventory[i]);
            }
            for(int i = GameData.Inventory.Count; i < _inventorySlots.Count; i++)
            {
                _inventorySlots[i].gameObject.SetActive(false);
            }
        }
        else
        {
            InventoryPanel.SetActive(false);
        }
        IsInventoryOpen = !IsInventoryOpen;
    }

    public void TogglePauseMenu()
    {
        IsPauseMenuOpen = !IsPauseMenuOpen;
        PauseMenuPanel.SetActive(IsPauseMenuOpen);
    }

    public void OnItemButtonbClick(ItemSlot itemSlot)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)InventoryPanel.transform, 
            Input.mousePosition, null, out Vector2 localPos);
        ((RectTransform)ItemContextMenu.transform).anchoredPosition = localPos;
        ItemContextMenu.SetActive(true);
        _activeItemSlot = itemSlot;
    }

    public void OnEquipButtonClick()
    {

    }

    public void OnDropButtonClick()
    {
        GameManager.Instance.PlayerController.DropItem(_activeItemSlot.Item);
        _activeItemSlot.gameObject.SetActive(false);
        ItemContextMenu.SetActive(false);
    }
}