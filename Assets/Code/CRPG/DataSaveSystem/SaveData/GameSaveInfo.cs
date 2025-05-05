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
		public PlayerSaveInfo[] CompanionsInfo { get; set; }
        private int _activePersonageIndex { get; set; }
        internal PlayerSaveInfo ActivePlayerInfo => _activePersonageIndex == 0 ? MainPlayerInfo : CompanionsInfo[_activePersonageIndex - 1];
		public AppearanceStruct MainPersonageAppearance;

		public string[] InventoryItems { get; set; }
    }
}