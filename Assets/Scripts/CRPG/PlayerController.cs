using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class PlayerController : PersonageController
{
    public float PathEndHitAccuracy; //Ќасколько близко надо нажать к концу пути
    public LineRenderer AccesableLineRenderer;
    public LineRenderer UnaccesableLineRenderer;
    public GameObject Sphere;
    public float JumpSpeedMultiplier;

    [System.NonSerialized] public GameObject Inventory;

    private float _normalAgentSpeed;

    public override void Setup()
    {
        base.Setup();
        _normalAgentSpeed = _controller.speed;
    }

    public void OnGroundPressedInFree(Vector3 hitPoint)
    {
        if (!IsFree || !NavMesh.SamplePosition(hitPoint, out NavMeshHit navMeshHit, 0.5f, NavMesh.AllAreas))
        {
            return;
        }
        switch (_activeAction)
        {
            case ActionType.Movement:
                GoToPosition(hitPoint);
                break;
            case ActionType.Jumping:
                if(Vector3.Distance(hitPoint, transform.position) < GameData.MaxJumpDistance)
                {
                    JumpToPosition(hitPoint);
                }
                break;
        }
        if(_activeAction != _defaultAction)
        {
            SetDefaultAction();
        }
    }

    public void OnGroundPressedInBattle(Vector3 hitPoint)
    {
        if (!IsFree || !NavMesh.SamplePosition(hitPoint, out NavMeshHit navMeshHit, 0.5f, NavMesh.AllAreas))
        { 
            return;
        }
        if(_activeAction == ActionType.Movement && BattleManager.RemainMovement > 0 && BattleManager.ActivePersonage == Personage)
        {
            if(_lastHitPoint != Vector3.positiveInfinity && _navMeshPath != null && _navMeshPath.status == NavMeshPathStatus.PathComplete &&
                Vector3.Distance(hitPoint, _lastHitPoint) <= PathEndHitAccuracy)
            {
                GoToPosition(_lastAccessablePathDot);
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

    public override void SetActiveAction(ActionType actionType)
    {
        base.SetActiveAction(actionType);
        if(actionType == ActionType.Jumping)
        {
            DisplaySphere(GameData.MaxJumpDistance);
        }
        else
        {
            HideSphere();
        }
    }

    private void DisplaySphere(float sphereSize)
    {
        Sphere.SetActive(true);
        Sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
    }

    private void HideSphere()
    {
        Sphere.SetActive(false);
    }

    public void PickupItem(Item item)
    {
        GameData.Inventory.Add(item);
        item.transform.SetParent(Inventory.transform);
        item.OnTaked();
        item.gameObject.SetActive(false);
        item.IsInInventory = true;
    }

    public override void DropItem(Item item)
    {
        base.DropItem(item);
        GameData.Inventory.Remove(item);
        item.transform.position = gameObject.transform.position + gameObject.transform.forward + Vector3.up;
        item.gameObject.SetActive(true);
        item.IsInInventory = false;
    }

    protected override void GoToPosition(Vector3 position, float maxTargetOffset = 0.1F)
    {
        base.GoToPosition(position, maxTargetOffset);
        if (Vector3.Distance(transform.position, position) > _normalAgentSpeed * 2)
        {
            _controller.speed = _normalAgentSpeed * 2;
        }
        else
        {
            _controller.speed = _normalAgentSpeed;
        }
    }

    public override void JumpToPosition(Vector3 target)
    {
        base.JumpToPosition(target);
        AnimatorManager.StartJumpAnim(target);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        GameManager.Instance.OnDeathEvent.Invoke();
    }
}