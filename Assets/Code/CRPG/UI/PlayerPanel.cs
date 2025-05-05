using DegasiPilot.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
    class PlayerPanel : MonoBehaviour
    {
		[SerializeField] private Image PersonageImage;
		[SerializeField] private TextMeshProUGUI PersonageNameText;
		[SerializeField] private LabeledProgressbar LifesProgressbar;
		[SerializeField] private LabeledProgressbar StaminaProgressbar;
		[SerializeField] private PersonageActionsView _personageActionsView;
		public PersonageActionsView PersonageActionsView => _personageActionsView;

		public string PlayerName
		{
			get => PersonageNameText.text;
			set => PersonageNameText.text = value;
		}

		public Texture2D PlayerPortrait
		{
			set
			{
				PersonageImage.sprite = Sprite.Create(
				value,
				new Rect(Vector2.zero, new Vector2(value.width, value.height)),
				Vector2.zero);
				PersonageImage.preserveAspect = true;
			}
		}

		public void UpdateHealthBar(float health, float maxHealth)
		{
			LifesProgressbar.Refresh(health, maxHealth);
		}

		public void UpdateStaminaBar(float stamina, float maxStamina)
		{
			StaminaProgressbar.Refresh(stamina, maxStamina);
		}
	}
}