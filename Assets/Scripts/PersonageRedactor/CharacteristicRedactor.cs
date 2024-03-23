using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacteristicRedactor : MonoBehaviour
{
    public Characteristics Characteristic;

    private Button _minusBtn;
    private Text _amontText;
    private Button _plusBtn;
    private Text _bonusText;

    private PersonageCreator _creator => PersonageCreator.Instance;

    public void Setup()
    {
        var buttons = GetComponentsInChildren<Button>();
        _minusBtn = buttons[0];
        _plusBtn = buttons[1];
        _minusBtn.onClick.AddListener(RemovePoint);
        _plusBtn.onClick.AddListener(AddPoint);
        var texts = GetComponentsInChildren<Text>();
        _amontText = texts.First(x => x.name == "Amount");
        _bonusText = texts.First(x => x.name == "Bonus");
        _creator.OnNoMoreStatPoints.AddListener(() => _plusBtn.interactable = false);
        _creator.OnGetStatPoints.AddListener(TryActivatePlusButton);
    }

    private void AddPoint()
    {
        _creator.AddStatPoint(Characteristic);
        _minusBtn.interactable = true;
        _plusBtn.interactable = _creator.CanAddMore(Characteristic);
        UpdateAmount();
    }

    private void RemovePoint()
    {
        _creator.RemoveStatPoint(Characteristic);
        _minusBtn.interactable = _creator.CanRemoveMore(Characteristic);
        _plusBtn.interactable = true;
        UpdateAmount();
    }

    private void TryActivatePlusButton()
    {
        if(_creator.CanAddMore(Characteristic))
        {
            _plusBtn.interactable = true;
        }
    }

    public void UpdateAmount()
    {
        var statValue = _creator.GetStatValue(Characteristic);
        _amontText.text = $"{statValue.Item1 + statValue.Item2}";
        if (statValue.Item2 > 0)
        {
            _bonusText.enabled = true;
            _bonusText.text = $"+{statValue.Item2}";
        }
        else
        {
            _bonusText.enabled = false;
        }
    }
}
