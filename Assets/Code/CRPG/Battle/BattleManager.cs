using CRPG;
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

	public static PersonageController ActivePersonageController => _participantPersonages[_activePersonageIndex];
	public static List<PersonageController> ParticipantPersonages => _participantPersonages;

	public static float RemainMovement;

	private static int _activePersonageIndex;
	private static List<PersonageController> _participantPersonages;
	private readonly static Dictionary<PersonageController, int> _personagesInitiative = new();
	private readonly static FigtherInfo[] _activeFigthers = new FigtherInfo[2];

	public static void StartBattle(PersonageController[] participantPersonages)
	{
		_participantPersonages = participantPersonages.ToList();
		foreach (PersonageController controller in _participantPersonages)
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
		_activePersonageIndex = _participantPersonages.IndexOf(ActivePersonageController);
		OnPersonageJoinBattleEvent.Invoke(controller);
	}

	private static void SetInitiative()
	{
		foreach (var controller in _participantPersonages)
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
				EndRound();
				_activePersonageIndex = 0;
			}
			SetupActivePersonage();
		}
	}

	private static void EndRound()
	{
		foreach (PersonageController personageController in _participantPersonages)
		{
			personageController.Personage.RemainActions = personageController.Personage.MaxActions;
			personageController.Personage.Stamina +=
				GlobalRules.BaseRestoreEnergyPerRound +
				(personageController.Personage.RemainActions / personageController.Personage.PersonageInfo.ActionsPerTurn)
				* GlobalRules.BaseRestoreEnergyPerRound;
		}
	}

	private static void SetupActivePersonage()
	{
		RemainMovement = ActivePersonageController.Personage.PersonageInfo.Speed;
		if (ActivePersonageController.Personage.BattleTeam == BattleTeam.Enemies)
		{
			if (ActivePersonageController is NPCController NPCController)
			{
				NPCController.MakeTurnInBattle();
			}
			else
			{
				Debug.LogError("Enemy is not npc", ActivePersonageController);
			}
		}
	}

	internal static void StartAttack(PersonageController attacker, PersonageController defender)
	{
		_activeFigthers[0] = new FigtherInfo() { PersonageController = attacker };
		_activeFigthers[1] = new FigtherInfo() { PersonageController = defender };
		if (ParticipantPersonages != null && ParticipantPersonages.Contains(defender) &&
			defender.Personage.RemainActions > 0)
		{
			bool canDefenderAttack = AttackRaycast(defender.Personage, defender.MaxAttackDistance, attacker.Personage);
			attacker.AttackModule.Attack(defender, false, true, canDefenderAttack);
			defender.AttackModule.Attack(attacker, true, canDefenderAttack, true);
		}
		else
		{
			attacker.AttackModule.Attack(defender, false, true, false);
			EnergyChoosed(defender, 0, 0);
		}
	}

	internal static void EnergyChoosed(PersonageController personageController, float attack, float defence)
	{
		string message = string.Empty;
		bool isAllReady = true;
		for (int i = 0; i < _activeFigthers.Length; i++)
		{
			if (personageController == _activeFigthers[i].PersonageController)
			{
				_activeFigthers[i].EnergyToAttack = attack;
				_activeFigthers[i].EnergyToDefend = defence;
			}
			isAllReady = isAllReady && _activeFigthers[i].IsReady;
		}
		if (isAllReady)
		{
			for (int i = 0; i < _activeFigthers.Length; i++)
			{
				bool isSpendAction = false;
				if (_activeFigthers[i].EnergyToAttack > 0)
				{
					isSpendAction = true;
					for (int j = 0; j < _activeFigthers.Length; j++)
					{
						if (i != j)
						{
							if (IsDodged(_activeFigthers[j]))
							{
								if (!string.IsNullOrEmpty(message))
								{
									message += "\n";
								}
								message += _activeFigthers[j].PersonageController.Personage.PersonageInfo.Name
									+ " уклонился";
								_activeFigthers[i].PersonageController.AttackModule.EndAttack();
							}
							else
							{
								_activeFigthers[i].PersonageController.AttackModule.StartAttackCoroutine();
							}
						}
					}
				}
				else if (_activeFigthers[i].PersonageController.AttackModule.IsAttacking)
				{
					_activeFigthers[i].PersonageController.AttackModule.EndAttack();
				}
				if (GameManager.Instance.GameMode == GameMode.Battle)
				{
					if(_activeFigthers[i].EnergyToDefend > 0)
					{
						isSpendAction = true;
						_activeFigthers[i].PersonageController.Personage.Stamina -= _activeFigthers[i].EnergyToDefend;
					}

					if (isSpendAction)
					{
						_activeFigthers[i].PersonageController.Personage.RemainActions--;
					}
				}

				if (!string.IsNullOrEmpty(message))
				{
					GameManager.Instance.ShowAttackResult(message);
				}
			}
		}
	}

	internal static bool IsDodged(FigtherInfo figtherInfo)
	{
		float dodgeChance = figtherInfo.EnergyToDefend * figtherInfo.PersonageController.Personage.DodgeCoefficient;
		float result = Random.Range(0, 0.999f);
		return result <= dodgeChance;
	}

	public static bool CanAttack(Personage personage)
	{
		if (ActivePersonageController.Personage.RemainActions > 0 &&
			AttackRaycast(ActivePersonageController.Personage, ActivePersonageController.MaxAttackDistance, personage))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static bool AttackRaycast(Personage attacker, float maxDistance, Personage targetPersonage)
	{
		bool result = AttackRaycast(attacker.HitPoint.position, maxDistance, targetPersonage);
		return result;
	}

	public static bool AttackRaycast(Vector3 attackerPos, float maxDistance, Personage targetPersonage)
	{
		Ray ray = new Ray(attackerPos, targetPersonage.HitPoint.position - attackerPos);
		if (Physics.Raycast(ray, out RaycastHit hit, maxDistance + 0.03f /*Прощаем небольшую погрешность*/))
		{
			if (hit.collider.gameObject.TryGetComponent(out Personage personage) && personage == targetPersonage)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	public static void EndBattle()
	{
		foreach (PersonageController personageController in _participantPersonages)
		{
			personageController.Personage.Stamina = personageController.Personage.PersonageInfo.MaxStamina;
			personageController.Personage.RemainActions = personageController.Personage.MaxActions;
		}
		OnBattleEndEvent.Invoke();
		_participantPersonages = null;
		_personagesInitiative.Clear();
	}

	public static bool PlayerTryEndTurn()
	{
		if (ActivePersonageController.IsFree && ActivePersonageController.Personage.BattleTeam == BattleTeam.Allies &&
			ActivePersonageController is PlayerController)
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
		if (!_participantPersonages.Any(p => p.Personage.BattleTeam == BattleTeam.Enemies) || !_participantPersonages.Any(p => p.Personage.BattleTeam == BattleTeam.Allies))
		{
			EndBattle();
		}
		else
		{
			_activePersonageIndex = _participantPersonages.IndexOf(ActivePersonageController);
		}
	}

	internal static BattleTeam GetOppostiteTeam(BattleTeam battleTeam)
	{
		switch (battleTeam)
		{
			case BattleTeam.Allies:
				return BattleTeam.Enemies;
			case BattleTeam.Enemies:
				return BattleTeam.Allies;
			default:
				throw new System.ArgumentException("incorrect team", "battleTeam");
		}
	}
}