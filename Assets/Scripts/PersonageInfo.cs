using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPersonageInfo", menuName = "ScriptableObjects/PersonageInfo")]
public class PersonageInfo : ScriptableObject, ISerializationCallbackReceiver
{
    public string Name;

    public readonly Dictionary<Characteristics, int> Stats = new();

    public RaceInfo RaceInfo;

    private List<int> StatsValues = new();

    public PersonageInfo()
    {
        Stats = new()
        {
            {Characteristics.Strength, 10},
            {Characteristics.Dexterity, 10},
            {Characteristics.Constitution, 10},
            {Characteristics.Intelligence, 10},
            {Characteristics.Wisdom, 10},
            {Characteristics.Charisma, 10},
        };
    }

    public int UnSpendedStatPoints = 12;

    public int GetCharacteristicBonus(Characteristics characteristic) => (Stats[characteristic] - 10) / 2;

    public void OnBeforeSerialize()
    {
        if (Stats.Count > 0)
        {
            StatsValues.Clear();
            for (int i = 0; i < 6; i++)
            {
                StatsValues.Add(Stats[(Characteristics)i]);
            }
        }
    }

    public void OnAfterDeserialize()
    {
        if (StatsValues.Count > 0)
        {
            Stats.Clear();
            for (int i = 0; i < 6; i++)
            {
                Stats.Add((Characteristics)i, StatsValues[i]);
            }
        }
    }
}