using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personage : MonoBehaviour
{
    public string Name;

    public readonly Dictionary<Characteristics, int> Stats = new ()
    {
        {Characteristics.Strength, 10},
        {Characteristics.Dexterity, 12},
        {Characteristics.Constitution, 10},
        {Characteristics.Intelligence, 10},
        {Characteristics.Wisdom, 10},
        {Characteristics.Charisma, 10},
    };

    public int GetCharacteristicBonus(Characteristics characteristic) => (Stats[characteristic] - 10) / 2;
}
