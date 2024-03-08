using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : ItemSlot
{
    public Sprite defaultSprite;

    private Color _darkenedColor = new(0.34f, 0.34f, 0.34f);

    public void Setup()
    {
        _iconImage = transform.GetChild(0).GetComponent<Image>();
    }

    public void EquipItem(Item item, out Item lastItem, bool darkened = false)
    {
        item.IsEquiped = true;
        if(Item != null)
        {
            lastItem = Item;
            UnequipItem();
        }
        else
        {
            lastItem = null;
        }
        Item = item;
        _iconImage.sprite = item.ItemInfo.Icon;
        _iconImage.color = darkened? _darkenedColor : Color.white;
    }

    public override void UnequipItem()
    {
        Item.IsEquiped = false;
        Item = null;
        _iconImage.sprite = defaultSprite;
        _iconImage.color = _darkenedColor;
    }
}