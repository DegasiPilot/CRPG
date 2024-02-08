using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public class PersonageCreator : MonoBehaviour
{
    public static PersonageCreator Instance;

    public Transform StatsParent;
    public Button RaceButton;
    public Text RaceTitle;
    public Text RaceDescription;
    public Text StatPointsText;

    private PersonageInfo _personage;
    private RaceInfo[] _raceInfos;
    private Text _raceBtnText;

    private void Awake()
    {
        Instance = this;
        _personage = ScriptableObject.CreateInstance<PersonageInfo>();
        _raceInfos = Resources.LoadAll<RaceInfo>("RacesInfo");
        var statRedactors = StatsParent.GetComponentsInChildren<CharacteristicRedactor>();
        foreach (var redactor in statRedactors)
        {
            redactor.Setup();
        }
        _raceBtnText = RaceButton.GetComponentInChildren<Text>();
        SetStatPoints(_personage.UnSpendedStatPoints);
    }

    public void AddStatPoint(Characteristics characteristic, out bool canAddMore)
    {
        if (_personage.Stats[characteristic] < 18 && _personage.UnSpendedStatPoints > 0)
        {
            _personage.Stats[characteristic]++;
            SetStatPoints(--_personage.UnSpendedStatPoints);
            canAddMore = _personage.Stats[characteristic] < 18 && _personage.UnSpendedStatPoints > 0;
        }
        else
        {
            canAddMore = false;
        }
    }

    public void RemoveStatPoint(Characteristics characteristic, out bool canRemoveMore)
    {
        if (_personage.Stats[characteristic] > 0)
        {
            _personage.Stats[characteristic]--;
            SetStatPoints(++_personage.UnSpendedStatPoints);
            canRemoveMore = _personage.Stats[characteristic] > 0;
        }
        else
        {
            canRemoveMore = false;
        }
    }

    public int GetCharacteristicValue(Characteristics characteristic)
    {
        return _personage.Stats[characteristic];
    }

    public void SetRace(Race race)
    {
        var info = _raceInfos.First(x => x.Race == race);
        string raceName = Translator.Translate(race.ToString());
        _raceBtnText.text = $"Раса\n{raceName}";
        RaceTitle.text = raceName;
        RaceDescription.text = info.Description;
        _personage.RaceInfo = info;
    }

    public void TrySavePersonage()
    {
        if(_personage.RaceInfo && _personage.UnSpendedStatPoints == 0)
        {
            string number = Resources.FindObjectsOfTypeAll<PersonageInfo>().Length.ToString();
            AssetDatabase.CreateAsset(_personage, "Assets/Resources/PersonageInfo/Test" + number + ".asset");
            Debug.Log("true");
        }
        else
        {
            Debug.Log("false");
        }
    }

    private void SetStatPoints(int unSpendedPoints)
    {
        StatPointsText.text = 12 - unSpendedPoints + "/" + 12;
        StatPointsText.color = unSpendedPoints == 0 ? Color.green : Color.red;
    }

    public void SetName(string name)
    {
        _personage.Name = name;
    }
}