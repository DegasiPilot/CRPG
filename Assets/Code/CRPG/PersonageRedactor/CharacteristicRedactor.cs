using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacteristicRedactor : MonoBehaviour
{
	public Characteristics Characteristic;

	[SerializeField] private ControlGroup _statControl;
	[SerializeField] private Text _bonusText;

	private Action<Characteristics> _onAddPoint;
	private Action<Characteristics> _onRemovePoint;
	private Func<Characteristics, bool> _canAddMore;
	private Func<Characteristics, bool> _canRemoveMore;
	private Func<Characteristics, int> _getAmount;

	private void OnValidate()
	{
		if (_bonusText == null) _bonusText = GetComponentsInChildren<Text>().First(x => x.name == "Bonus");
		if (_statControl == null) _statControl = GetComponentInChildren<ControlGroup>();
	}

	public void Setup(Action<Characteristics> onAddPoint, Action<Characteristics> onRemovePoint,
		Func<Characteristics, bool> canAddMore, Func<Characteristics, bool> canRemoveMore,
		Func<Characteristics, int> getAmount)
	{
		_onAddPoint = onAddPoint;
		_onRemovePoint = onRemovePoint;
		_canAddMore = canAddMore;
		_canRemoveMore = canRemoveMore;
		_getAmount = getAmount;
		_statControl.Setup(AddPoint, RemovePoint, CanAddMore, CanRemoveMore, GetAmount);
	}

	private void AddPoint() => _onAddPoint.Invoke(Characteristic);
	private void RemovePoint() => _onRemovePoint.Invoke(Characteristic);
	private bool CanAddMore() => _canAddMore.Invoke(Characteristic);
	private bool CanRemoveMore() => _canRemoveMore.Invoke(Characteristic);
	private int GetAmount() => _getAmount.Invoke(Characteristic);

	public void UpdateAmount((int, int) statValue)
	{
		_statControl.UpdateAmount();
		UpdateBonus(statValue);
	}

	public void UpdateBonus((int, int) statValue)
	{
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

	public void DeactivatePlusButton() => _statControl.DeactivatePlusButton();
	public void TryActivatePlusButton() => _statControl.TryActivatePlusButton();
}