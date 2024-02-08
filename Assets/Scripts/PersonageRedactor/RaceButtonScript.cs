using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RaceButtonScript : MonoBehaviour
{
    public Race Race;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(SetRace);
    }

    private void SetRace()
    {
        PersonageCreator.Instance.SetRace(Race);
    }
}
