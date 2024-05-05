using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public static class BattleManager
{
    [System.NonSerialized] public static UnityEvent OnBattleStartEvent = new();
    [System.NonSerialized] public static UnityEvent OnBattleEndEvent = new();

    public static Personage ActivePersonage => _participantPersonages[_activePersonageIndex];
    public static List<Personage> ParticipantPersonages => _participantPersonages;

    public static float RemainMovement;
    public static bool HasAction;

    private static int _activePersonageIndex;
    private static List<Personage> _participantPersonages;
    private readonly static Dictionary<Personage, int> _personagesInitiative = new();

    public static void StartBattle(Personage[] participantPersonages)
    {
        _participantPersonages = participantPersonages.ToList();
        foreach(Personage personage in _participantPersonages)
        {
            personage.OnDeath.AddListener(() => OnPersonageDeath(personage));
        }
        _personagesInitiative.TrimExcess(participantPersonages.Length);
        SetInitiative();
        _activePersonageIndex = 0;
        SetupActivePersonage();
        OnBattleStartEvent.Invoke();
    }

    private static void SetInitiative()
    {
        foreach(var personage in _participantPersonages)
        {
            int initiative = CharacteristicChecker.RoleCharacteristic(personage.PersonageInfo, Characteristics.Dexterity);
            _personagesInitiative[personage] = initiative;
            Debug.Log(personage.PersonageInfo.Name + " инициатива " + initiative);
        }
        _participantPersonages = (from pers in _personagesInitiative orderby pers.Value descending select pers.Key).ToList();
    }

    public static void SetNextActivePersonage()
    {
        BattleUIManager.Instance.SetNextActivePersonage();
        if (_activePersonageIndex < _participantPersonages.Count - 1)
        {
            _activePersonageIndex++;
        }
        else
        {
            _activePersonageIndex = 0;
        }
        SetupActivePersonage();
    }

    private static void SetupActivePersonage()
    {
        RemainMovement = ActivePersonage.PersonageInfo.Speed;
        HasAction = true;
        if (ActivePersonage.BattleTeam == BattleTeam.Enemies)
        {
            (ActivePersonage.Controller as NPCController).MakeTurnInBattle();
        }
    }

    public static bool CanAttack(Personage personage)
    {
        if (HasAction &&
            AttackRaycast(ActivePersonage.HitPoint.position, personage.HitPoint.position, ActivePersonage.Controller.MaxAttackDistance, personage))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool AttackRaycast(Vector3 attackerPos, Vector3 targetPos, float maxDistance, Personage targetPersonage)
    {
        Ray ray = new Ray(attackerPos, targetPos - attackerPos);
        if(Physics.Raycast(ray, out RaycastHit hit, maxDistance)
            && hit.collider.gameObject.TryGetComponent(out Personage personage) && personage == targetPersonage)
        {
            return true;
        }
        else
        {
            Debug.DrawRay(attackerPos, targetPos - attackerPos, Color.red, maxDistance);
            return false;
        }
    }

    public static void EndBattle()
    {
        OnBattleEndEvent.Invoke();
        _participantPersonages = null;
        _personagesInitiative.Clear();
    }

    public static bool TryEndTurn()
    {
        if (ActivePersonage.Controller.IsFree)
        {
            SetNextActivePersonage();
            return true;
        }
        else
        {
            return false;
        }
    }

    private static void OnPersonageDeath(Personage personage)
    {
        _participantPersonages.Remove(personage);
        if(!_participantPersonages.Any(p => p.BattleTeam == BattleTeam.Enemies) || !_participantPersonages.Any(p => p.BattleTeam == BattleTeam.Allies))
        {
            EndBattle();
        }
    }
}