using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemSlot : MonoBehaviour
{
    public Item Item;

    private Image _image;
    private Button _button;

    private void Awake()
    {
        _image = transform.GetChild(0).GetComponent<Image>();
        _button = transform.GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    public void Setup(Item item)
    {
        Item = item;
        _image.sprite = Item.ItemInfo.Icon;
    }

    private void OnClick()
    {
        CanvasManager.Instance.OnItemButtonbClick(this);
    }
}