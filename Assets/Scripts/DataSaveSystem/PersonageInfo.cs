using MongoDB.Bson;
using System;
using UnityEngine;
using MongoDB.Bson.Serialization.Attributes;

[CreateAssetMenu(fileName = "NewPersonageInfo", menuName = "ScriptableObjects/PersonageInfo")]
public class PersonageInfo : ScriptableObject
{
    [BsonIgnore] public Action OnStatsChanged;

    public ObjectId Id { get; set; }
    public string Name;

    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Intelligence;
    public int Wisdom;
    public int Charisma;

    public Race Race;
    public int UnSpendedStatPoints;

    [NonSerialized, BsonIgnore] public RaceInfo RaceInfo;

    public int MaxHealth => RaceInfo.BaseHealth + GetCharacteristicBonus(Characteristics.Constitution);

    public int Speed => RaceInfo.StandartSpeed;

    public byte[] ImageBytes { get; set; }
    [BsonIgnore] public Texture2D PersonagePortrait;
    public Color PersonagePortraitColor = Color.white;

    public void Setup()
    {
        RaceInfo = GameData.RaceInfos[(int)Race - 1];
    }

    public int this [Characteristics index]
    {
        get
        {
            switch (index)
            {
                case Characteristics.Strength: return Strength;
                case Characteristics.Dexterity: return Dexterity;
                case Characteristics.Constitution: return Constitution;
                case Characteristics.Intelligence: return Intelligence;
                case Characteristics.Wisdom: return Wisdom;
                case Characteristics.Charisma: return Charisma;
                default: throw new Exception($"Personage don't have {index} property");
            }
        }
        set
        {
            switch (index)
            {
                case Characteristics.Strength: Strength = value; break;
                case Characteristics.Dexterity: Dexterity = value; break;
                case Characteristics.Constitution: Constitution = value; break;
                case Characteristics.Intelligence: Intelligence = value; break;
                case Characteristics.Wisdom: Wisdom = value; break;
                case Characteristics.Charisma: Charisma = value; break;
                default: throw new Exception($"Personage don't have {index} property");
            }
            OnStatsChanged?.Invoke();
        }
    }

    public int GetCharacteristicBonus(Characteristics characteristic) => (this[characteristic] - 10) / 2;

    public void ResetStats()
    {
        Strength = 8;
        Dexterity = 8;
        Constitution = 8;
        Intelligence = 8;
        Wisdom = 8;
        Charisma = 8;
    }
}