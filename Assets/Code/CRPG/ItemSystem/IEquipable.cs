using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRPG.ItemSystem
{
    interface IEquipable
    {
        public bool IsEquiped { get; }

        public void Equip();
    }
}
