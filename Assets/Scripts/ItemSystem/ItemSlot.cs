using UnityEngine;
using UnityEngine.UI;

public abstract class ItemSlot : MonoBehaviour
{
    public Item Item { get; protected set; }

    protected Image _iconImage;

    public abstract void UnequipItem();
}