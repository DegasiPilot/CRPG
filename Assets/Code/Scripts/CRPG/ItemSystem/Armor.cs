using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRPG.ItemSystem
{
	class Armor : EquipableItem
	{
		public ArmorInfo ArmorInfo;
		public override ItemInfo ItemInfo => ArmorInfo;
	}
}
