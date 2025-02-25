using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ControlGroup : MonoBehaviour
{
    private Button _minusBtn;
    private Text _amontText;
    private Button _plusBtn;

    private Action _onAddPoint;
    private Action _onRemovePoint;
    private Func<bool> _canAddMore;
    private Func<bool> _canRemoveMore;
    private Func<int> _getAmount;

    public void Setup(Action OnAddPoint, Action OnRemovePoint, Func<bool> CanAddMore, Func<bool> CanRemoveMore, Func<int> GetAmount)
    {
        var buttons = GetComponentsInChildren<Button>();
        _minusBtn = buttons[0];
        _plusBtn = buttons[1];
        _minusBtn.onClick.AddListener(RemovePoint);
        _plusBtn.onClick.AddListener(AddPoint);
        var texts = GetComponentsInChildren<Text>();
        _amontText = texts.First(x => x.name == "Amount");
        _onAddPoint = OnAddPoint;
        _onRemovePoint = OnRemovePoint;
        _canAddMore = CanAddMore;
        _canRemoveMore = CanRemoveMore;
        _getAmount = GetAmount;
    }

    private void AddPoint()
    {
        _onAddPoint.Invoke();
        _minusBtn.interactable = true;
        _plusBtn.interactable = _canAddMore();
        UpdateAmount();
    }

    private void RemovePoint()
    {
        _onRemovePoint.Invoke();
        _minusBtn.interactable = _canRemoveMore();
        _plusBtn.interactable = true;
        UpdateAmount();
    }

    public void TryActivatePlusButton()
    {
        if (_canAddMore.Invoke())
        {
            _plusBtn.interactable = true;
        }
    }

    public void ActivatePlusButton() => _plusBtn.interactable = true;
    public void DeactivatePlusButton() => _plusBtn.interactable = false;

    public void UpdateAmount()
    {
        _amontText.text = _getAmount.Invoke().ToString();
    }
}