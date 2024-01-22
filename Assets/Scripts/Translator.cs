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
        { "Charisma", "�������"}
    };

    public static string TranslateToRussian(string englishString)
    {
        if (_russianDictionary.Keys.Contains(englishString))
        {
            return _russianDictionary[englishString];
        }
        else
        {
            return "";
        }
    }
}
