using CRPG.ItemSystem;
using UnityEngine;
using UnityEngine.UI;

public class ItemContextMenu : MonoBehaviour
{
    public Text ItemNameText;

    public Button EquipmentButton;
    [SerializeField] private Text EquipmentBtnText;
    public Button InfoButton;
    public Button DropButton;

	private void OnValidate()
	{
		if(EquipmentBtnText == null && EquipmentButton != null)
        {
			EquipmentBtnText = EquipmentButton.GetComponentInChildren<Text>();
		}
	}

    public void Setup(Item item)
    {
        ItemNameText.text = item.ItemInfo.Name;
        if(item is EquipableItem equipable)
        {
			EquipmentButton.interactable = true;
			EquipmentBtnText.text = equipable.IsEquiped ? "Снять" : "Экипировать";
			DropButton.interactable = !equipable.IsEquiped;
		}
        else
        {
			EquipmentButton.interactable = false;
			EquipmentButton.interactable = false;
            DropButton.interactable = true;
        }
    }
}