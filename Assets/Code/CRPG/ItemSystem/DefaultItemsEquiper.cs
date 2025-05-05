using UnityEngine;

namespace CRPG.ItemSystem
{
    class DefaultItemsEquiper : MonoBehaviour
    {
		[SerializeField] private EquipmentManager _equipmentManager;

		[Header("Items")]
		[SerializeField] private EquipableItem Head;
		[SerializeField] private EquipableItem Body;
		[SerializeField] private EquipableItem Boots;
		[SerializeField] private EquipableItem LHand;
		[SerializeField] private EquipableItem RHand;
		[SerializeField] private ProjectileItem[] InitProjectileItems;

		private void Awake()
		{
			if (Head != null) _equipmentManager.HealmetSlot.EquipItem(Head);
			if (Body != null) _equipmentManager.BodySlot.EquipItem(Body);
			if (Boots != null) _equipmentManager.BootsSlot.EquipItem(Boots);
			if (LHand != null) _equipmentManager.LeftHandSlot.EquipItem(LHand);
			if (RHand != null) _equipmentManager.RightHandSlot.EquipItem(RHand);
			if (InitProjectileItems != null)
			{
				foreach (var item in InitProjectileItems)
				{
					_equipmentManager.ProjectileSlot.EquipProjectile(item);
				}
			}
		}
	}
}
