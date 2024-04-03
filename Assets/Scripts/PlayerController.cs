using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public delegate void Interact(GameObject gameObject, Component interactComponent);

    public float Speed;
    public float PathEndHitAccuracy; //Ќасколько близко надо нажать к концу пути
    public LineRenderer AccesableLineRenderer;
    public LineRenderer UnaccesableLineRenderer;

    public ActionType ActiveAction => _activeAction;
    public Personage Personage => _personage;
    public WeaponInfo WeaponInfo => EquipmentManager.Instance.Weapon?.ItemInfo as WeaponInfo;

    public int ArmorClass
    {
        get
        {
            var info = EquipmentManager.Instance.GetArmorInfo();
            if(info.maxArmorWeight == ArmorWeight.Heavy)
            {
                return info.ArmorClass;
            }
            else if(info.maxArmorWeight == ArmorWeight.Medium)
            {
                return info.ArmorClass + Mathf.Min(2, Personage.PersonageInfo.GetCharacteristicBonus(Characteristics.Dexterity));
            }
            else
            {
                return info.ArmorClass + Personage.PersonageInfo.GetCharacteristicBonus(Characteristics.Dexterity);
            }
        }
    }

    private NavMeshAgent _controller;
    private Rigidbody _rigidBody;
    private Personage _personage;

    private Interact _interact;
    private GameObject _interactObject;
    private Component _interactComponent;

    private bool _isGrounded = true;
    private float _startJumpTime;

    private bool _isFree => _isGrounded;

    private ActionType _defaultAction = ActionType.Movement;
    private ActionType _activeAction;
    private NavMeshPath _navMeshPath;
    private float _pathLength;
    private Vector3 _lastAccessablePathDot;
    private Vector3 _lastHitPoint;

    public void Setup()
    {
        _controller = GetComponent<NavMeshAgent>();
        _rigidBody = GetComponent<Rigidbody>();
        _personage = GetComponent<Personage>();
        _navMeshPath = new();
        _activeAction = ActionType.Movement;
    }

    private void Update()
    {
        if (_interact != null && _interactObject && _interactComponent && (!_controller.hasPath || _controller.remainingDistance <= _controller.stoppingDistance))
        {
            _interact.Invoke(_interactObject, _interactComponent);
            _interact = null;
            _interactObject = null;
            _interactComponent = null;
            _controller.ResetPath();
        }
    }

    public void OnGroundPressedInFree(Vector3 hitPoint)
    {
        if (!_isFree) return;
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
        if (!_isFree) return;
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

    float PathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return 0;

        float lengthSoFar = 0.0F;
        for (int i = 1; i < path.corners.Length; i++)
        {
            lengthSoFar += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return lengthSoFar;
    }

    Vector3 CutPath(NavMeshPath path, float maxDistance,out int lastIndex)
    {
        float lengthSoFar = 0.0F;
        float distanceToNext;
        for (int i = 1; i < path.corners.Length; i++)
        {
            distanceToNext = Vector3.Distance(path.corners[i - 1], path.corners[i]);
            if (lengthSoFar + distanceToNext > maxDistance)
            {
                lastIndex = i;
                return Vector3.MoveTowards(path.corners[i - 1], path.corners[i], maxDistance - lengthSoFar);
            }
            lengthSoFar += distanceToNext;
        }
        lastIndex = 0;
        return Vector3.zero;
    }

    private void GoToPosition(Vector3 position, float maxTargetOffset = 0.1f)
    {
        _controller.SetDestination(position);
        _controller.stoppingDistance = maxTargetOffset;
        _interact = null;
        _interactObject = null;
    }

    public void ForceStop()
    {
        _controller.ResetPath();
        _interact = null;
        _interactObject = null;
    }

    public void InteractWith(GameObject interactObject, float maxInteractDistance, Interact interact, Component interactComponent)
    {
        GoToPosition(interactObject.transform.position, maxInteractDistance);
        _interact = interact;
        _interactObject = interactObject;
        _interactComponent = interactComponent;
    }

    public void PickupItem(Item item)
    {
        GameData.Inventory.Add(item);
        item.gameObject.SetActive(false);
        item.IsInInventory = true;
    }

    public void DropItem(Item item)
    {
        GameData.Inventory.Remove(item);
        item.transform.position = gameObject.transform.position + gameObject.transform.forward;
        item.gameObject.SetActive(true);
        item.IsInInventory = false;
    }

    public void JumpToPosition(Vector3 target)
    {
        _isGrounded = false;
        _controller.enabled = false;
        _rigidBody.isKinematic = false;

        float gravity = Physics.gravity.magnitude;
        float angle = 45 * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(target.x, 0, target.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        float planarDistance = Vector3.Distance(planarTarget, planarPostion);
        float yOffset = transform.position.y - target.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(planarDistance, 2)) / (planarDistance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (planarTarget.x > planarPostion.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        _rigidBody.AddForce(finalVelocity, ForceMode.VelocityChange);
        _startJumpTime = Time.timeSinceLevelLoad;
        SetDefaultAction();
    }

    public void GetAttackInfo(Personage personage, out int bonus, out int difficulty, out Characteristics characteristic)
    {
        if (WeaponInfo)
        {
            characteristic = WeaponInfo.usingCharacteristic;
        }
        else
        {
            characteristic = Characteristics.Strength;
        }
        bonus = _personage.PersonageInfo.GetCharacteristicBonus(characteristic);
        difficulty = personage.ArmorClass; //Todo: rework
    }

    public void Attack(Personage personage)
    {
        GetAttackInfo(personage, out int bonus, out int difficulty, out Characteristics characteristic);
        CheckResult hitResult = CharacteristicChecker.Check(bonus,difficulty, out int diceResult, out int finalResult);
        if(hitResult > CheckResult.Fail)
        {
            WeaponInfo weaponInfo = WeaponInfo;
            int damage = 1;
            if (weaponInfo)
            {
                if (hitResult == CheckResult.CriticalSucces)
                {
                    damage = weaponInfo.MaxDamage;
                }
                else
                {
                    damage = Random.Range(weaponInfo.MinDamage, weaponInfo.MaxDamage);
                }
            }
            else
            {
                damage = Random.Range(1, 4);
            }

            if(Personage.PersonageInfo.Race == Race.Orc)
            {
                if (hitResult == CheckResult.CriticalSucces)
                {
                    damage += 4;
                }
                else
                {
                    damage = Random.Range(1,4);
                }
            }
            personage.GetDamage(damage, DamageType.Physical);
        }
        SetDefaultAction();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isGrounded && 
            (Time.timeSinceLevelLoad - _startJumpTime) > Time.fixedDeltaTime
            && collision.GetContact(0).point.y < transform.position.y)
        {
            _isGrounded = true;
            _controller.enabled = true;
            _rigidBody.isKinematic = true;
        }
    }

    public void SetDefaultAction()
    {
        SetActiveAction(_defaultAction);
        CanvasManager.Instance.ForceChangeAction(_defaultAction);
    }

    public void SetActiveAction(ActionType actionType)
    {
        _activeAction = actionType;
    }
}