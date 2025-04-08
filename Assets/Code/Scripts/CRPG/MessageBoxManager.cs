using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxManager : MonoBehaviour
{
    private static MessageBoxManager _instance;
    private static Coroutine _emergenceMessageRoutine;
    private static Coroutine _hideMessageRoutine;
    private static float _alphaFactor = 0;

    public GameObject MessageBoxCanvas;
    public Image MessageBackground;
    public Text MessageTextblock;
    public float MessageEmergenceTime;
    public float MessageTime;
    public float MessageHideTime;

    private Color _normalTextColor;
    private Color _normalBackgroundColor;

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        _normalTextColor = _instance.MessageTextblock.color;
        _normalBackgroundColor = _instance.MessageBackground.color;
        DontDestroyOnLoad(gameObject);
    }

    public static void ShowMessage(string message)
    {
        _instance.MessageBoxCanvas.SetActive(true);
        _instance.MessageTextblock.text = message;
        _instance.EmergenceMessage();
    }

    private void EmergenceMessage()
    {
        if (_emergenceMessageRoutine != null)
        {
            StopCoroutine(_emergenceMessageRoutine);
        }
        if (_hideMessageRoutine != null)
        {
            _instance.StopCoroutine(_hideMessageRoutine);
        }
        _emergenceMessageRoutine = StartCoroutine(EmergenceMessageRoutine());

        IEnumerator EmergenceMessageRoutine()
        {
            while (_alphaFactor < 1)
            {
                yield return null;
                _alphaFactor += Time.deltaTime / MessageEmergenceTime;
                Color messageBackgroundColor = _normalBackgroundColor;
                messageBackgroundColor.a = _normalBackgroundColor.a * _alphaFactor;
                MessageBackground.color = messageBackgroundColor;
                Color messageTextColor = _normalTextColor;
                messageTextColor.a = _normalTextColor.a * _alphaFactor;
                MessageTextblock.color = _normalTextColor * _alphaFactor;
            }
            HideMessage();
        }
    }

    private void HideMessage()
    {
        if(_hideMessageRoutine != null)
        {
            StopCoroutine(_hideMessageRoutine);
        }
        _hideMessageRoutine = StartCoroutine(HideMessageRoutine());

        IEnumerator HideMessageRoutine()
        {
            float t = 1;
            while (t > 0)
            {
                yield return null;
                t -= Time.deltaTime / MessageTime;
            }

            while(_alphaFactor > 0)
            {
                yield return null;
                _alphaFactor -= Time.deltaTime / MessageHideTime;
                Color messageBackgroundColor = _normalBackgroundColor;
                messageBackgroundColor.a = _normalBackgroundColor.a * _alphaFactor;
                MessageBackground.color = messageBackgroundColor;
                Color messageTextColor = _normalTextColor;
                messageTextColor.a = _normalTextColor.a * _alphaFactor;
                MessageTextblock.color = _normalTextColor * _alphaFactor;
            }
        }
    }
}