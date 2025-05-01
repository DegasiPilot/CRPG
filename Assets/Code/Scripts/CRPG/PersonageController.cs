using CRPG;
using CRPG.Battle;
using CRPG.DataSaveSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class PersonageController : MonoBehaviour
{
	public delegate void Interact(Component interactComponent);

	public float Speed;

	public ActionType ActiveAction => _activeAction;
	public Personage Personage => _personage;
	public float MaxAttackDistance => _personage.EquipmentManager.MaxAttackDistance;
	public bool IsFree => _isGrounded && !IsAttacking &&
		(GameManager.Instance.GameMode != GameMode.Battle ||
		(!_controller.pathPending && !IsMoving));

	public bool IsAttacking => _attackModule.IsAttacking;
	public float Radius => _controller.radius;
	[field: SerializeField] public AnimatorManager AnimatorManager { get; protected set; }

	[SerializeField] protected NavMeshAgent _controller;
	[SerializeField] protected Rigidbody _rigidBody;
	[SerializeField] protected Collider _collider;
	[SerializeField] protected Personage _personage;
	[SerializeField] private protected ChooseAttackForceModule _chooseAttackForceModule;
	private AttackModule _attackModule;

	protected Interact _interact;
	protected Component _interactComponent;

	[Header("Настройки прыжка")]
	[SerializeField] protected float _distanceToHeightRatio = 0.4f; // Базовый множитель высоты
	[SerializeField] protected float _minJumpHeight = 0.1f;
	[SerializeField] protected float _maxJumpHeight = 1.5f;

	protected bool _isGrounded = true;

	protected ActionType _defaultAction = ActionType.Movement;
	protected ActionType _activeAction;
	protected NavMeshPath _navMeshPath;
	protected float _pathLength;
	protected Vector3 _lastAccessablePathDot;
	protected Vector3 _lastHitPoint;
	public bool IsMoving { get; private set; }

	public void Awake()
	{
		Setup();
	}

	protected virtual void OnValidate()
	{
		if (_controller == null) _controller = GetComponent<NavMeshAgent>();
		if (_rigidBody == null) _rigidBody = GetComponent<Rigidbody>();
		if (_collider == null) _collider = GetComponent<Collider>();
		if (_personage == null) _personage = GetComponent<Personage>();
		if (AnimatorManager == null) AnimatorManager = GetComponentInChildren<AnimatorManager>();
	}

	protected virtual void Setup()
	{
		_controller.updatePosition = false;
		_personage.OnDeath.AddListener(OnDeath);
		_navMeshPath = new();
		_activeAction = ActionType.Movement;
		_attackModule = new AttackModule(this, _chooseAttackForceModule);
	}

	protected virtual void Update()
	{
		if (_controller.enabled)
		{
			if (_controller.hasPath && Vector3.Distance(_controller.destination, transform.position) >= _controller.stoppingDistance)
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

	public virtual IEnumerator JumpToPosition(Vector3 targetPosition, float jumpHeigth, float jumpDuration)
	{
		_interact = null;
		_interactComponent = null;
		_isGrounded = false;
		_controller.enabled = false; // Выключаем NavMeshAgent
		SetDefaultAction();

		Vector3 startPos = transform.position;
		float time = 0;

		while (time < jumpDuration)
		{
			time += Time.deltaTime;
			float progress = time / jumpDuration;

			transform.position = PhysicHelper.JumpToPosition(startPos, targetPosition, jumpHeigth, progress);

			yield return null;
		}

		// Фиксируем позицию после прыжка
		transform.position = targetPosition;
		_isGrounded = true;
		_controller.enabled = true;
	}

	public void StartAttack(PersonageController personageController)
	{
		_attackModule.Attack(personageController);
	}

	public virtual void EndAttack()
	{
		
	}

	public IEnumerator RotateTo(Transform target)
	{
		Quaternion startRot = transform.rotation;
		Quaternion finalRot = Quaternion.LookRotation(target.position - Personage.HitPoint.position);
		if (_personage.EquipmentManager.Weapon != null && _personage.EquipmentManager.Weapon.TargetingOffset != Vector3.zero)
		{
			finalRot *= Quaternion.Euler(_personage.EquipmentManager.Weapon.TargetingOffset);
		}
		float angle = Quaternion.Angle(startRot, finalRot);
		float t = 0;
		do
		{
			t += 180 * Time.deltaTime / angle;
			transform.rotation = Quaternion.Slerp(startRot, finalRot, t);
			yield return null;
		} while (t < 1);
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

	public void GetDamage(float damage, DamageType damageType)
	{
		AnimatorManager.StartGetDamageAnim();
		_personage.GetDamage(damage, damageType);
	}

	protected virtual void OnDeath()
	{
		foreach (var item in _personage.EquipmentManager.EquipableItems)
		{
			item.IsEquiped = false;
			DropItem(item);
		}
		AnimatorManager.StartDeathAnim();
	}

	public virtual void DropItem(Item item)
	{
		item.transform.SetParent(null);
		item.OnDropped();
	}

	private void OnDestroy()
	{
		_attackModule.Dispose();
	}
}