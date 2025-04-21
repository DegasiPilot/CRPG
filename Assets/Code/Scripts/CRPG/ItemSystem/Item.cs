using CRPG.DataSaveSystem;
using UnityEngine;

[RequireComponent(typeof(SaveableGameobject))]
public abstract class Item : MonoBehaviour, ISaveBlocker
{
    public abstract ItemInfo ItemInfo { get; }

	public virtual bool IsBlockSave => IsInInventory;

	public bool IsInInventory { get; private set; }

    private Rigidbody rb;
    private new Collider collider;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void OnTaked()
    {
        rb.isKinematic = true;
        collider.enabled = false;
        IsInInventory = true;
    }

    public void OnDropped()
    {
        rb.isKinematic = false;
        collider.enabled = true;
        IsInInventory = false;
    }
}