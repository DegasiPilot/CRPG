using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class NPCController : PersonageController
{
    private NavMeshObstacle _obstacleComponent;

    public override void Setup()
    {
        base.Setup();
        _controller.enabled = false;
        _obstacleComponent = GetComponent<NavMeshObstacle>();
    }

    public void MakeTurnInBattle()
    {
        Debug.Log("Start enemy turn");
        if (!BattleManager.HasAction)
        {
            return;
        }
        _obstacleComponent.enabled = false;
        _controller.enabled = true;
        Personage target;
        if (Personage.BattleTeam == BattleTeam.Enemies)
        {
            target = GameManager.Instance.PlayerPersonage; //TODO: replace to nearest player;
        }
        else
        {
            Debug.Log("Skip turn because not enemy");
            BattleManager.SetNextActivePersonage();
            return;
        }
        _controller.CalculatePath(target.transform.position, _navMeshPath);

        Vector3 lastPoint = CutPath(_navMeshPath, BattleManager.RemainMovement, out int lastIndex);
        float remainMovent = BattleManager.RemainMovement;
        Vector3? finalPos = null;
        for(int i = 0; i <= lastIndex - 1; i++)
        {
            Vector3 corner = _navMeshPath.corners[i];
            if (i > 0)
            {
                remainMovent -= Vector3.Distance(corner, _navMeshPath.corners[i -1]);
            }
            if (BattleManager.AttackRaycast(transform.position, target.transform.position, MaxAttackDistance + remainMovent, target))
            {
                float distanceToTarget = Vector3.Distance(corner, target.transform.position);
                if(distanceToTarget <= MaxAttackDistance)
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
        if(finalPos != null)
        {
            _controller.SetDestination((Vector3)finalPos);
            StartCoroutine(SheduleAction(target, Attack));
        }
        else
        {
            _controller.SetDestination(lastPoint);
            StartCoroutine(SheduleAction(target, null));
        }
    }

    private IEnumerator SheduleAction(Personage personage, Action<Personage> action)
    {
        yield return new WaitUntil(() => IsFree);
        action?.Invoke(personage);
        _controller.enabled = false;
        Debug.Log("End enemy turn");
        BattleManager.SetNextActivePersonage();
    }
}