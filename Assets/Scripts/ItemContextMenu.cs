using UnityEngine;
using UnityEngine.UI;

public class ItemContextMenu : MonoBehaviour
{
    public Button EquipmentButton;
    public Text EquipmentBtnText;
    public Button DropButton;


    private void Awake()
    {
        EquipmentBtnText = EquipmentButton.GetComponentInChildren<Text>();
    }

    public void Setup(Item item)
    {
        bool isEquipable = item.ItemInfo.ItemType != ItemType.Other;
        EquipmentButton.interactable = isEquipable;
        if (isEquipable)
        {
            EquipmentBtnText.text = item.IsEquiped ? "Снять" : "Экипировать";
            DropButton.interactable = !item.IsEquiped;
        }
        else
        {
            EquipmentBtnText.text = "Экипировать";
            DropButton.interactable = true;
        }
    }
}