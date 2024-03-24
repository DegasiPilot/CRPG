using System.Collections.Generic;
using System.Linq;

public static class Translator
{
    private static Dictionary<Characteristics, string> _characteristicsDictionary = new()
    {
        {Characteristics.Strength, "����"},
        {Characteristics.Dexterity, "��������"},
        {Characteristics.Constitution, "������������"},
        {Characteristics.Intelligence, "��������"},
        {Characteristics.Wisdom, "��������"},
        {Characteristics.Charisma, "�������"},
    };

    private static Dictionary<Race, string> _racesDictionary = new()
    {
        {Race.Human, "�������"},
        {Race.Elf, "����"},
        {Race.Dwarf, "�����"},
        {Race.Orc, "���"},
    };

    private static Dictionary<CheckResult, string> _resultsDictionary = new()
    {
        {CheckResult.CriticalFail, "����������� ������"},
        {CheckResult.Fail, "������"},
        {CheckResult.Succes, "�����"},
        {CheckResult.CriticalSucces, "����������� �����"}
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
