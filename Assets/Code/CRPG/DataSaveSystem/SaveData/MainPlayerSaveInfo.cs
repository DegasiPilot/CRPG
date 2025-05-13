
namespace CRPG.DataSaveSystem.SaveData
{
	public class MainPlayerSaveInfo
	{
		public string Name;

		public int Strength;
		public int Dexterity;
		public int Constitution;
		public int Charisma;

		public int UnSpendedStatPoints;

		public Race Race { get; set; }

		public byte[] ImageBytes { get; set; }

		public Gender Gender;
	}
}
