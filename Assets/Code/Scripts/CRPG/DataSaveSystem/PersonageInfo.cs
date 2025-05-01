//using MongoDB.Bson;
using System;
using UnityEditor;
using UnityEngine;
//using MongoDB.Bson.Serialization.Attributes;

[CreateAssetMenu(fileName = "NewPersonageInfo", menuName = "ScriptableObjects/PersonageInfo")]
public class PersonageInfo : ScriptableObject
{
    /*[BsonIgnore]*/ public Action OnStatsChanged;

    public string Name;

    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Intelligence;
    public int Wisdom;
    public int Charisma;

    //Only for load, use RaceInfo if can
    public Race Race;
    public int UnSpendedStatPoints;

	[NonSerialized, /*BsonIgnore*/] private RaceInfo _raceIfo;
    public RaceInfo RaceInfo
    {
        get => _raceIfo;
        set
        {
            _raceIfo = value;
            if(RaceInfo == null)
            {
                Race = Race.None;
            }
            else
            {
				Race = value.Race;
			}
        }
    }

    public int MaxHealth => RaceInfo.BaseHealth;
    public int MaxStamina => RaceInfo.BaseStamina + Constitution*2;
    public int ActionsPerTurn => 1 + Dexterity / 3;

    public int Speed => RaceInfo.StandartSpeed;

    public byte[] ImageBytes { get; set; }
    /*[BsonIgnore]*/ public Texture2D PersonagePortrait;
    public Color PersonagePortraitColor = Color.white;
    public Gender Gender;

    public void Setup(Func<Race, RaceInfo> getRaceInfo)
    {
        RaceInfo = getRaceInfo.Invoke(Race);
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

	public void ResetPersonageInfo()
	{
        ResetStats();
        ImageBytes = null;
        Gender = Gender.None;
        RaceInfo = null;
	}

	public void ResetStats()
    {
        Strength = 0;
        Dexterity = 0;
        Constitution = 0;
        Intelligence = 0;
        Wisdom = 0;
        Charisma = 0;
    }
}

[CustomEditor(typeof(PersonageInfo))]
public class PersonageInfoEditor : Editor
{
	public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
	{
		PersonageInfo info = target as PersonageInfo;
        if(info != null && info.PersonagePortrait != null)
        {
            return Instantiate(info.PersonagePortrait);
		}
        else
        {
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
		}
	}
}