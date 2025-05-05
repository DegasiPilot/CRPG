using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RaceButtonScript : MonoBehaviour
{
	[SerializeField] private RaceInfo _raceInfo;
	[SerializeField] private Image _iconImage;
    [SerializeField] private Button _button;

	private void OnValidate()
	{
		if (_button == null)
		{
			_button = GetComponent<Button>();
		}
		if(_iconImage != null && _raceInfo != null)
		{
			_iconImage.sprite = _raceInfo.Sprite;
		}
	}

	public void AddListener(Action<RaceInfo> call)
	{
		_button.onClick.AddListener(() => call?.Invoke(_raceInfo));
	}
}