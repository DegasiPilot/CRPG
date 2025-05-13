namespace CRPG.ItemSystem
{
	interface IEquipable
	{
		public bool IsEquiped { get; }

		public void Equip();
	}
}
