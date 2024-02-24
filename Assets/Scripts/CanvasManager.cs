using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    public GameObject InventoryPanel;
    public GameObject PauseMenuPanel;

    private GridLayoutGroup InventoryContainer;
    public bool IsInventoryOpen { get; private set; } = false;
    public bool IsPauseMenuOpen { get; private set; } = false;

    public void Awake()
    {
        Instance = this;
    }

    public void Setup()
    {
        InventoryContainer = InventoryPanel.GetComponentInChildren<GridLayoutGroup>();
    }

    public void ToggleInventory()
    {
        if (!IsInventoryOpen)
        {
            InventoryPanel.SetActive(true);
            for(int i = 0; i < GameData.Inventory.Count; i++)
            {
                GameObject currentButton = InventoryContainer.transform.GetChild(i).gameObject;
                currentButton.gameObject.SetActive(true);
                currentButton.transform.GetChild(0).GetComponent<Image>().sprite = GameData.Inventory[i].GetComponent<Item>().ItemInfo.Icon; 
            }
            for(int i = GameData.Inventory.Count; i < InventoryContainer.transform.childCount; i++)
            {
                InventoryContainer.transform.GetChild(i).gameObject.SetActive(false);
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
}
