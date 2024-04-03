using System.Collections.Generic;
using UnityEngine;

public class BattlePanel : MonoBehaviour
{
    public static BattlePanel Instance;

    public BattlePortraitController PortraitPrefab;

    private readonly List<BattlePortraitController> _portraitControllers = new();
    private int _activePortraitIndex;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Setup(Personage[] personages)
    {
        _portraitControllers.Capacity = personages.Length;
        foreach(Personage personage in personages)
        {
            var portaitController = Instantiate(PortraitPrefab, transform);
            portaitController.Setup(personage);
            _portraitControllers.Add(portaitController);
        }
        _activePortraitIndex = 0;
        _portraitControllers[_activePortraitIndex].Activate();
    }

    public void SetNextActivePersonage()
    {
        _portraitControllers[_activePortraitIndex].Disactivate();
        if (_activePortraitIndex < _portraitControllers.Count - 1)
        {
            _activePortraitIndex++;
        }
        else
        {
            _activePortraitIndex = 0;
            _portraitControllers.ForEach(x => x.SetPrepaired());
        }
        _portraitControllers[_activePortraitIndex].Activate();
    }

    public void OnBattleEnd()
    {
        foreach(var controller in _portraitControllers)
        {
            Destroy(controller);
        }
        _portraitControllers.Clear();
        gameObject.SetActive(false);
    }
}
