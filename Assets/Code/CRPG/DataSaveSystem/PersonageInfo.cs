using CRPG.DataSaveSystem.SaveData;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPersonageInfo", menuName = "ScriptableObjects/PersonageInfo")]
public class PersonageInfo : ScriptableObject
{
	public Action OnStatsChanged;

	public string Name;

	public int Strength;
	public int Dexterity;
	public int Constitution;
	public int Charisma;

	public int UnSpendedStatPoints;

	[SerializeField] private RaceInfo _raceIfo;
	public RaceInfo RaceInfo
	{
		get => _raceIfo;
		set
		{
			_raceIfo = value;
		}
	}

	public int MaxHealth => RaceInfo.BaseHealth;
	public int MaxStamina => RaceInfo.BaseStamina + Constitution * 2;
	public int ActionsPerTurn => 1 + Dexterity / 3;

	public int Speed => RaceInfo.StandartSpeed;

	[SerializeField] private Texture2D _personagePortrait;
	public Texture2D PersonagePortrait
	{
		get => _personagePortrait;
		set => _personagePortrait = value;
	}

	public Color PersonagePortraitColor = Color.white;
	public Gender Gender;

	internal void Setup(MainPlayerSaveInfo personageSaveInfo, Func<Race, RaceInfo> getRaceInfo)
	{
		Name = personageSaveInfo.Name;

		Strength = personageSaveInfo.Strength;
		Dexterity = personageSaveInfo.Dexterity;
		Constitution = personageSaveInfo.Constitution;
		Charisma = personageSaveInfo.Charisma;

		RaceInfo = getRaceInfo.Invoke(personageSaveInfo.Race);
		UnSpendedStatPoints = 0;

		_personagePortrait = new Texture2D(1, 1);
		_personagePortrait.LoadImage(personageSaveInfo.ImageBytes);

		PersonagePortraitColor = Color.white;
		Gender = personageSaveInfo.Gender;
	}

	internal MainPlayerSaveInfo Save()
	{
		MainPlayerSaveInfo personageSaveInfo = new();

		personageSaveInfo.Name = Name;

		personageSaveInfo.Strength = Strength;
		personageSaveInfo.Dexterity = Dexterity;
		personageSaveInfo.Constitution = Constitution;
		personageSaveInfo.Charisma = Charisma;

		personageSaveInfo.Race = RaceInfo.Race;

		personageSaveInfo.ImageBytes = _personagePortrait.EncodeToPNG();

		personageSaveInfo.Gender = Gender;

		return personageSaveInfo;
	}

	public int this[Characteristics index]
	{
		get
		{
			switch (index)
			{
				case Characteristics.Strength: return Strength;
				case Characteristics.Dexterity: return Dexterity;
				case Characteristics.Constitution: return Constitution;
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
				case Characteristics.Charisma: Charisma = value; break;
				default: throw new Exception($"Personage don't have {index} property");
			}
			OnStatsChanged?.Invoke();
		}
	}

	public int GetCharacteristicBonus(Characteristics characteristic) => this[characteristic] / 2;

	public void ResetPersonageInfo()
	{
		ResetStats();
		PersonagePortrait = null;
		Gender = Gender.None;
		RaceInfo = null;
	}

	public void ResetStats()
	{
		Strength = 0;
		Dexterity = 0;
		Constitution = 0;
		Charisma = 0;
	}
}