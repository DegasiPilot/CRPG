using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;
    public Button CloseButton;

    public void Setup(ItemInfo itemInfo)
    {
        ItemName.text = itemInfo.Name;
        ItemDescription.text = itemInfo.GetFullDescrition();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}