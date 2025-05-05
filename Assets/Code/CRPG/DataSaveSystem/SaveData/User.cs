using MongoDB.Bson;
using System.Collections.Generic;

namespace CRPG.DataSaveSystem.SaveData
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<GameSaveInfo> GameSaves { get; set; }

        public void AddGameSave(GameSaveInfo gameSaveInfo)
        {
            if (GameSaves == null)
            {
                GameSaves = new List<GameSaveInfo>() { gameSaveInfo };
            }
            else
            {
                GameSaves.Add(gameSaveInfo);
            }
        }
    }
}