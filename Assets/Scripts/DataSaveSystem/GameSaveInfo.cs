using MongoDB.Bson;

public class GameSaveInfo
{
    public ObjectId Id { get; set; }

    public BsonDateTime DateTime;
    public ObjectId MainPersonageId { get; set; }
}
