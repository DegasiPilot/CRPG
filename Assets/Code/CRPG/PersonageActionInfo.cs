using UnityEngine;

namespace CRPG
{
	[CreateAssetMenu(fileName = "NewPersonageActionInfo", menuName = "ScriptableObjects/PersonageActionInfo")]
	class PersonageActionInfo : ScriptableObject
	{
		[SerializeField] private ActionType _actionType;
		public ActionType ActionType => _actionType;

		[SerializeField] private Sprite _icon;
		public Sprite Icon => _icon;
	}
}