using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRaceInfo", menuName = "ScriptableObjects/RaceInfo")]
public class RaceInfo : ScriptableObject
{
    public Race Race;
    public string Description;
    public int StandartSpeed;

    public int StrengthBonus { get; set; }
    public int DexterityBonus { get; set; }
    public int ConstitutionBonus { get; set; }
    public int IntelligenceBonus { get; set; }
    public int WisdomBonus { get; set; }
    public int CharismaBonus { get; set; }

    public int this[Characteristics index]
    {
        get
        {
            switch (index)
            {
                case Characteristics.Strength: return StrengthBonus;
                case Characteristics.Dexterity: return DexterityBonus;
                case Characteristics.Constitution: return ConstitutionBonus;
                case Characteristics.Intelligence: return IntelligenceBonus;
                case Characteristics.Wisdom: return WisdomBonus;
                case Characteristics.Charisma: return CharismaBonus;
                default: throw new System.Exception($"Personage don't have {index} property");
            }
        }
        set
        {
            switch (index)
            {
                case Characteristics.Strength: StrengthBonus = value; break;
                case Characteristics.Dexterity: DexterityBonus = value; break;
                case Characteristics.Constitution: ConstitutionBonus = value; break;
                case Characteristics.Intelligence: IntelligenceBonus = value; break;
                case Characteristics.Wisdom: WisdomBonus = value; break;
                case Characteristics.Charisma: CharismaBonus = value; break;
                default: throw new System.Exception($"Personage don't have {index} property");
            }
        }
    }

    public static RaceInfo[] GetAllRaceInfos()
    {
        return Resources.LoadAll<RaceInfo>("RacesInfo");
    }
}