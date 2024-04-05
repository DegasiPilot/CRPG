using UnityEngine;
using UnityEngine.AI;

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
        _obstacleComponent.enabled = false;
        _controller.enabled = true;
        Personage target;
        if (Personage.battleTeam == BattleTeam.Enemies)
        {
            target = GameManager.Instance.PlayerPersonage; //TODO: replace to nearest player;
        }
        else
        {
            return;
        }
        _controller.CalculatePath(target.transform.position, _navMeshPath);
        float dictanceToAttackZone = PathLength(_navMeshPath) - WeaponInfo.MaxAttackDistance;

        bool canHit = BattleManager.AttackRaycast(transform.position, target.transform.position,
            WeaponInfo.MaxAttackDistance, target);

        if (dictanceToAttackZone <= 0)
        {
            Attack(target);
            foreach(Vector3 corner in _navMeshPath.corners)
            {
            }
        }
        _controller.enabled = false;
    }
}