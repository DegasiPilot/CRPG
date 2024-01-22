using System.Collections.Generic;
using System.Linq;

public static class Translator
{
    private static Dictionary<string, string> _russianDictionary = new()
    {
        { "Strength", "Сила"},
        { "Dexterity", "Ловкость"},
        { "Constitution", "Телосложение"},
        { "Intelligence", "Интелект"},
        { "Wisdom", "Мудрость"},
        { "Charisma", "Харизма"}
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
