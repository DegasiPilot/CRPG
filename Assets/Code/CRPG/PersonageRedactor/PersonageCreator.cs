using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonageCreator : MonoBehaviour
{
	public UnityEvent<Gender> OnGenderChanged;
	public UnityEvent<Color> PickedColorChanged => ColorPicker.onColorChange;

	public InputField NameInput;
	public Transform StatsParent;
	public Transform ApearanceControllersParent;
	public Transform RacesParent;
	public Button PickRaceButton;
	public Text RaceTitle;
	public Text RaceDescription;
	public Text StatPointsText;

	public Toggle GenderMToggle;
	public Toggle GenderFToggle;
	public FlexibleColorPicker ColorPicker;
	public Image HairColorImage;
	public Image SkinColorImage;

	[Header("Camera settings")]
	public float MouseSensitivity;
	public Camera PhotoCamera;

	[SerializeField] private Text _pickRaceBtnText;
	[SerializeField] private CharacteristicRedactor[] _statRedactors;
	public CharacteristicRedactor[] StatRedactors => _statRedactors;

	[SerializeField] private ApperancePartRedactor[] _apperanceRedactors;
	public ApperancePartRedactor[] ApperanceRedactors => _apperanceRedactors;
	[SerializeField] private RaceButtonScript[] _racesButtons;

	public Color HairColor
	{
		set => HairColorImage.color = value;
	}
	public Color SkinColor
	{
		set => SkinColorImage.color = value;
	}

	private void OnValidate()
	{
		if (_pickRaceBtnText == null)
			_pickRaceBtnText = PickRaceButton.GetComponentInChildren<Text>();
		if (_statRedactors == null)
			_statRedactors = StatsParent.GetComponentsInChildren<CharacteristicRedactor>();
		if (_apperanceRedactors == null)
			_apperanceRedactors = ApearanceControllersParent.GetComponentsInChildren<ApperancePartRedactor>();
		if (_racesButtons == null)
			_racesButtons = RacesParent.GetComponentsInChildren<RaceButtonScript>();
	}

	internal void Awake()
	{
		foreach (var button in _racesButtons)
		{
			button.AddListener(InvokeSetRace);
		}

		GenderMToggle.onValueChanged.AddListener((activation) => TogleValueChanged(activation, Gender.Male));
		GenderFToggle.onValueChanged.AddListener((activation) => TogleValueChanged(activation, Gender.Female));
	}

	public UnityEvent<string> OnNameChanged => NameInput.onEndEdit;

	private void InvokeSetRace(RaceInfo raceInfo)
	{
		SetRace?.Invoke(raceInfo);
	}

	public event Action<RaceInfo> SetRace;

	private void Update()
	{
		if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0))
		{
			float hRotation = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
			PhotoCamera.transform.parent.Rotate(Vector3.up, -hRotation);
			OnRotate?.Invoke(hRotation);
		}
	}

	public event Action<float> OnRotate;

	public void UpdateRaceInfo(RaceInfo info)
	{
		string raceName = TextHelper.Translate(info.Race);
		_pickRaceBtnText.text = $"Раса\n{raceName}";
		RaceTitle.text = raceName;
		RaceDescription.text = $"{info.Description}\nСкорость: {info.StandartSpeed}\nБазовое здоровье: {info.BaseHealth}\nБазовая выносливость: {info.BaseStamina}";
	}

	public void TrySavePersonage()
	{
		OnTrySavePersonage?.Invoke();
	}

	public event Action OnTrySavePersonage;

	public Texture2D SavePersonagePortrait()
	{
		RenderTexture.active = PhotoCamera.targetTexture;
		PhotoCamera.Render();

		Texture2D renderedTexture = new Texture2D(256, 256);
		renderedTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
		renderedTexture.Apply();
		RenderTexture.active = null;

		return renderedTexture;
	}

	public void RefreshStatPoints(int maxStatPoints, int unspendedStatPoints)
	{
		StatPointsText.text = $"{maxStatPoints - unspendedStatPoints}/{maxStatPoints}";
		StatPointsText.color = unspendedStatPoints == 0 ? Color.green : Color.red;
	}

	public void PickHairColor()
	{
		PickHairColorPressed.Invoke();
	}

	public event Action PickHairColorPressed;

	public void ActivateColorPicker(Color initColor)
	{
		if (!ColorPicker.gameObject.activeInHierarchy)
		{
			ColorPicker.gameObject.SetActive(true);
		}
		ColorPicker.color = initColor;
	}

	public void PickSkinColor()
	{
		PickSkinColorPressed.Invoke();
	}

	public event Action PickSkinColorPressed;

	private void TogleValueChanged(bool activation, Gender gender)
	{
		if (activation)
		{
			GenderChange(gender);
		}
	}

	public void GenderChange(Gender gender)
	{
		OnGenderChanged?.Invoke(gender);
	}

	public void SetGenderWithoutNotify(Gender gender)
	{
		if (gender == Gender.Male)
		{
			GenderMToggle.SetIsOnWithoutNotify(true);
		}
		else if (gender == Gender.Female)
		{
			GenderFToggle.SetIsOnWithoutNotify(true);
		}
		foreach (var redactor in _apperanceRedactors)
		{
			if (redactor.IsOnlyMenPart) redactor.gameObject.SetActive(gender == Gender.Male);
		}
	}

	public void SetNameWithoutNotify(string name) => NameInput.SetTextWithoutNotify(name);
}