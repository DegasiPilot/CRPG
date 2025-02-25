using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;

public class PersonageCreator : MonoBehaviour
{
    public static PersonageCreator Instance;
    public UnityEvent OnNoMoreStatPoints;
    public UnityEvent OnGetStatPoints;
    public UnityEvent<Gender> OnGenderChanged;

    public Transform StatsParent;
    public Light MainLight;
    public Transform ApearanceControllersParent;
    public Button RaceButton;
    public Text RaceTitle;
    public Text RaceDescription;
    public Text StatPointsText;

    public Toggle GenderMToggle;
    public Toggle GenderFToggle;
    public FlexibleColorPicker ColorPicker;
    public Image HairColorImage;
    public Image SkinColorImage;

    public List<GameObject> Hairs => GameData.PlayerCustomizer.Hairs;
    public List<GameObject> Faces => GameData.PlayerCustomizer.Faces;
    public List<GameObject> Beards => GameData.PlayerCustomizer.Beards;

    private GameObject MaleObject => GameData.PlayerCustomizer.MaleObject;
    private GameObject FemaleObject => GameData.PlayerCustomizer.FemaleObject;

    [Header("Camera settings")]
    public float MouseSensitivity;
    public Camera PhotoCamera;

    private PersonageInfo _personageInfo;
    private Text _raceBtnText;
    private const int _minPointsForStat = 8;
    private const int _maxPointsForStat = 15;
    private CharacteristicRedactor[] _statRedactors;
    private ApperancePartRedactor[] _apperanceRedactors;

    private int _maxStatPointForSpent => _personageInfo.Race == Race.Human ? 27 + _humanStatPointsBonus : 27;
    private const int _humanStatPointsBonus = 5;


    private void Awake()
    {
        Instance = this;

        _personageInfo = ScriptableObject.CreateInstance<PersonageInfo>();
        _personageInfo.Name = name;
        _personageInfo.ResetStats();
        _personageInfo.UnSpendedStatPoints = _maxStatPointForSpent;
        GameData.PlayerPersonageInfo = _personageInfo;
        _raceBtnText = RaceButton.GetComponentInChildren<Text>();
        _statRedactors = StatsParent.GetComponentsInChildren<CharacteristicRedactor>();
        foreach (var redactor in _statRedactors)
        {
            redactor.Setup();
        }
        _apperanceRedactors = ApearanceControllersParent.GetComponentsInChildren<ApperancePartRedactor>();
        foreach (var redactor in _apperanceRedactors)
        {
            redactor.Setup();
        }
        SetRace(Race.Human);
        RefreshStatPoints();
        Color hairColor = GameData.PlayerCustomizer.GetHairsColor();
        hairColor.a = 1;
        HairColorImage.color = hairColor;
        Color skinColor = GameData.PlayerCustomizer.GetSkinColor();
        skinColor.a = 1;
        SkinColorImage.color = skinColor;

        GenderMToggle.onValueChanged.AddListener((activation) => TogleValueChanged(activation, Gender.Male));
        GenderFToggle.onValueChanged.AddListener((activation) => TogleValueChanged(activation, Gender.Female));
        GenderMToggle.isOn = true;
    }

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0))
        {
            float hRotation = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            MaleObject.transform.Rotate(Vector3.up, -hRotation);
            FemaleObject.transform.Rotate(Vector3.up, -hRotation);
            PhotoCamera.transform.parent.Rotate(Vector3.up, -hRotation);
            MainLight.transform.Rotate(Vector3.up, -hRotation);
        }
    }

    public void AddStatPoint(Characteristics characteristic)
    {
        if (CanAddMore(characteristic))
        {
            _personageInfo[characteristic]++;
            _personageInfo.UnSpendedStatPoints -= CostOfStatPoint(_personageInfo[characteristic]);
            RefreshStatPoints();
            if (_personageInfo.UnSpendedStatPoints == 0)
            {
                OnNoMoreStatPoints?.Invoke();
            }
        }
    }

    public void RemoveStatPoint(Characteristics characteristic)
    {
        if (CanRemoveMore(characteristic))
        {
            _personageInfo.UnSpendedStatPoints += CostOfStatPoint(_personageInfo[characteristic]);
            _personageInfo[characteristic]--;
            if (_personageInfo.UnSpendedStatPoints <= 2)
            {
                OnGetStatPoints?.Invoke();
            }
            RefreshStatPoints();
        }
    }

    public (int, int) GetStatValue(Characteristics characteristic)
    {
        return (_personageInfo[characteristic], GameData.RaceInfos.First(x => x.Race == _personageInfo.Race)[characteristic]);
    }

    public bool CanAddMore(Characteristics characteristic)
    {
        return _personageInfo[characteristic] < _maxPointsForStat && _personageInfo.UnSpendedStatPoints >= CostOfStatPoint(_personageInfo[characteristic] + 1);
    }

    public bool CanRemoveMore(Characteristics characteristic)
    {
        return _personageInfo[characteristic] > _minPointsForStat;
    }

    public void SetRace(Race race)
    {
        if (race != _personageInfo.Race)
        {
            var info = GameData.RaceInfos.First(x => x.Race == race);
            string raceName = Translator.Translate(race);
            _raceBtnText.text = $"Раса\n{raceName}";
            RaceTitle.text = raceName;
            RaceDescription.text = $"{info.Description}\nСкорость: {info.StandartSpeed}\nБазовое здоровье: {info.BaseHealth}";
            RemoveOldRaceBonus();
            _personageInfo.Race = race;
            ApplyRaceBonus(race);
            RefreshStatPoints();
            foreach (var redactor in _statRedactors)
            {
                redactor.UpdateAmount();
            }
        }
    }

    private void RemoveOldRaceBonus()
    {
        switch (_personageInfo.Race)
        {
            case Race.Human:
                if (_personageInfo.UnSpendedStatPoints < _humanStatPointsBonus)
                {
                    ResetStatPoints();
                }
                else
                {
                    _personageInfo.UnSpendedStatPoints -= _humanStatPointsBonus;
                }
                break;
        }
    }

    private void ApplyRaceBonus(Race race)
    {
        switch (race)
        {
            case Race.Human:
                _personageInfo.UnSpendedStatPoints += _humanStatPointsBonus;
                break;
        }
    }

    private void ResetStatPoints()
    {
        _personageInfo.UnSpendedStatPoints = _maxStatPointForSpent;
        if (_personageInfo.Race == Race.Human) _personageInfo.UnSpendedStatPoints += _humanStatPointsBonus;
        _personageInfo.ResetStats();
        OnGetStatPoints?.Invoke();
        RefreshStatPoints();
    }

    public void TrySavePersonage()
    {
        if (_personageInfo.UnSpendedStatPoints == 0 && !string.IsNullOrEmpty(_personageInfo.Name))
        {
            ApplyBonuses();
            PersonageInfo.AppearanceStruct appearance = new PersonageInfo.AppearanceStruct();
            foreach (var redactor in _apperanceRedactors)
            {
                switch (redactor.MyAppearancePart)
                {
                    case AppearancePart.Hairs:
                        appearance.HairIndex = redactor.ActiveIndex;
                        break;
                    case AppearancePart.Beard:
                        appearance.BeardIndex = redactor.ActiveIndex;
                        break;
                    case AppearancePart.Face:
                        appearance.FaceIndex = redactor.ActiveIndex;
                        break;
                }
            }
            appearance.HairsColor = HairColorImage.color;
            appearance.SkinColor = SkinColorImage.color;
            _personageInfo.Appearance = appearance;
            _personageInfo.ImageBytes = SavePersonagePortrait();
            _personageInfo.PersonagePortrait = new Texture2D(256, 256);
            _personageInfo.PersonagePortrait.LoadImage(_personageInfo.ImageBytes);
            CRUD.CreatePersonageInfo(_personageInfo);

            GameData.NewGameSave();
            GameData.InitializeNewGame(_personageInfo);
            MaleObject.transform.rotation = Quaternion.identity;
            FemaleObject.transform.rotation = Quaternion.identity;
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.Log("false");
        }
    }

    byte[] SavePersonagePortrait()
    {
        RenderTexture screenTexture = new RenderTexture(256, 256, 32);
        PhotoCamera.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        PhotoCamera.Render();

        Texture2D renderedTexture = new Texture2D(256, 256);
        renderedTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        RenderTexture.active = null;

        return renderedTexture.EncodeToPNG();
    }

    private void ApplyBonuses()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Characteristics)).Length; i++)
        {
            _personageInfo[(Characteristics)i] += GameData.RaceInfos.First(x => x.Race == _personageInfo.Race)[(Characteristics)i];
        }
    }

    private void RefreshStatPoints()
    {
        StatPointsText.text = $"{_maxStatPointForSpent - _personageInfo.UnSpendedStatPoints}/{_maxStatPointForSpent}";
        StatPointsText.color = _personageInfo.UnSpendedStatPoints == 0 ? Color.green : Color.red;
    }

    public void SetName(string name)
    {
        _personageInfo.Name = name;
    }

    private int CostOfStatPoint(int statPointNunber)
    {
        return statPointNunber <= 13 ? 1 : 2;
    }

    public void PickHairColor()
    {
        ColorPicker.onColorChange.RemoveAllListeners();
        ColorPicker.gameObject.SetActive(true);
        ColorPicker.color = HairColorImage.color;
        ColorPicker.onColorChange.AddListener(GameData.PlayerCustomizer.ChangeHairColor);
        ColorPicker.onColorChange.AddListener(color => HairColorImage.color = color);
    }

    public void PickSkinColor()
    {
        ColorPicker.onColorChange.RemoveAllListeners();
        ColorPicker.gameObject.SetActive(true);
        ColorPicker.color = SkinColorImage.color;
        ColorPicker.onColorChange.AddListener(GameData.PlayerCustomizer.ChangeSkinColor);
        ColorPicker.onColorChange.AddListener(color => SkinColorImage.color = color);
    }

    private void TogleValueChanged(bool activation, Gender gender)
    {
        if (activation)
        {
            GenderChange(gender);
        }
    }

    public void GenderChange(Gender gender)
    {
        GameData.PlayerCustomizer.ChangeGender(gender);
        _personageInfo.Gender = gender;
        OnGenderChanged?.Invoke(gender);
    }
}