using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;

public class Personage : MonoBehaviour
{
    public string DefaultName;

    public PersonageInfo PersonageInfo;

    public void Setup()
    {
        PersonageInfo ??= CRUD.GetPersonageInfo(DefaultName);
    }
}
