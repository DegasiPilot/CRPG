using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
    class PlayerPanel : MonoBehaviour
    {
		[SerializeField] private Image PersonageImage;
		[SerializeField] private Text PersonageNameText;
		[SerializeField] private Text LifesText;
		[SerializeField] private Image LifesImage;
		[SerializeField] private Text StaminaText;
		[SerializeField] private Image StaminaImage;
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

		private float _targetLifes;
		private float _targetStamina;

		private void Update()
		{
			if (Mathf.Abs(_targetLifes - LifesImage.fillAmount) > 0.01f)
			{
				LifesImage.fillAmount = Mathf.Lerp(LifesImage.fillAmount, _targetLifes, Time.deltaTime * GameData.DefaultUIInterpolationSpeed);
			}
			if (Mathf.Abs(_targetStamina - StaminaImage.fillAmount) > 0.01f)
			{
				LifesImage.fillAmount = Mathf.Lerp(StaminaImage.fillAmount, _targetStamina, Time.deltaTime * GameData.DefaultUIInterpolationSpeed);
			}
		}

		public void UpdateHealthBar(float health, float maxHealth)
		{
			_targetLifes = health / maxHealth;
			LifesText.text = $"{health}/{maxHealth}";
		}

		public void UpdateStaminaBar(float stamina, float maxStamina)
		{
			_targetStamina = stamina / maxStamina;
			StaminaText.text = $"{stamina}/{maxStamina}";
		}
	}
}