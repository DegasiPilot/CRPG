using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ControlGroup : MonoBehaviour
{
	[SerializeField] private Button _minusBtn;
	[SerializeField] private Text _amontText;
	[SerializeField] private Button _plusBtn;

	private void OnValidate()
	{
		if (_minusBtn == null || _plusBtn == null)
		{
			var buttons = GetComponentsInChildren<Button>();
			if (_minusBtn == null) _minusBtn = buttons[0];
			if (_plusBtn == null) _plusBtn = buttons[1];
		}
		if (_amontText == null) _amontText = GetComponentsInChildren<Text>().First(x => x.name == "Amount");
	}

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
        UpdateAmount();
    }

    private void RemovePoint()
    {
        _onRemovePoint.Invoke();
        UpdateAmount();
    }

    public void TryActivatePlusButton()
    {
        if (_canAddMore.Invoke())
        {
            _plusBtn.interactable = true;
        }
    }

    public void DeactivatePlusButton() => _plusBtn.interactable = false;

    public void UpdateAmount()
    {
        _amontText.text = _getAmount.Invoke().ToString();
        _plusBtn.interactable = _canAddMore.Invoke();
        _minusBtn.interactable = _canRemoveMore.Invoke();
    }
}