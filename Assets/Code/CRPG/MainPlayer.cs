using UnityEngine;

namespace CRPG
{
	[RequireComponent(typeof(PlayerCustomizer))]
	class MainPlayer : MonoBehaviour
	{
		[field: SerializeField]
		public PlayerController PlayerController { get; private set; }

		[field: SerializeField]
		public PlayerCustomizer PlayerCustomizer { get; private set; }

		protected void OnValidate()
		{
			if (PlayerController == null) PlayerController = GetComponent<PlayerController>();
			if (PlayerCustomizer == null) PlayerCustomizer = GetComponent<PlayerCustomizer>();
		}

		private void Awake()
		{
			PlayerCustomizer.Setup(PlayerController.Personage);
		}
	}
}