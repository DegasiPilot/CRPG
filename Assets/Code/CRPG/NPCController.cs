using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using CRPG.Battle;
using CRPG.DataSaveSystem;
using CRPG.Interactions;
using static UnityEngine.UI.GridLayoutGroup;

[RequireComponent(typeof(NPCChooseAttackForceModule))]
public class NPCController : PersonageController
{
    [SerializeField] private NavMeshObstacle _obstacleComponent;

	protected override void OnValidate()
	{
		base.OnValidate();
		if(_obstacleComponent == null) _obstacleComponent = GetComponent<NavMeshObstacle>();
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
        if (BattleManager.RemainActions <= 0)
        {
            yield break;
        }
        _obstacleComponent.enabled = false;
        yield return null;
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
        if(BattleManager.AttackRaycast(Personage.HitPoint.position, target.Personage.HitPoint.position, MaxAttackDistance, target.Personage))
        {
            Debug.Log("Should Start Attacking");
            StartAttack(target);
            yield break;
        }
        else
        {
            _controller.CalculatePath(target.transform.position, _navMeshPath);
            Vector3 targetPos = Vector3.MoveTowards(_navMeshPath.corners[_navMeshPath.corners.Length - 1], _navMeshPath.corners[_navMeshPath.corners.Length - 2], _controller.radius + target.Radius + 0.01f);
            _controller.CalculatePath(targetPos, _navMeshPath);

            Vector3 lastPoint = CutPath(_navMeshPath, BattleManager.RemainMovement, out int lastIndex);
            float minDisatnce = Vector3.Distance(lastPoint, target.transform.position);
            if (minDisatnce <= MaxAttackDistance)
            {
                Vector3? finalPos = null;
                for (int i = 0; i <= lastIndex; i++)
                {
                    Vector3 corner = _navMeshPath.corners[i];
                    float distanceToTarget = Vector3.Distance(corner, target.transform.position);
                    if (distanceToTarget <= MaxAttackDistance)
                    {
                        if (BattleManager.AttackRaycast(new Vector3(corner.x, target.Personage.HitPoint.position.y, corner.z), target.Personage.HitPoint.position, MaxAttackDistance, target.Personage))
                        {
                            if (i == 0)
                            {
                                finalPos = corner;
                            }
                            else
                            {
                                finalPos = Vector3.Lerp(corner, _navMeshPath.corners[i - 1], MaxAttackDistance - distanceToTarget);
                            }
                        }
                    }
                }
                if (finalPos.HasValue)
                {
                    InteractWith(MaxAttackDistance, finalPos.Value, new AttackInteract(target));
                }
                else
                {
                    Debug.Log("Enemy finalPos is null\n lastPoint is " + lastPoint);
                    lastPoint = Vector3.Lerp(lastPoint, _navMeshPath.corners[lastIndex], MaxAttackDistance - minDisatnce);
                    InteractWith(MaxAttackDistance, lastPoint, new AttackInteract(target));
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
        float nearestDistance = Vector3.Distance(nearestPlayer.transform.position, transform.position);
        foreach(var companion in GameData.Companions)
        {
            float distance = Vector3.Distance(companion.transform.position, transform.position);
            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPlayer = companion;
            }
		}
        return nearestPlayer;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        Destroy(_collider);
        if(TryGetComponent(out DialogueActor dialogueActor))
        {
            Destroy(dialogueActor);
        }
        Destroy(this);
        Destroy(_controller);
    }

	public override void EndAttack()
	{
		base.EndAttack();
        if(BattleManager.ActivePersonageController == this)
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