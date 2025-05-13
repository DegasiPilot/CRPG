using System.Linq;
using UnityEngine;

namespace CRPG.DataSaveSystem
{
	[RequireComponent(typeof(SaveableGameobject))]
	class NavMeshObstacleSaver : MonoBehaviour, ISaveBlocker
	{
		[SerializeField] private SaveableGameobject _saveableGameobject;
		[SerializeField] private string _targetName;

		public bool IsBlockSave => !gameObject.activeSelf;

		private void OnValidate()
		{
			if (_saveableGameobject == null) _saveableGameobject = GetComponent<SaveableGameobject>();
		}

		private void Awake()
		{
			_saveableGameobject.AfterLoadEndEvent.AddListener(ListenPersonageDeath);
		}

		private void ListenPersonageDeath()
		{
			var target = GameManager.Instance.SceneSaveLoadManager.ObjectsToSave.First(x => x.UniqueName == _targetName);
			target.GetComponent<Personage>().OnDeath.AddListener(() => gameObject.SetActive(false));
		}
	}
}