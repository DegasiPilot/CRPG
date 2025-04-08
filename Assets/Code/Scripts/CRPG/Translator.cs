using System.Collections.Generic;
using System.Linq;

public static class Translator
{
    private static Dictionary<Characteristics, string> _characteristicsDictionary = new()
    {
        {Characteristics.Strength, "Сила"},
        {Characteristics.Dexterity, "Ловкость"},
        {Characteristics.Constitution, "Выносливость"},
        {Characteristics.Intelligence, "Интелект"},
        {Characteristics.Wisdom, "Мудрость"},
        {Characteristics.Charisma, "Харизма"},
    };

    private static Dictionary<Race, string> _racesDictionary = new()
    {
        {Race.Human, "Человек"},
        {Race.Elf, "Эльф"},
        {Race.Dwarf, "Дварф"},
        {Race.Orc, "Орк"},
    };

    private static Dictionary<CheckResult, string> _resultsDictionary = new()
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

    public static string Translate(Race race)
    {
        return _racesDictionary[race];
    }

    public static string Translate(CheckResult checkResult)
    {
        return _resultsDictionary[checkResult];
    }
}
