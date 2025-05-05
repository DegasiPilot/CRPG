using CRPG.DataSaveSystem.SaveData;
using System.Collections.Generic;

namespace CRPG.DataSaveSystem
{
	internal interface IDataSaveLoader
	{
		public bool TryLogin(string login, string password, out string errors);
		public bool TryRegistrate(string login, string password, out string errors);

		public void CreateGameSaveInfo(GameSaveInfo gameSave);
		public GameSaveInfo GetLastGameSave();
		public List<GameSaveInfo> GetAllGameSaves();
	}
}