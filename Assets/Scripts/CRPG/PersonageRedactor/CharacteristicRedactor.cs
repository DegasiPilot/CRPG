using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacteristicRedactor : MonoBehaviour
{
    public Characteristics Characteristic;

    private ControlGroup statControl;
    private Text _bonusText;

    private PersonageCreator _creator => PersonageCreator.Instance;

    public void Setup()
    {
        var texts = GetComponentsInChildren<Text>();
        _bonusText = texts.First(x => x.name == "Bonus");
        statControl = GetComponentInChildren<ControlGroup>();
        statControl.Setup(AddPoint, RemovePoint, CanAddMore, CanRemoveMore, GetAmount);
        _creator.OnNoMoreStatPoints.AddListener(statControl.DeactivatePlusButton);
        _creator.OnGetStatPoints.AddListener(statControl.TryActivatePlusButton);
    }

    private void AddPoint()
    {
        _creator.AddStatPoint(Characteristic);
    }

    private void RemovePoint()
    {
        _creator.RemoveStatPoint(Characteristic);
    }

    private bool CanAddMore()
    {
        return _creator.CanAddMore(Characteristic);
    }

    private bool CanRemoveMore()
    {
        return _creator.CanRemoveMore(Characteristic);
    }

    public void UpdateAmount()
    {
        statControl.UpdateAmount();
        var statValue = _creator.GetStatValue(Characteristic);
        UpdateBonus(statValue);
    }

    public int GetAmount()
    {
        var statValue = _creator.GetStatValue(Characteristic);
        return statValue.Item1 + statValue.Item2;
    }

    public void UpdateBonus((int,int) statValue)
    {
        if (statValue.Item2 > 0)
        {
            _bonusText.enabled = true;
            _bonusText.text = $"+{statValue.Item2}";
        }
        else
        {
            _bonusText.enabled = false;
        }
    }
}