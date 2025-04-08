using System.Collections.Generic;
using UnityEngine;

public class BattlePanel : MonoBehaviour
{
    public BattlePortraitController PortraitPrefab;

    private readonly List<BattlePortraitController> _portraitControllers = new();
    private int _activePortraitIndex;

    public void Setup(List<PersonageController> personageControllers)
    {
        _portraitControllers.Capacity = personageControllers.Count;
        foreach(PersonageController controller in personageControllers)
        {
            var portaitController = Instantiate(PortraitPrefab, transform);
            portaitController.Setup(controller.Personage);
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
            Destroy(controller.gameObject);
        }
        _portraitControllers.Clear();
    }

    public void OnPersonageJoinBattle(PersonageController personage)
    {
        var portaitController = Instantiate(PortraitPrefab, transform);
        portaitController.Setup(personage.Personage);
        BattlePortraitController portrait = _portraitControllers[_activePortraitIndex];
        _portraitControllers.Add(portaitController);
        _activePortraitIndex = _portraitControllers.IndexOf(portrait);
    }
}