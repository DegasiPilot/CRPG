using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace CRPG.DataSaveSystem
{
	[RequireComponent(typeof(SaveableGameobject), typeof(NavMeshObstacle))]
	class NavMeshObstacleSaver : MonoBehaviour, ISaveBlocker
	{
		[SerializeField] private SaveableGameobject _saveableGameobject;
		[SerializeField] private NavMeshObstacle _navMeshObstacle;
		[SerializeField] private SaveableGameobject _personage;

		[SerializeField, HideInInspector] private string _targetName;

		public bool IsBlockSave => !gameObject.activeSelf;

		private void OnValidate()
		{
			if (_saveableGameobject == null) _saveableGameobject = GetComponent<SaveableGameobject>();
			if (_navMeshObstacle == null) _navMeshObstacle = GetComponent<NavMeshObstacle>();
			if(_personage != null) _targetName = _personage.UniqueName;
		}

		private void Awake()
		{
			_saveableGameobject.AfterLoadEndEvent.AddListener(ListenPersonageDeath);
			_navMeshObstacle.enabled = true;
		}

		private void ListenPersonageDeath()
		{
			var target = GameManager.Instance.SceneSaveLoadManager.ObjectsToSave.First(x => x.UniqueName == _targetName);
			target.GetComponent<Personage>().OnDeath.AddListener(() => gameObject.SetActive(false));
		}
	}
}