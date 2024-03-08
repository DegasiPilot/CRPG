using UnityEngine.UI;

public class InventorySlot : ItemSlot
{
    private void Awake()
    {
        _iconImage = transform.GetChild(0).GetComponent<Image>();
    }

    public void Setup(Item item)
    {
        Item = item;
        _iconImage.sprite = Item.ItemInfo.Icon;
    }
    public override void UnequipItem()
    {
        Item = null;
        gameObject.SetActive(false);
    }
}