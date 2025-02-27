using MongoDB.Bson;

public class GameSaveInfo
{
    public ObjectId Id { get; set; }

    public BsonDateTime DateTime;
    public ObjectId MainPersonageId { get; set; }
    public string UserLogin { get; set; }

    public SceneSaveInfo SceneSaveInfo { get; set; }
    public PersonageInfo MainPersonageInfo { get; set; }

    public InventoryItem[] InventoryItems { get; set; }

    public struct InventoryItem
    {
        public int ItemId;
        public bool IsEquiped;
    }
}
