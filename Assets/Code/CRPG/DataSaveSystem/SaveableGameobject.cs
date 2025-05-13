using CRPG;
using CRPG.DataSaveSystem;
using CRPG.DataSaveSystem.SaveData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SaveableGameobject : MonoBehaviour
{
	[Tooltip("Уникальное имя для объекта этого типа")]
	public string UniqueName;
	[SerializeField] private MonoBehaviour[] _saveableComponents;

	private ISaveableComponent[] _saveables;
	internal ISaveableComponent[] Saveables
	{
		get
		{
			if (_saveables == null)
			{
				InitCollections();
			}
			return _saveables;
		}
	}

	private ISaveBlocker[] _saveBlockers;
	internal ISaveBlocker[] SaveBlockers
	{
		get
		{
			if (_saveBlockers == null)
			{
				InitCollections();
			}
			return _saveBlockers;
		}
	}

	private void InitCollections()
	{
		List<ISaveableComponent> saveables = new List<ISaveableComponent>(_saveableComponents.Length);
		List<ISaveBlocker> saveBlockers = new List<ISaveBlocker>(_saveableComponents.Length);

		for (int i = 0; i < _saveableComponents.Length; i++)
		{
			if (_saveableComponents[i] is ISaveableComponent saveableComponent)
			{
				saveables.Add(saveableComponent);
			}
			if (_saveableComponents[i] is ISaveBlocker saveBlocker)
			{
				saveBlockers.Add(saveBlocker);
			}
		}

		_saveables = saveables.ToArray();
		_saveBlockers = saveBlockers.ToArray();
	}

	public SaveObjectInfo GetSaveInfo(GlobalDataManager globalDataManager)
	{
		foreach (var saveBlocker in SaveBlockers)
		{
			if (saveBlocker.IsBlockSave)
			{
				return null;
			}
		}
		SaveObjectInfo info = new SaveObjectInfo()
		{
			Pos = transform.position,
			Rot = transform.eulerAngles,
			IsActive = isActiveAndEnabled,
		};

		if (Saveables != null && Saveables.Length > 0)
		{
			info.ComponentsInfo = new object[Saveables.Length];
			for (int i = 0; i < info.ComponentsInfo.Length; i++)
			{
				info.ComponentsInfo[i] = Saveables[i].Save();
			}
		}

		info.Name = UniqueName;

		return info;
	}

	public void LoadSaveInfo(SaveObjectInfo info)
	{
		if (info.ComponentsInfo != null && info.ComponentsInfo.Length > 0)
		{
			for (int i = 0; i < Saveables.Length; i++)
			{
				Saveables[i].Load(info.ComponentsInfo);
			}
		}
		transform.SetPositionAndRotation(info.Pos, Quaternion.Euler(info.Rot));
		gameObject.SetActive(info.IsActive);
	}

	public void AfterLoadEnd()
	{
		AfterLoadEndEvent.Invoke();
	}
	public UnityEvent AfterLoadEndEvent;
}