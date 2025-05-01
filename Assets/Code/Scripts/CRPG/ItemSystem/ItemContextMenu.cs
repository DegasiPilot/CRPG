using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemContextMenu : MonoBehaviour
{
	public TextMeshProUGUI ItemNameText;

	public Button EquipmentButton;
	[SerializeField] private TextMeshProUGUI EquipmentBtnText;
	public Button InfoButton;
	public Button DropButton;

	private void OnValidate()
	{
		if (EquipmentBtnText == null && EquipmentButton != null)
		{
			EquipmentBtnText = EquipmentButton.GetComponentInChildren<TextMeshProUGUI>();
		}
	}

	public void Setup(ItemInfo itemInfo, bool isEquipable, bool isEquiped)
	{
		ItemNameText.text = itemInfo.Name;
		EquipmentButton.interactable = isEquipable;
		EquipmentBtnText.text = isEquiped ? "Снять" : "Экипировать";
		DropButton.interactable = !isEquiped;
	}
}