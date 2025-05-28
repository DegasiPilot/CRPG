using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CRPG.PersonageRedactor
{
    class CharacteristicTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
		private static readonly Color _darkenedColor = new Color(0.8f, 0.8f, 0.8f);

        private Characteristics _characteristic;
		[SerializeField] private Image _image;

        [SerializeField] private TextMeshProUGUI _text;

		private void OnValidate()
		{
			TryGetComponent(out _image);
		}

		public void Setup(Characteristics characteristic)
        {
            _characteristic = characteristic;
        }

		public void OnPointerEnter(PointerEventData eventData)
		{
			_image.color = _darkenedColor;
			_text.text = TextHelper.InfoOf(_characteristic);
			_text.transform.parent.gameObject.SetActive(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_image.color = Color.white;
			_text.transform.parent.gameObject.SetActive(false);
			_text.text = string.Empty;
		}
	}
}