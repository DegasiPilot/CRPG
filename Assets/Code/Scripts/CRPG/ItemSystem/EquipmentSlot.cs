using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : ItemSlot
{
    public Sprite defaultSprite;

    private BodyPart _bodyPart;
    private Color _darkenedColor = new(0.34f, 0.34f, 0.34f);
    private EquipmentCustomizer _equipmentCustomizer;

	private void OnValidate()
	{
		if(_iconImage == null) _iconImage = transform.GetChild(0).GetComponent<Image>();
	}

	public void Setup(BodyPart bodyPart, EquipmentCustomizer equipmentCustomizer)
    {
        _bodyPart = bodyPart;
        _equipmentCustomizer = equipmentCustomizer;
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
        _equipmentCustomizer.EquipSkin(item, _bodyPart);
    }

    public override void UnequipItem()
    {
        Item.IsEquiped = false;
        Item = null;
        _iconImage.sprite = defaultSprite;
        _iconImage.color = _darkenedColor;
        _equipmentCustomizer.ResetSkin(_bodyPart);
    }
}