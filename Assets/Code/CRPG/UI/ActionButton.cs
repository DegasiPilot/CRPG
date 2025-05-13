using CRPG;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
internal class ActionButton : MonoBehaviour, IDisposable
{
	public UnityEvent<ActionType, bool> OnToggle;
	public ToggleGroup ToggleGroup
	{
		get => _toggle.group;
		set => _toggle.group = value;
	}

	[SerializeField] private Toggle _toggle;
	[SerializeField] private Image _actionImage;

	private ActionType _myAction;
	public ActionType MyAction => _myAction;

	private void OnValidate()
	{
		if (_toggle == null) _toggle = GetComponent<Toggle>();
	}

	private void Awake()
	{
		_toggle.onValueChanged.AddListener(OnToggleChanged);
	}

	private void OnToggleChanged(bool activate)
	{
		OnToggle.Invoke(_myAction, activate);
	}

	public void SetWithoutNotify(bool activate)
	{
		_toggle.SetIsOnWithoutNotify(activate);
	}

	public void Setup(PersonageActionInfo actionInfo)
	{
		_actionImage.sprite = actionInfo.Icon;
		_myAction = actionInfo.ActionType;
	}

	private void OnDestroy()
	{
		_toggle.onValueChanged.RemoveListener(OnToggleChanged);
	}

	public void Dispose()
	{
		Destroy(gameObject);
	}
}