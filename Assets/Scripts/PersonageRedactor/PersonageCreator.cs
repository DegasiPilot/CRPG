using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

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

    private PersonageInfo _personageInfo;
    private Text _raceBtnText;
    private const int _minPointsForStat = 8;
    private const int _maxPointsForStat = 15;
    private CharacteristicRedactor[] _statRedactors;

    private int _maxStatPointForSpent => _personageInfo.Race == Race.Human ? 27 + _humanStatPointsBonus : 27;
    private const int _humanStatPointsBonus = 5;

    private void Awake()
    {
        Instance = this;
        _personageInfo = new PersonageInfo();
        _personageInfo.UnSpendedStatPoints = _maxStatPointForSpent;
        _raceBtnText = RaceButton.GetComponentInChildren<Text>();
        _statRedactors = StatsParent.GetComponentsInChildren<CharacteristicRedactor>();
        foreach (var redactor in _statRedactors)
        {
            redactor.Setup();
        }
        SetRace(Race.Human);
        RefreshStatPoints();
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
                OnNoMoreStatPoints.Invoke();
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
                OnGetStatPoints.Invoke(); 
            }
            RefreshStatPoints();
        }
    }

    public (int,int) GetStatValue(Characteristics characteristic)
    {
        return (_personageInfo[characteristic],RaceInfos[(int)_personageInfo.Race - 1][characteristic]);
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
            var info = RaceInfos.First(x => x.Race == race);
            string raceName = Translator.Translate(race.ToString());
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
        OnGetStatPoints.Invoke();
        RefreshStatPoints();
    }

    public void TrySavePersonage()
    {
        if(_personageInfo.Race != Race.None && _personageInfo.UnSpendedStatPoints == 0 && !string.IsNullOrEmpty(_personageInfo.Name))
        {
            ApplyBonuses();
            CRUD.CreatePersonageInfo(_personageInfo);
            GameData.PlayerPersonage = _personageInfo;
            GameData.NewGameSave();
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.Log("false");
        }
    }

    private void ApplyBonuses()
    {
        for(int i = 0; i < Enum.GetValues(typeof(Characteristics)).Length; i++)
        {
            _personageInfo[(Characteristics)i] += RaceInfos[(int)_personageInfo.Race - 1][(Characteristics)i];
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
}