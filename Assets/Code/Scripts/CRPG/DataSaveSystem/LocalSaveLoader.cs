using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CRPG.DataSaveSystem
{
	internal class LocalSaveLoader : IDataSaveLoader
	{
		private List<GameSaveInfo> _gameSaves = new();

		public void CreateGameSaveInfo(GameSaveInfo gameSave)
		{
			if (gameSave.MainPersonageInfo.PersonagePortrait != null)
			{
				gameSave.MainPersonageInfo.ImageBytes = gameSave.MainPersonageInfo.PersonagePortrait.EncodeToPNG();
			}
			_gameSaves.Add(gameSave);
		}

		public List<GameSaveInfo> GetAllGameSaves()
		{
			return _gameSaves;
		}

		public GameSaveInfo GetLastGameSave()
		{
			return _gameSaves[_gameSaves.Count - 1];
		}

		public bool HasAnySaves()
		{
			return _gameSaves.Any();
		}
	}
}
