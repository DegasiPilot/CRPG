using CRPG.DataSaveSystem.SaveData;
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
			if (gameSave.MainPlayerInfo.PersonageInfo.PersonagePortrait != null)
			{
				gameSave.MainPlayerInfo.PersonageInfo.ImageBytes = gameSave.MainPlayerInfo.PersonageInfo.PersonagePortrait.EncodeToPNG();
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
