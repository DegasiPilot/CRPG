using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRPG.ItemSystem
{
	class Weapon : EquipableItem
	{
		public WeaponInfo WeaponInfo;
		public override ItemInfo ItemInfo => WeaponInfo;
	}
}
