using CRPG.DataSaveSystem.SaveData;
using System.Collections.Generic;

namespace CRPG.DataSaveSystem
{
	internal interface IDataSaveLoader
	{
		public bool IsUserLogined { get; }
		public string UserLogin { get; }
		public bool CanExit { get; }

		public bool TryLogin(string login, string password, out string errors);
		public bool TryRegistrate(string login, string password, out string errors);

		public void CreateGameSaveInfo(GameSaveInfo gameSave);
		public GameSaveInfo GetLastGameSave();
		public List<GameSaveInfo> GetAllGameSaves();
		public bool HasSaves { get; }
	}
}