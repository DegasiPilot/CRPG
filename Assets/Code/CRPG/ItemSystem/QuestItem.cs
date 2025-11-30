using UnityEngine;

namespace CRPG.ItemSystem
{
	class QuestItem : Item
	{
		public override ItemInfo ItemInfo => QuestItemInfo;
		[SerializeField] private ItemInfo QuestItemInfo;
	}
}
