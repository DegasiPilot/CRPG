using DegasiPilot.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

public class BattlePortraitController : MonoBehaviour
{
    public RawImage PortaitImage;
    [SerializeField] internal ProgressBar HealthBar;
	[SerializeField] internal ProgressBar StaminaBar;
    [System.NonSerialized] public Personage Personage;

    private Color _baseColor;

    public void Setup(Personage personage)
    {
        Personage = personage;
        PortaitImage.texture = personage.PersonageInfo.PersonagePortrait;
        _baseColor = personage.PersonageInfo.PersonagePortraitColor;
        PortaitImage.color = _baseColor;
        Personage.OnHealthChanged.AddListener(UpdateHealth);
        Personage.OnStaminaChanged.AddListener(UpdateStamina);
        UpdateHealth();
        UpdateStamina();
    }

    public void SetPrepaired()
    {
        PortaitImage.color = _baseColor;
    }
    
    public void Activate()
    {
        transform.localScale = Vector3.one * 1.25f;
    }
    
    public void Disactivate()
    {
        transform.localScale = Vector3.one;
        PortaitImage.color = Color.grey;
    }

    public void UpdateHealth()
    {
        HealthBar.Refresh(Personage.Health, Personage.MaxHealth);
    }

	public void UpdateStamina()
	{
		StaminaBar.Refresh(Personage.Stamina, Personage.MaxStamina);
	}
}