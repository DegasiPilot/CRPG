using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PersonageCreator : MonoBehaviour
{
    public static PersonageCreator Instance;
    public UnityEvent OnNoMoreStatPoints;
    public UnityEvent OnGetStatPoints;

    public Transform StatsParent;
    public Button RaceButton;
    public Text RaceTitle;
    public Text RaceDescription;
    public Text StatPointsText;
    public List<RaceInfo> RaceInfos;

    private PersonageInfo _personage;
    private Text _raceBtnText;

    private void Awake()
    {
        Instance = this;
        _personage = new PersonageInfo();
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
        if (_personage[characteristic] < 18 && _personage.UnSpendedStatPoints > 0)
        {
            _personage[characteristic]++;
            SetStatPoints(--_personage.UnSpendedStatPoints);
            canAddMore = _personage[characteristic] < 18 && _personage.UnSpendedStatPoints > 0;
            if (_personage.UnSpendedStatPoints == 0)
            {
                OnNoMoreStatPoints.Invoke();
            }
        }
        else
        {
            canAddMore = false;
        }
    }

    public void RemoveStatPoint(Characteristics characteristic, out bool canRemoveMore)
    {
        if (_personage[characteristic] > 0)
        {
            if (_personage.UnSpendedStatPoints == 0)
            {
                OnGetStatPoints.Invoke();
            }
            _personage[characteristic]--;
            SetStatPoints(++_personage.UnSpendedStatPoints);
            canRemoveMore = _personage[characteristic] > 0;
        }
        else
        {
            canRemoveMore = false;
        }
    }

    public int GetCharacteristicValue(Characteristics characteristic)
    {
        return _personage[characteristic];
    }

    public bool CanAddMore(Characteristics characteristic)
    {
        return _personage[characteristic] < 18; 
    }

    public void SetRace(Race race)
    {
        var info = RaceInfos.First(x => x.Race == race);
        string raceName = Translator.Translate(race.ToString());
        _raceBtnText.text = $"����\n{raceName}";
        RaceTitle.text = raceName;
        RaceDescription.text = info.Description;
        _personage.Race = race;
    }

    public void TrySavePersonage()
    {
        if(_personage.Race != Race.None && _personage.UnSpendedStatPoints == 0 && !string.IsNullOrEmpty(_personage.Name))
        {
            CRUD.CreatePersonageInfo(_personage);
            GameData.PlayerPersonage = _personage;
            GameData.NewGameSave();
            SceneManager.LoadScene("SampleScene");
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