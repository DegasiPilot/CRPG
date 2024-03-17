using MongoDB.Bson;
using System;

public class PersonageInfo
{
    public Action OnStatsChanged;

    public ObjectId Id { get; set; }
    public string Name;

    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }

    public Race Race;

    [NonSerialized] public RaceInfo RaceInfo;

    public int MaxHealth => RaceInfo.BaseHealth + GetCharacteristicBonus(Characteristics.Constitution);

    public PersonageInfo(string name = "Unnammed")
    {
        Name = name;
        Strength = 10;
        Dexterity = 10;
        Constitution = 10;
        Intelligence = 10;
        Wisdom = 10;
        Charisma = 10;
    }

    public void Setup()
    {
        RaceInfo = GameData.RaceInfos[(int)Race];
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
                default: throw new System.Exception($"Personage don't have {index} property");
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
                default: throw new System.Exception($"Personage don't have {index} property");
            }
            OnStatsChanged?.Invoke();
        }
    }

    public int UnSpendedStatPoints = 12;

    public int GetCharacteristicBonus(Characteristics characteristic) => (this[characteristic] - 10) / 2;
}