using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class PersonageController : MonoBehaviour
{
    public delegate void Interact(Component interactComponent);

    public float Speed;
    public float AttackAnimTimeBeforeHit;

    public ActionType ActiveAction => _activeAction;
    public Personage Personage => _personage;
    public float MaxAttackDistance => _personage.WeaponInfo != null ? _personage.WeaponInfo.MaxAttackDistance : GameData.MaxUnarmedAttackDistance;
    public bool IsFree => _isGrounded && !IsAttacking && (GameManager.Instance.GameMode != GameMode.Battle || !_controller.pathPending || !IsMoving);

    public bool IsAttacking { get; private set; }
    public float Radius => _controller.radius;
    public AnimatorManager AnimatorManager { get; protected set; }

    private WeaponInfo WeaponInfo => _personage.WeaponInfo;

    protected NavMeshAgent _controller;
    protected Rigidbody _rigidBody;
    protected Collider _collider;
    protected Personage _personage;

    protected Interact _interact;
    protected Component _interactComponent;

    protected bool _isGrounded = true;
    protected float _startJumpTime;

    protected ActionType _defaultAction = ActionType.Movement;
    protected ActionType _activeAction;
    protected NavMeshPath _navMeshPath;
    protected float _pathLength;
    protected Vector3 _lastAccessablePathDot;
    protected Vector3 _lastHitPoint;
    public bool IsMoving { get; private set; }
    protected bool _isAnimPlaying;

    public virtual void Setup()
    {
        _controller = GetComponent<NavMeshAgent>();
        _controller.updatePosition = false;
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _personage = GetComponent<Personage>();
        AnimatorManager = GetComponentInChildren<AnimatorManager>();
        _navMeshPath = new();
        _activeAction = ActionType.Movement;
        _personage.OnDeath.AddListener(OnDeath);
    }

    protected virtual void Update()
    {
        if (_controller.enabled)
        {
            if(_controller.hasPath && _controller.remainingDistance > _controller.stoppingDistance)
            {
                IsMoving = true;
            }
            else
            {
                if (_interact != null && _interactComponent != null)
                {
                    _controller.velocity = Vector3.zero;
                    _interact.Invoke(_interactComponent);
                    _interact = null;
                    _interactComponent = null;
                }
                if (_controller.hasPath) _controller.ResetPath();
                IsMoving = false;
            }
        }
    }

    protected float PathLength(NavMeshPath path)
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

    protected Vector3 CutPath(NavMeshPath path, float maxDistance, out int lastIndex)
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
            else if (i == path.corners.Length - 1)
            {
                lastIndex = i;
                return path.corners[i];
            }
            lengthSoFar += distanceToNext;
        }
        lastIndex = path.corners.Length - 1;
        return Vector3.zero;
    }

    protected virtual void GoToPosition(Vector3 position, float maxTargetOffset = 0.1f)
    {
        _controller.stoppingDistance = maxTargetOffset;
        NavMeshPath path = new();
        NavMesh.SamplePosition(position, out NavMeshHit hit, 2f, NavMesh.AllAreas);
        _controller.CalculatePath(hit.position, path);
        position = Vector3.MoveTowards(position, path.corners[path.corners.Length - 2], _controller.radius);
        _controller.SetDestination(position);
        _interact = null;
    }

    public void ForceStop()
    {
        _controller.ResetPath();
        _interact = null;
    }

    public void InteractWith(float maxInteractDistance, Interact interact, Component interactComponent)
    {
        GoToPosition(interactComponent.transform.position, maxInteractDistance);
        _interact = interact;
        _interactComponent = interactComponent;
    }

    public virtual void JumpToPosition(Vector3 target)
    {
        _interact = null;
        _interactComponent = null;

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

        _startJumpTime = Time.timeSinceLevelLoad;
        _isGrounded = false;
        _controller.enabled = false;
        _rigidBody.isKinematic = false;
        _rigidBody.AddForce(finalVelocity, ForceMode.VelocityChange);
        SetDefaultAction();
    }

    protected void OnCollisionEnter(Collision collision)
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

    public IEnumerator Attack(PersonageController personageController)
    {
        IsAttacking = true;
        GetAttackInfo(personageController.Personage, out int bonus, out int difficulty, out Characteristics characteristic);
        CheckResult hitResult = CharacteristicChecker.Check(bonus, difficulty, out int diceResult, out int finalResult);
        _isAnimPlaying = true;
        while (_isAnimPlaying)
        {
            yield return AttackAnim(personageController.transform);
        }
        if (hitResult > CheckResult.Fail)
        {
            WeaponInfo weaponInfo = WeaponInfo;
            int damage;
            if (weaponInfo)
            {
                if (hitResult == CheckResult.CriticalSucces)
                {
                    damage = weaponInfo.MaxDamage;
                    MessageBoxManager.ShowMessage("Критическое попадание!");
                }
                else
                {
                    damage = Random.Range(weaponInfo.MinDamage, weaponInfo.MaxDamage);
                    MessageBoxManager.ShowMessage("Попадание");
                }
            }
            else
            {
                if (hitResult == CheckResult.CriticalSucces)
                {
                    damage = 4;
                    MessageBoxManager.ShowMessage("Критическое попадание!");
                }
                else
                {
                    damage = Random.Range(1, 4);
                    MessageBoxManager.ShowMessage("Попадание");
                }
            }
            if (Personage.PersonageInfo.Race == Race.Orc)
            {
                if (hitResult == CheckResult.CriticalSucces)
                {
                    damage += 4;
                }
                else
                {
                    damage = Random.Range(1, 4);
                }
            }
            personageController.GetDamage(damage, DamageType.Physical);
        }
        else
        {
            MessageBoxManager.ShowMessage("Уворот/Блок");
        }
        SetDefaultAction();
        IsAttacking = false;

        IEnumerator AttackAnim(Transform target)
        {
            Quaternion startRot = transform.rotation;
            Quaternion finalRot = Quaternion.LookRotation(target.position - transform.position);
            float angle = Quaternion.Angle(startRot, finalRot);
            float t = 0;
            do
            {
                t += 180 * Time.deltaTime / angle;
                transform.rotation = Quaternion.Slerp(startRot, finalRot, t);
                yield return null;
            } while (t < 1);
            AnimatorManager.StartAttackAnim(WeaponInfo != null);
            yield return new WaitForSeconds(AttackAnimTimeBeforeHit);
            _isAnimPlaying = false;
        }
    }

    public virtual void SetDefaultAction()
    {
        SetActiveAction(_defaultAction);
    }

    public virtual void SetActiveAction(ActionType actionType)
    {
        _activeAction = actionType;
    }

    public void SetPositonAndRotation(Vector3 pos, Vector3 rot)
    {
        if (_controller.enabled)
        {
            _controller.enabled = false;
            transform.position = pos;
            transform.eulerAngles = rot;
            _controller.enabled = true;
        }
        else
        {
            transform.position = pos;
            transform.eulerAngles = rot;
        }
    }

    public void GetDamage(int damage, DamageType damageType)
    {
        AnimatorManager.StartGetDamageAnim();
        _personage.GetDamage(damage, damageType);
    }

    private void OnDeath()
    {
        AnimatorManager.StartDeathAnim();
    }
}