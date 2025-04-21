using UnityEngine;

namespace CRPG
{
    [RequireComponent(typeof(PlayerCustomizer))]
    class MainPlayer : Player
    {
		[field: SerializeField]
		public PlayerCustomizer PlayerCustomizer { get; private set; }

		protected override void OnValidate()
		{
			base.OnValidate();
			if (PlayerCustomizer == null) PlayerCustomizer = GetComponent<PlayerCustomizer>();
		}

		private void Awake()
		{
			PlayerCustomizer.Setup(PlayerController.Personage);
		}
	}
}