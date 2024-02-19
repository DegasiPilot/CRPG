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

    public void Setup()
    {
        var buttons = GetComponentsInChildren<Button>();
        _minusBtn = buttons[0];
        _plusBtn = buttons[1];
        _minusBtn.onClick.AddListener(RemovePoint);
        _plusBtn.onClick.AddListener(AddPoint);
        _amontText = GetComponentsInChildren<Text>().First(x => x.name == "Amount");
        _amontText.text = PersonageCreator.Instance.GetCharacteristicValue(Characteristic).ToString();
        PersonageCreator.Instance.OnNoMoreStatPoints.AddListener(() => _plusBtn.interactable = false);
        PersonageCreator.Instance.OnGetStatPoints.AddListener(TryActivatePlusButton);
    }

    private void AddPoint()
    {
        PersonageCreator.Instance.AddStatPoint(Characteristic, out bool canAddMore);
        _minusBtn.interactable = true;
        _plusBtn.interactable = canAddMore;
        _amontText.text = PersonageCreator.Instance.GetCharacteristicValue(Characteristic).ToString();
    }

    private void RemovePoint()
    {
        PersonageCreator.Instance.RemoveStatPoint(Characteristic, out bool canRemoveMore);
        _minusBtn.interactable = canRemoveMore;
        _plusBtn.interactable = true;
        _amontText.text = PersonageCreator.Instance.GetCharacteristicValue(Characteristic).ToString();
    }

    private void TryActivatePlusButton()
    {
        if(PersonageCreator.Instance.CanAddMore(Characteristic))
        {
            _plusBtn.interactable = true;
        }
    }
}
