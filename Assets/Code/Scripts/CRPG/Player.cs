using UnityEngine;

namespace CRPG
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerCustomizer))]
    internal class Player : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerController PlayerController { get; private set; }

        [field: SerializeField]
        public PlayerCustomizer PlayerCustomizer { get; private set; }

		private void OnValidate()
		{
            if (PlayerController == null) PlayerController = GetComponent<PlayerController>();
            if (PlayerCustomizer == null) PlayerCustomizer = GetComponent<PlayerCustomizer>();
		}
	}
}
