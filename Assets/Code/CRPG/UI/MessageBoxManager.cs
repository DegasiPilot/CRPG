using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxManager : MonoBehaviour
{
	private Coroutine _emergenceMessageRoutine;
	private Coroutine _hideMessageRoutine;
	private float _alphaFactor = 0;

	public GameObject MessageBoxCanvas;
	public Image MessageBackground;
	public Text MessageTextblock;
	public float MessageEmergenceTime;
	public float MessageTimePerCharacter;
	public float MessageHideTime;

	private Color _normalTextColor;
	private Color _normalBackgroundColor;

	private string _message;
	private bool _isInitialized;

	private void Awake()
	{
		if (!_isInitialized)
		{
			_normalTextColor = MessageTextblock.color;
			_normalBackgroundColor = MessageBackground.color;
			DontDestroyOnLoad(gameObject);
			_isInitialized = true;
		}
	}

	public void ShowMessage(string message)
	{
		_message = message;
		MessageBoxCanvas.SetActive(true);
		MessageTextblock.text = _message;
		EmergenceMessage();
	}

	private void EmergenceMessage()
	{
		if (_emergenceMessageRoutine != null)
		{
			StopCoroutine(_emergenceMessageRoutine);
		}
		if (_hideMessageRoutine != null)
		{
			StopCoroutine(_hideMessageRoutine);
		}
		_emergenceMessageRoutine = StartCoroutine(EmergenceMessageRoutine());

		IEnumerator EmergenceMessageRoutine()
		{
			while (_alphaFactor < 1)
			{
				_alphaFactor = Mathf.MoveTowards(_alphaFactor, 1f, Time.unscaledDeltaTime / MessageEmergenceTime);
				Color messageBackgroundColor = _normalBackgroundColor;
				messageBackgroundColor.a = _normalBackgroundColor.a * _alphaFactor;
				MessageBackground.color = messageBackgroundColor;
				Color messageTextColor = _normalTextColor;
				messageTextColor.a = _normalTextColor.a * _alphaFactor;
				MessageTextblock.color = _normalTextColor * _alphaFactor;
				yield return null;
			}
			HideMessage();
		}
	}

	private void HideMessage()
	{
		if (_hideMessageRoutine != null)
		{
			StopCoroutine(_hideMessageRoutine);
		}
		_hideMessageRoutine = StartCoroutine(HideMessageRoutine());

		IEnumerator HideMessageRoutine()
		{
			yield return new WaitForSeconds(_message.Length * MessageTimePerCharacter);

			while (_alphaFactor > 0)
			{
				_alphaFactor = Mathf.MoveTowards(_alphaFactor, 0, Time.unscaledDeltaTime / MessageHideTime);
				Color messageBackgroundColor = _normalBackgroundColor;
				messageBackgroundColor.a = _normalBackgroundColor.a * _alphaFactor;
				MessageBackground.color = messageBackgroundColor;
				Color messageTextColor = _normalTextColor;
				messageTextColor.a = _normalTextColor.a * _alphaFactor;
				MessageTextblock.color = messageTextColor;
				yield return null;
			}

			_message = null;
		}
	}
}