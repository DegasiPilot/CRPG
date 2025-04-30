using CRPG.DataSaveSystem;
using UnityEngine;

[RequireComponent(typeof(SaveableGameobject))]
public abstract class Item : MonoBehaviour, ISaveBlocker
{
    public abstract ItemInfo ItemInfo { get; }

	public virtual bool IsBlockSave => IsInInventory;

	public bool IsInInventory { get; private set; }

    [SerializeReference] private Rigidbody rb;
	[SerializeReference] private new Collider collider;

    protected virtual void OnValidate()
    {
        if(rb == null) TryGetComponent(out rb);
        if(collider == null) TryGetComponent(out collider);
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