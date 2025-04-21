using UnityEngine;

namespace CRPG
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(EquipmentManager))]
    internal class Player : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerController PlayerController { get; private set; }

		[field: SerializeField]
		public EquipmentManager EquipmentManager { get; private set; }

		protected virtual void OnValidate()
		{
            if (PlayerController == null) PlayerController = GetComponent<PlayerController>();
            if (EquipmentManager == null) EquipmentManager = GetComponent<EquipmentManager>();
		}
	}
}
