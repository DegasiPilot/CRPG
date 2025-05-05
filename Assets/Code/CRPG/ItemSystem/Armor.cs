

namespace CRPG.ItemSystem
{
	class Armor : EquipableItem
	{
		public ArmorInfo ArmorInfo;
		public override ItemInfo ItemInfo => ArmorInfo;
	}
}
