using System.Collections.Generic;

namespace CRPG.DataSaveSystem
{
	internal interface IDataSaveLoader
	{
		public void CreateGameSaveInfo(GameSaveInfo gameSave);

		public GameSaveInfo GetLastGameSave();

		public List<GameSaveInfo> GetAllGameSaves();

		public bool HasAnySaves();
	}
}
