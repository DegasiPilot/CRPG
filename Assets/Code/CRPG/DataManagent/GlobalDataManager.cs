using CRPG.DataManagement;
using CRPG.DataSaveSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CRPG
{
	public class GlobalDataManager : MonoBehaviour
	{
#if UNITY_EDITOR
		internal static IDataSaveLoader DataSaveLoader = LocalSaveLoader.Instance;
#else
		internal static IDataSaveLoader DataSaveLoader;
#endif

		[SerializeField] private AssetLabelReference _personageActionLabel;
		[SerializeField] private AssetLabelReference _raceInfoLabel;
		//[field: SerializeField] public string ItemsPrefix { get; private set; }

		private LazyAddresablesCollection<PersonageActionInfo> _personageActionsInfos;
		internal IList<PersonageActionInfo> PersonageActionsInfos
		{
			get
			{
				_personageActionsInfos ??= new LazyAddresablesCollection<PersonageActionInfo>(_personageActionLabel.labelString);
				return _personageActionsInfos.Collection;
			}
		}

		private LazyAddresablesCollection<RaceInfo> _raceInfos;
		internal IList<RaceInfo> RaceInfos
		{
			get
			{
				_raceInfos ??= new LazyAddresablesCollection<RaceInfo>(_raceInfoLabel.labelString);
				return _raceInfos.Collection;
			}
		}

		internal PersonageActionInfo GetActionInfo(ActionType actionType)
		{
			return PersonageActionsInfos.First(x => x.ActionType == actionType);
		}

		internal RaceInfo GetRaceInfo(Race race)
		{
			return RaceInfos.First(x => x.Race == race);
		}

		public GameObject GetClone(string key)
		{
			try
			{
				var operation = Addressables.LoadAssetAsync<GameObject>(key);
				operation.WaitForCompletion();
				GameObject go = Instantiate(operation.Result);
				operation.Release();
				return go;
			}
			catch
			{
				Debug.LogError("No addresable for key " + key);
				return null;
			}
		}
	}
}