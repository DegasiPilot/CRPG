using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class PersonageController : MonoBehaviour
{
    public delegate void Interact(GameObject gameObject, Component interactComponent);

    public float Speed;

    public ActionType ActiveAction => _activeAction;
    public Personage Personage => _personage;
    public WeaponInfo WeaponInfo => _personage.Weapon?.ItemInfo as WeaponInfo;
    public bool IsFree => _isGrounded && (GameManager.Instance.GameMode != GameMode.Battle || !_controller.hasPath);

    public int ArmorClass
    {
        get
        {
            var ArmorInfo = from armor in _personage.Armor select armor.ItemInfo as ArmorInfo;
            ArmorWeight maxArmorWeight = ArmorInfo.Max(x => x.ArmorWeight);
            int armorClass = ArmorInfo.Sum(x => x.ArmorClass);
            if (maxArmorWeight == ArmorWeight.Heavy)
            {
                return armorClass;
            }
            else if (maxArmorWeight == ArmorWeight.Medium)
            {
                return armorClass + Mathf.Min(2, Personage.PersonageInfo.GetCharacteristicBonus(Characteristics.Dexterity));
            }
            else
            {
                return armorClass + Personage.PersonageInfo.GetCharacteristicBonus(Characteristics.Dexterity);
            }
        }
    }

    protected NavMeshAgent _controller;
    protected Rigidbody _rigidBody;
    protected Personage _personage;

    protected Interact _interact;
    protected GameObject _interactObject;
    protected Component _interactComponent;

    protected bool _isGrounded = true;
    protected float _startJumpTime;

    protected ActionType _defaultAction = ActionType.Movement;
    protected ActionType _activeAction;
    protected NavMeshPath _navMeshPath;
    protected float _pathLength;
    protected Vector3 _lastAccessablePathDot;
    protected Vector3 _lastHitPoint;

    public virtual void Setup()
    {
        _controller = GetComponent<NavMeshAgent>();
        _rigidBody = GetComponent<Rigidbody>();
        _personage = GetComponent<Personage>();
        _navMeshPath = new();
        _activeAction = ActionType.Movement;
    }

    protected void Update()
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
            lengthSoFar += distanceToNext;
        }
        lastIndex = 0;
        return Vector3.zero;
    }

    protected void GoToPosition(Vector3 position, float maxTargetOffset = 0.1f)
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
        CheckResult hitResult = CharacteristicChecker.Check(bonus, difficulty, out int diceResult, out int finalResult);
        if (hitResult > CheckResult.Fail)
        {
            WeaponInfo weaponInfo = WeaponInfo;
            int damage;
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
            personage.GetDamage(damage, DamageType.Physical);
        }
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

    public virtual void SetDefaultAction()
    {
        SetActiveAction(_defaultAction);
    }

    public void SetActiveAction(ActionType actionType)
    {
        _activeAction = actionType;
    }
}