using System;

public class GameSaveInfo
{
    public DateTime DateTime;

    public SceneSaveInfo SceneSaveInfo { get; set; }
    public PersonageInfo MainPersonageInfo { get; set; }

    public InventoryItem[] InventoryItems { get; set; }

    public struct InventoryItem
    {
        public string ItemName;
        public bool IsEquiped;
    }
}
