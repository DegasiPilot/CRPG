using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;

public class Personage : MonoBehaviour
{
    public string DefaultName;

    public int MaxHealth { get; private set; }

    private PersonageInfo _personageInfo;
    public PersonageInfo PersonageInfo => _personageInfo;

    public int CurrentHealth;

    public void Setup()
    {
        _personageInfo ??= CRUD.GetPersonageInfo(DefaultName);
        _personageInfo.Setup();
        CurrentHealth = _personageInfo.MaxHealth;
    }

    public void Setup(PersonageInfo personageInfo)
    {
        _personageInfo = personageInfo;
        _personageInfo.Setup();
        CurrentHealth = _personageInfo.MaxHealth;
    }
}
