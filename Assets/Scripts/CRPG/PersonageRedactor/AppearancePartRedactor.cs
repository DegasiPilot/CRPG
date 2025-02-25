using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ApperancePartRedactor : MonoBehaviour
{
    public AppearancePart MyAppearancePart;

    private ControlGroup _appearacePartControl;
    private int _activeIndex;
    public int ActiveIndex => _activeIndex;

    private PersonageCreator _creator => PersonageCreator.Instance;
    private List<GameObject> _parts
    {
        get
        {
            if(MyAppearancePart == AppearancePart.Hairs)
            {
                return _creator.Hairs;
            }
            else if(MyAppearancePart == AppearancePart.Beard)
            {
                return _creator.Beards;
            }
            else if(MyAppearancePart == AppearancePart.Face)
            {
                return _creator.Faces;
            }
            else
            {
                return null;
            }
        }
    }

    public void Setup()
    {
        var texts = GetComponentsInChildren<Text>();
        _appearacePartControl = GetComponentInChildren<ControlGroup>();
        _appearacePartControl.Setup(AddPoint, RemovePoint, CanAddMore, CanRemoveMore, GetAmount);
        _creator.OnGenderChanged.AddListener(GenderChanged);
    }

    private void AddPoint()
    {
        _activeIndex++;
        if(MyAppearancePart == AppearancePart.Face)
        {
            if (_activeIndex == _parts.Count)
            {
                _activeIndex = 0;
            }
        }
        else if(_activeIndex > _parts.Count + 1)
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

    private void GenderChanged(Gender gender)
    {
        _activeIndex = 0;
        _appearacePartControl.UpdateAmount();
        if(MyAppearancePart == AppearancePart.Beard)
        {
            if(gender == Gender.Female)
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