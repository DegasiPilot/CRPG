using CRPG.PersonageRedactor;
using System.Collections.Generic;
using UnityEngine;

public class ApperancePartRedactor : MonoBehaviour
{
	public AppearancePart MyAppearancePart;
	public bool IsOnlyMenPart;

	[SerializeField] private ControlGroup _appearacePartControl;
	private int _activeIndex;
	public int ActiveIndex => _activeIndex;

	private PersonageCreatorViewModel _creator;
	private List<GameObject> _parts
	{
		get
		{
			if (MyAppearancePart == AppearancePart.Hairs)
			{
				return _creator.Hairs;
			}
			else if (MyAppearancePart == AppearancePart.Beard)
			{
				return _creator.Beards;
			}
			else if (MyAppearancePart == AppearancePart.Face)
			{
				return _creator.Faces;
			}
			else
			{
				return null;
			}
		}
	}

	private void OnValidate()
	{
		if (_appearacePartControl == null)
		{
			_appearacePartControl = GetComponentInChildren<ControlGroup>();
		}
	}

	internal void Setup(PersonageCreatorViewModel creator)
	{
		_creator = creator;
		_appearacePartControl.Setup(AddPoint, RemovePoint, CanAddMore, CanRemoveMore, GetAmount);
	}

	private void AddPoint()
	{
		_activeIndex++;
		if (MyAppearancePart == AppearancePart.Face)
		{
			if (_activeIndex == _parts.Count)
			{
				_activeIndex = 0;
			}
		}
		else if (_activeIndex > _parts.Count + 1)
		{
			_activeIndex = 0;
		}
	}

	private void RemovePoint()
	{
		_activeIndex--;
		if (_activeIndex < 0)
		{
			if (MyAppearancePart == AppearancePart.Face)
			{
				_activeIndex = _parts.Count - 1;
			}
			else
			{
				_activeIndex = _parts.Count + 1;
			}
		}
	}

	private bool CanAddMore()
	{
		return true;
	}

	private bool CanRemoveMore()
	{
		return true;
	}

	public int GetAmount()
	{
		if (MyAppearancePart == AppearancePart.Face)
		{
			_parts.ForEach(x => x.SetActive(false));
			_parts[_activeIndex].SetActive(true);
		}
		else
		{
			if (_activeIndex == 0)
			{
				_parts.ForEach(x => x.SetActive(false));
			}
			else if (_activeIndex == _parts.Count + 1)
			{
				_parts.ForEach(x => x.SetActive(true));
			}
			else
			{
				_parts.ForEach(x => x.SetActive(false));
				_parts[_activeIndex - 1].SetActive(true);
			}
		}
		return _activeIndex;
	}

	public void GenderChanged(Gender gender)
	{
		_activeIndex = 0;
		_appearacePartControl.UpdateAmount();
		if (MyAppearancePart == AppearancePart.Beard)
		{
			if (gender == Gender.Female)
			{
				gameObject.SetActive(false);
			}
			else if (gender == Gender.Male)
			{
				gameObject.SetActive(true);
			}
		}
	}
}

public enum AppearancePart : byte
{
	Hairs = 0,
	Beard,
	Face,
}