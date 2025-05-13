using UnityEngine;

[CreateAssetMenu(fileName = "NewRaceInfo", menuName = "ScriptableObjects/RaceInfo")]
public class RaceInfo : ScriptableObject
{
	public Race Race;
	[TextArea] public string Description;
	public int StandartSpeed;
	public int BaseHealth;
	public int BaseStamina;
	public Sprite Sprite;

	[Header("Бонусы")]
	public int StrengthBonus;
	public int DexterityBonus;
	public int ConstitutionBonus;
	public int IntelligenceBonus;
	public int WisdomBonus;
	public int CharismaBonus;

	public int this[Characteristics index]
	{
		get
		{
			switch (index)
			{
				case Characteristics.Strength: return StrengthBonus;
				case Characteristics.Dexterity: return DexterityBonus;
				case Characteristics.Constitution: return ConstitutionBonus;
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
				case Characteristics.Charisma: CharismaBonus = value; break;
				default: throw new System.Exception($"Personage don't have {index} property");
			}
		}
	}
}