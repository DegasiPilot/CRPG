using UnityEngine;

[RequireComponent(typeof(SaveableGameobject))]
public class Item : MonoBehaviour
{
    public ItemInfo ItemInfo;
    public bool IsInInventory;
    public bool IsEquiped;

    private Rigidbody rb;
    private new Collider collider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void OnTaked()
    {
        rb.isKinematic = true;
        collider.enabled = false;
    }

    public void OnDropped()
    {
        rb.isKinematic = false;
        collider.enabled = true;
    }
}