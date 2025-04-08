using CRPG.Battle;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public static class BattleManager
{
    [System.NonSerialized] public static UnityEvent OnBattleStartEvent = new();
    [System.NonSerialized] public static UnityEvent<PersonageController> OnPersonageJoinBattleEvent = new();
    [System.NonSerialized] public static UnityEvent OnBattleEndEvent = new();

    public static PersonageController ActivePersonage => _participantPersonages[_activePersonageIndex];
    public static List<PersonageController> ParticipantPersonages => _participantPersonages;

    public static float RemainMovement;
    public static int RemainActions;

    private static int _activePersonageIndex;
    private static List<PersonageController> _participantPersonages;
    private readonly static Dictionary<PersonageController, int> _personagesInitiative = new();

    public static void StartBattle(PersonageController[] participantPersonages)
    {
        _participantPersonages = participantPersonages.ToList();
        foreach(PersonageController controller in _participantPersonages)
        {
            controller.Personage.OnDeath.AddListener(() => OnPersonageDeath(controller));
        }
        _personagesInitiative.TrimExcess(participantPersonages.Length);
        SetInitiative();
        _activePersonageIndex = 0;
        SetupActivePersonage();
        OnBattleStartEvent.Invoke();
    }

    public static void JoinToBattle(PersonageController controller)
    {
        controller.Personage.OnDeath.AddListener(() => OnPersonageDeath(controller));
        AddToInitiative(controller);
        _activePersonageIndex = _participantPersonages.IndexOf(ActivePersonage);
        OnPersonageJoinBattleEvent.Invoke(controller);
    }

    private static void SetInitiative()
    {
        foreach(var controller in _participantPersonages)
        {
            int initiative = CharacteristicChecker.RoleCharacteristic(controller.Personage.PersonageInfo, Characteristics.Dexterity);
            _personagesInitiative[controller] = initiative;
            Debug.Log(controller.Personage.PersonageInfo.Name + " инициатива " + initiative);
        }
        _participantPersonages = (from pers in _personagesInitiative orderby pers.Value descending select pers.Key).ToList();
    }

    private static void AddToInitiative(PersonageController controller)
    {
        int initiative = CharacteristicChecker.RoleCharacteristic(controller.Personage.PersonageInfo, Characteristics.Dexterity);
        _personagesInitiative[controller] = initiative;
        _participantPersonages = (from pers in _personagesInitiative orderby pers.Value descending select pers.Key).ToList();
    }

    public static void SetNextActivePersonage()
    {
        if (_participantPersonages != null)
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
    }

    private static void SetupActivePersonage()
    {
        RemainMovement = ActivePersonage.Personage.PersonageInfo.Speed;
        RemainActions = ActivePersonage.Personage.PersonageInfo.ActionsPerTurn;
        if (ActivePersonage.Personage.BattleTeam == BattleTeam.Enemies)
        {
            if (ActivePersonage is NPCController NPCController)
            {
                NPCController.MakeTurnInBattle();
            }
            else
            {
                Debug.LogError("Enemy is not npc", ActivePersonage);
            }
        }
    }

    internal static void AfterAttack(PersonageController personageController)
    {
        RemainActions--;
        if (RemainActions <= 0)
        {
            personageController.EndAttack();
        }
	}

    public static bool CanAttack(Personage personage)
    {
        if (RemainActions > 0 &&
            AttackRaycast(ActivePersonage.Personage.HitPoint.position, personage.HitPoint.position, ActivePersonage.MaxAttackDistance, personage))
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
        if (ActivePersonage.IsFree)
        {
            SetNextActivePersonage();
            return true;
        }
        else
        {
            return false;
        }
    }

    private static void OnPersonageDeath(PersonageController controller)
    {
        _participantPersonages.Remove(controller);
        if(!_participantPersonages.Any(p => p.Personage.BattleTeam == BattleTeam.Enemies) || !_participantPersonages.Any(p => p.Personage.BattleTeam == BattleTeam.Allies))
        {
            EndBattle();
        }
    }
}