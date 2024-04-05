using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class PlayerController : PersonageController
{
    public float PathEndHitAccuracy; //Ќасколько близко надо нажать к концу пути
    public LineRenderer AccesableLineRenderer;
    public LineRenderer UnaccesableLineRenderer;

    public void OnGroundPressedInFree(Vector3 hitPoint)
    {
        if (!IsFree) return;
        switch (_activeAction)
        {
            case ActionType.Movement:
                GoToPosition(hitPoint);
                break;
            case ActionType.Jumping:
                JumpToPosition(hitPoint);
                break;
        }
        if(_activeAction != _defaultAction)
        {
            SetDefaultAction();
        }
    }

    public void OnGroundPressedInBattle(Vector3 hitPoint)
    {
        if (!IsFree) return;
        if(_activeAction == ActionType.Movement && BattleManager.RemainMovement > 0 && BattleManager.ActivePersonage == Personage)
        {
            Debug.Log($"{hitPoint} {_lastHitPoint} {PathEndHitAccuracy}");
            if(_lastHitPoint != Vector3.positiveInfinity && _navMeshPath != null && _navMeshPath.status == NavMeshPathStatus.PathComplete &&
                Vector3.Distance(hitPoint, _lastHitPoint) <= PathEndHitAccuracy)
            {
                Debug.Log("я погнал");
                _controller.destination = _lastAccessablePathDot;
                _navMeshPath.ClearCorners();
                _lastHitPoint = Vector3.positiveInfinity;
                UnaccesableLineRenderer.enabled = false;
                AccesableLineRenderer.enabled = false;
                BattleManager.RemainMovement -= Mathf.Min(BattleManager.RemainMovement, _pathLength);
                _pathLength = 0;
                return;
            }
            _controller.CalculatePath(hitPoint, _navMeshPath);
            _lastHitPoint = hitPoint;

            if(_navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                _pathLength = PathLength(_navMeshPath);
                if (_pathLength > BattleManager.RemainMovement)
                {
                    UnaccesableLineRenderer.enabled = true;
                    AccesableLineRenderer.enabled = true;
                    _lastAccessablePathDot = CutPath(_navMeshPath, BattleManager.RemainMovement, out int lastIndex);
                    var accesablePath = _navMeshPath.corners.Take(lastIndex + 1).ToArray();
                    accesablePath[lastIndex] = _lastAccessablePathDot;
                    AccesableLineRenderer.positionCount = accesablePath.Length;
                    AccesableLineRenderer.SetPositions(accesablePath);
                    var unaccesablePath = _navMeshPath.corners.TakeLast(_navMeshPath.corners.Length -
                        accesablePath.Length + 2).ToArray();
                    unaccesablePath[0] = _lastAccessablePathDot;
                    UnaccesableLineRenderer.positionCount = unaccesablePath.Length;
                    UnaccesableLineRenderer.SetPositions(unaccesablePath);
                }
                else
                {
                    UnaccesableLineRenderer.enabled = false;
                    AccesableLineRenderer.enabled = true;
                    _lastAccessablePathDot = _navMeshPath.corners.Last();
                    AccesableLineRenderer.positionCount = _navMeshPath.corners.Count();
                    AccesableLineRenderer.SetPositions(_navMeshPath.corners);
                }
            }
            else
            {
                AccesableLineRenderer.enabled = false;
                UnaccesableLineRenderer.enabled = true;
                _lastAccessablePathDot = Vector3.zero;
                UnaccesableLineRenderer.positionCount = 2;
                UnaccesableLineRenderer.SetPositions(new Vector3[2] { transform.position, hitPoint});
            }
        }
    }

    public override void SetDefaultAction()
    {
        base.SetDefaultAction();
        CanvasManager.Instance.ForceChangeAction(_defaultAction);
    }
}