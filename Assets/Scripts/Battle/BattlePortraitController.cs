using UnityEngine;
using UnityEngine.UI;

public class BattlePortraitController : MonoBehaviour
{
    public RawImage PortaitImage;
    public Image DamagedImage;
    [System.NonSerialized] public Personage Personage;


    private Color _baseColor;
    private float _damagedPart => 1f - (float)Personage.CurrentHealth / Personage.MaxHealth;

    public void Setup(Personage personage)
    {
        Personage = personage;
        PortaitImage.texture = personage.PersonageInfo.PersonagePortrait;
        _baseColor = personage.PersonageInfo.PersonagePortraitColor;
        PortaitImage.color = _baseColor;
        OnUpdateHealth();
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

    public void OnUpdateHealth()
    {
        DamagedImage.fillAmount = _damagedPart;
    }
}