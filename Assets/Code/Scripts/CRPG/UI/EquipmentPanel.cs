using UnityEngine;

namespace CRPG.UI
{
    class EquipmentPanel : MonoBehaviour
    {
		[SerializeField] private EquipmentSlotUI HealmetSlot;
		[SerializeField] private EquipmentSlotUI BodySlot;
		[SerializeField] private EquipmentSlotUI LeftHandSlot;
		[SerializeField] private EquipmentSlotUI RightHandSlot;
		[SerializeField] private EquipmentSlotUI BootsSlot;

		public void Setup(EquipmentManager equipmentManager)
		{
			HealmetSlot.Setup(equipmentManager.HealmetSlot);
			BodySlot.Setup(equipmentManager.BodySlot);
			LeftHandSlot.Setup(equipmentManager.LeftHandSlot);
			RightHandSlot.Setup(equipmentManager.RightHandSlot);
			BootsSlot.Setup(equipmentManager.BootsSlot);
		}

		private void OnDisable()
		{
			HealmetSlot.ReleaseSlot();
			BodySlot.ReleaseSlot();
			LeftHandSlot.ReleaseSlot();
			RightHandSlot.ReleaseSlot();
			BootsSlot.ReleaseSlot();
		}
	}
}