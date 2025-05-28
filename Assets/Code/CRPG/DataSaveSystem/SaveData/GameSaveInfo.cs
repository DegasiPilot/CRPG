using CRPG.Customization;
using System;

namespace CRPG.DataSaveSystem.SaveData
{
	public class GameSaveInfo
	{
		public DateTime DateTime;

		public SceneSaveInfo SceneSaveInfo { get; set; }
		public PlayerSaveInfo MainPlayerInfo { get; set; }
		public MainPlayerSaveInfo MainPersonageInfo { get; set; }
		public AppearanceStruct MainPersonageAppearance;

		public string[] InventoryItems { get; set; }
	}
}