using CRPG.Battle;
using CRPG.DataSaveSystem;
using CRPG.Interactions;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCChooseAttackForceModule))]
public class NPCController : PersonageController
{
	[SerializeField] private NavMeshObstacle _obstacleComponent;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (_obstacleComponent == null) _obstacleComponent = GetComponent<NavMeshObstacle>();
		if (_chooseAttackForceModule == null)
		{
			_chooseAttackForceModule = GetComponent<NPCChooseAttackForceModule>();
		}
	}

	protected override void Setup()
	{
		base.Setup();
		_controller.enabled = false;
	}

	public void MakeTurnInBattle()
	{
		StartCoroutine(MakingTurnRoutine());
	}

	private IEnumerator MakingTurnRoutine()
	{
		Debug.Log("Start enemy turn");
		if (Personage.RemainActions <= 0 || Personage.Stamina < Personage.MinAttackEnergy)
		{
			Debug.Log("Skip turn");
			EndTurn();
			yield break;
		}
		_obstacleComponent.enabled = false;
		yield return new WaitForEndOfFrame();
		_controller.enabled = true;
		PersonageController target;
		if (Personage.BattleTeam == BattleTeam.Enemies)
		{
			target = NearestPlayer();
		}
		else
		{
			Debug.Log("Skip turn because not enemy");
			BattleManager.SetNextActivePersonage();
			yield break;
		}
		if (BattleManager.AttackRaycast(Personage, MaxAttackDistance, target.Personage))
		{
			StartAttack(target);
			yield break;
		}
		else
		{

			_controller.CalculatePath(target.transform.position, _navMeshPath);
			Vector3 targetPos = Vector3.MoveTowards(_navMeshPath.corners[_navMeshPath.corners.Length - 1], _navMeshPath.corners[_navMeshPath.corners.Length - 2], _controller.radius + target.Radius + 0.01f);
			_controller.CalculatePath(targetPos, _navMeshPath);

			Vector3 lastPoint = CutPath(_navMeshPath, BattleManager.RemainMovement, out int lastIndex);
			float minDisatnce = Vector3.Distance(lastPoint, targetPos);
			if (minDisatnce <= MaxAttackDistance)
			{
				Vector3? finalPos = null;
				for (int i = 0; i <= lastIndex; i++)
				{
					Vector3 corner = _navMeshPath.corners[i];
					if (BattleManager.AttackRaycast(new Vector3(corner.x, target.Personage.HitPoint.position.y, corner.z), MaxAttackDistance, target.Personage))
					{
						if (i == 0)
						{
							finalPos = corner;
						}
						else
						{
							finalPos = Vector3.MoveTowards(corner, _navMeshPath.corners[i - 1], MaxAttackDistance + target.Radius - Vector3.Distance(corner, target.transform.position));
							Debug.Log("NPC distance to target: " + (Vector3.Distance(finalPos.Value, target.transform.position) - target.Radius));
						}
					}
				}
				if (finalPos.HasValue)
				{
					InteractWith(0.01f, finalPos.Value, new AttackInteract(target));
				}
				else
				{
					Debug.Log("Enemy finalPos is null\n lastPoint is " + lastPoint);
					lastPoint = Vector3.MoveTowards(lastPoint, _navMeshPath.corners[lastIndex], MaxAttackDistance - minDisatnce);
					InteractWith(0.01f, lastPoint, new AttackInteract(target));
				}
			}
			else
			{
				InteractWith(0.01f, lastPoint, new ActionInteract(EndTurn));
			}
		}
	}

	private PlayerController NearestPlayer()
	{
		PlayerController nearestPlayer = GameData.MainPlayer.PlayerController;
		return nearestPlayer;
	}

	protected override void OnDeath()
	{
		foreach (var item in _personage.EquipmentManager.EquipableItems)
		{
			GameManager.Instance.SceneSaveLoadManager.ObjectsToSave.Add(item.GetComponent<SaveableGameobject>());
		}
		base.OnDeath();
		Destroy(_collider);
		if (TryGetComponent(out DialogueActor dialogueActor))
		{
			Destroy(dialogueActor);
		}
		Destroy(this);
		Destroy(_controller);
	}

	public override void EndAttack()
	{
		base.EndAttack();
		if (GameManager.Instance.GameMode != GameMode.Battle || BattleManager.ActivePersonageController == this)
		{
			EndTurn();
		}
	}

	private void EndTurn()
	{
		_controller.enabled = false;
		_obstacleComponent.enabled = true;
		Debug.Log("End enemy turn");
		BattleManager.SetNextActivePersonage();
	}
}