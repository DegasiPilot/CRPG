using System.Collections.Generic;

public static class TextHelper
{
	private static readonly Dictionary<Characteristics, string> _characteristicsDictionary = new()
	{
		{Characteristics.Strength, "Сила"},
		{Characteristics.Dexterity, "Ловкость"},
		{Characteristics.Constitution, "Выносливость"},
		{Characteristics.Charisma, "Харизма"},
	};

	private static readonly Dictionary<Characteristics, string> _characteristicsInfo = new()
	{
		{Characteristics.Strength, "За каждую 1 силы:\n+1 к макс. энергии на удар"},
		{Characteristics.Dexterity, "За каждые 2 ловкости:\n+ 3% уклонения за энергию\nЗа каждые 3 ловкости:\n+1 действие в ход"},
		{Characteristics.Constitution, "За каждую 1 выносливости:\n+2 к максимуму энергии"},
		{Characteristics.Charisma, "Влияет на социальные взаимодействия\n(например диалоги)"},
	};

	private static readonly Dictionary<Race, string> _racesDictionary = new()
	{
		{Race.Human, "Человек"},
		{Race.Elf, "Эльф"},
		{Race.Dwarf, "Дварф"},
		{Race.Orc, "Орк"},
	};

	private static readonly Dictionary<CheckResult, string> _resultsDictionary = new()
	{
		{CheckResult.CriticalFail, "Критический провал"},
		{CheckResult.Fail, "Провал"},
		{CheckResult.Succes, "Успех"},
		{CheckResult.CriticalSucces, "Критический успех"}
	};

	public static string Translate(Characteristics characteristic)
	{
		return _characteristicsDictionary[characteristic];
	}

	public static string InfoOf(Characteristics characteristic)
	{
		return _characteristicsInfo[characteristic];
	}

	public static string Translate(Race race)
	{
		return _racesDictionary[race];
	}

	public static string Translate(CheckResult checkResult)
	{
		return _resultsDictionary[checkResult];
	}
}