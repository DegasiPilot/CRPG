using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
	class PersonageActionsView : MonoBehaviour
	{
		[SerializeField] private ActionButton _prefab;
		[SerializeField] private ToggleGroup _toggleGroup;

		private void OnValidate()
		{
			if (_toggleGroup == null)
			{
				_toggleGroup = GetComponent<ToggleGroup>();
			}
		}

		public ActionButton InstantiateActionButton()
		{
			ActionButton actionButton = Instantiate(_prefab, transform);
			actionButton.ToggleGroup = _toggleGroup;
			return actionButton;
		}

		public void DeactivateAllActions()
		{
			_toggleGroup.SetAllTogglesOff(false);
		}
	}
}