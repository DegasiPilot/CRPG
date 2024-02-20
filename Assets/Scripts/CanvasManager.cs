using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    public GameObject InventoryPanel;

    private GridLayoutGroup InventoryContainer;
    private bool IsInventoryOpen = false;
    private List<GameObject> Inventory => GameManager.Instance.PlayerPersonage.PersonageInfo.Inventory;

    public void Setup()
    {
        Instance = this;
        InventoryContainer = InventoryPanel.GetComponentInChildren<GridLayoutGroup>();
    }

    public bool ToggleInventory()
    {
        if (!IsInventoryOpen)
        {
            InventoryPanel.SetActive(true);
            for(int i = 0; i < Inventory.Count; i++)
            {
                GameObject currentButton = InventoryContainer.transform.GetChild(i).gameObject;
                currentButton.gameObject.SetActive(true);
                currentButton.transform.GetChild(0).GetComponent<Image>().sprite = Inventory[i].GetComponent<Item>().Icon; 
            }
            for(int i = Inventory.Count; i < InventoryContainer.transform.childCount; i++)
            {
                InventoryContainer.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            InventoryPanel.SetActive(false);
        }
        IsInventoryOpen = !IsInventoryOpen;
        return IsInventoryOpen;
    }
}
