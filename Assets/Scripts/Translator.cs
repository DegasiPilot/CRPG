using System.Collections.Generic;
using System.Linq;

public static class Translator
{
    private static Dictionary<string, string> _russianDictionary = new()
    {
        { "Strength", "Сила"},
        { "Dexterity", "Ловкость"},
        { "Constitution", "Выносливость"},
        { "Intelligence", "Интелект"},
        { "Wisdom", "Мудрость"},
        { "Charisma", "Харизма"},
        { "Human", "Человек"},
        { "Elf", "Эльф"},
        { "Dwarf", "Дварф"},
        { "Orc", "Орк"},
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
