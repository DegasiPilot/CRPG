using CRPG.DataManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CRPG
{
    public class GlobalDataManager : MonoBehaviour
    {
		[SerializeField] private AssetLabelReference _personageActionLabel;
		[SerializeField] private AssetLabelReference _raceInfoLabel;

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
			var operation = Addressables.LoadAssetAsync<GameObject>(key);
			operation.WaitForCompletion();
			GameObject go = Instantiate(operation.Result);
			operation.Release();
			return go;
		}
	}
}