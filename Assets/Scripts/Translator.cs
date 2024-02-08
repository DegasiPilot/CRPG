using System.Collections.Generic;
using System.Linq;

public static class Translator
{
    private static Dictionary<string, string> _russianDictionary = new()
    {
        { "Strength", "����"},
        { "Dexterity", "��������"},
        { "Constitution", "������������"},
        { "Intelligence", "��������"},
        { "Wisdom", "��������"},
        { "Charisma", "�������"},
        { "Human", "�������"},
        { "Elf", "����"},
        { "Dwarf", "�����"},
        { "Orc", "���"},
    };

    public static string Translate(string englishString)
    {
        if (_russianDictionary.Keys.Contains(englishString))
        {
            return _russianDictionary[englishString];
        }
        else
        {
            return englishString;
        }
    }
}
