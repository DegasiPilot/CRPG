using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleManager
{
    public static Personage ActivePersonage => _participantPersonages[_activePersonageIndex];
    public static float RemainMovement;
    public static bool HasAction;

    private static int _activePersonageIndex;
    private static Personage[] _participantPersonages;
    private readonly static Dictionary<Personage, int> _personagesInitiative = new();

    public static void StartBattle(Personage[] participantPersonages)
    {
        _participantPersonages = participantPersonages;
        _personagesInitiative.TrimExcess(participantPersonages.Length);
        SetInitiative();
        _activePersonageIndex = 0;
        SetupActivePersonage();
        GameManager.Instance.ChangeGameMode(GameMode.Battle);
        BattlePanel.Instance.gameObject.SetActive(true);
        BattlePanel.Instance.Setup(_participantPersonages);
    }

    private static void SetInitiative()
    {
        foreach(var personage in _participantPersonages)
        {
            int initiative = CharacteristicChecker.RoleCharacteristic(personage.PersonageInfo, Characteristics.Dexterity);
            _personagesInitiative[personage] = initiative;
            Debug.Log(personage.PersonageInfo.Name + " инициатива " + initiative);
        }
        _participantPersonages = (from pers in _personagesInitiative orderby pers.Value descending select pers.Key).ToArray();
    }

    public static void SetNextActivePersonage()
    {
        BattlePanel.Instance.SetNextActivePersonage();
        if (_activePersonageIndex < _participantPersonages.Length)
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
    }

    public static bool CanAttack(this Personage attacker, Personage target, WeaponInfo weapon)
    {
        if(HasAction && Vector3.Distance(attacker.transform.position, target.transform.position) <= weapon.MaxAttackDistance)
        {
            if(AttackRaycast(attacker.transform.position, target.transform.position, weapon.MaxAttackDistance, target))
            {
                return true;
            }
        }
        return false;
    }

    private static bool AttackRaycast(Vector3 attackerPos, Vector3 targetPos, float maxDistance, Personage targetPersonage)
    {
        Ray ray = new Ray(attackerPos, (targetPos - attackerPos).normalized);
        if(Physics.Raycast(ray, out RaycastHit hit, maxDistance)
            && hit.collider.gameObject.TryGetComponent(out Personage personage) && personage == targetPersonage)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void EndBattle()
    {
        BattlePanel.Instance.OnBattleEnd();
        _participantPersonages = null;
        _personagesInitiative.Clear();
    }
}