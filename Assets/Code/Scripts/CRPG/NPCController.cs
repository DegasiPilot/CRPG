using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using CRPG.Battle;
using CRPG.DataSaveSystem;

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
            Debug.Log("ShouldStartAttacking");
            StartCoroutine(SheduleAction(target, StartAttack, () => !IsAttacking));
            yield break;
        }
        else
        {
            _controller.CalculatePath(target.transform.position, _navMeshPath);
            Vector3 targetPos = Vector3.MoveTowards(_navMeshPath.corners[_navMeshPath.corners.Length - 1], _navMeshPath.corners[_navMeshPath.corners.Length - 2], _controller.radius + target.Radius + 0.01f);
            _controller.CalculatePath(targetPos, _navMeshPath);

            Vector3 lastPoint = CutPath(_navMeshPath, BattleManager.RemainMovement, out int lastIndex);
            float remainMovent = BattleManager.RemainMovement;
            Vector3? finalPos = null;
            for (int i = 0; i <= lastIndex - 1; i++)
            {
                Vector3 corner = _navMeshPath.corners[i];
                if (i > 0)
                {
                    remainMovent -= Vector3.Distance(corner, _navMeshPath.corners[i - 1]);
                }
                if (BattleManager.AttackRaycast(Personage.HitPoint.position, target.Personage.HitPoint.position, MaxAttackDistance + remainMovent, target.Personage))
                {
                    float distanceToTarget = Vector3.Distance(corner, target.transform.position);
                    if (distanceToTarget <= MaxAttackDistance)
                    {
                        finalPos = corner;
                    }
                    else
                    {
                        finalPos = Vector3.Lerp(corner, _navMeshPath.corners[i + 1], (distanceToTarget - MaxAttackDistance) / distanceToTarget);
                    }
                    break;
                }
            }
            if (finalPos != null)
            {
                _controller.SetDestination((Vector3)finalPos);
                StartCoroutine(SheduleAction(target, StartAttack, () => !IsAttacking));
            }
            else
            {
                Debug.Log("Enemy finalPos is null\n lastPoint is " + lastPoint);
                _controller.SetDestination(lastPoint);
                StartCoroutine(SheduleAction(target, null, null));
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
                nearestPlayer = companion.PlayerController;
            }
		}
        return nearestPlayer;
    }

    private IEnumerator SheduleAction(PersonageController personageController, System.Action<PersonageController> action, System.Func<bool> actionEndCheck)
    {
        if(action != null)
        {
            action.Invoke(personageController);
            yield return new WaitUntil(actionEndCheck);
        }
        _controller.enabled = false;
        _obstacleComponent.enabled = true;
        Debug.Log("End enemy turn");
        BattleManager.SetNextActivePersonage();
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
}