using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRaceInfo", menuName = "ScriptableObjects/RaceInfo")]
public class RaceInfo : ScriptableObject
{
    public Race Race;
    public string Description;
    public Dictionary<Characteristics, int> StatBonuses = new();
    public int StandartSpeed;
}
