using CRPG.Customization;
using System;

namespace CRPG.DataSaveSystem.SaveData
{
    public class GameSaveInfo
    {
        public DateTime DateTime;

        public SceneSaveInfo SceneSaveInfo { get; set; }
        internal PlayerSaveInfo MainPlayerInfo { get; set; }
        internal PlayerSaveInfo[] CompanionsInfo { get; set; }
        private int _activePersonageIndex { get; set; }
        internal PlayerSaveInfo ActivePlayerInfo => _activePersonageIndex == 0 ? MainPlayerInfo : CompanionsInfo[_activePersonageIndex - 1];
        internal AppearanceStruct MainPersonageAppearance;

        internal string[] InventoryItems { get; set; }
    }
}