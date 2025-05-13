using CRPG.ItemSystem;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimatorManager : MonoBehaviour
{
	[SerializeField] private float _attackAnimTimeBeforeHit;
	public float AttackAnimTimeBeforeHit => _attackAnimTimeBeforeHit;

	[SerializeField] private UnityEvent _onContactEnemy;
	public UnityEvent OnContactEnemy => _onContactEnemy;

	[SerializeField] private Animator _animator;
	[SerializeField] private ItemSkin _rightHand;

	private readonly int _atackAnimId = Animator.StringToHash("Attack");
	private readonly int _hitAnimId = Animator.StringToHash("Hit");
	private readonly int _fallAnimId = Animator.StringToHash("Fall");
	private readonly int _animSpeedVId = Animator.StringToHash("speedv");
	private readonly int _animSpeedHId = Animator.StringToHash("speedh");
	private readonly int _animIsArmedId = Animator.StringToHash("IsArmed");
	private readonly int _animIsMeleeId = Animator.StringToHash("IsMelee");
	private readonly int _animIsMovingId = Animator.StringToHash("IsMoving");
	private readonly int _animIsJumpingId = Animator.StringToHash("IsJumping");
	private readonly int _animJumpSpeedMultiplier = Animator.StringToHash("JumpSpeedMultiplier");
	private readonly int _animIsTalkingId = Animator.StringToHash("IsTalking");

	[SerializeField] private float _jumpGroundingTime;

	private WeaponAnimationManager _weaponAnimationManager;

	private void OnValidate()
	{
		if (_animator == null) TryGetComponent(out _animator);
	}

	private void Awake()
	{
		_animator.applyRootMotion = false;
		_animator.enabled = true;
	}

	public void SetVelocity(float velocityV)
	{
		_animator.SetFloat(_animSpeedVId, velocityV);
	}

	public void SetVelocity(Vector2 velocity)
	{
		_animator.SetFloat(_animSpeedVId, velocity.y);
		_animator.SetFloat(_animSpeedHId, velocity.x);
		_animator.SetBool(_animIsMovingId, Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y) > 0);
	}

	internal void StartAttackAnim(Transform target, bool IsWeaponed, bool IsMelee, WeaponAnimationManager weaponAnimationManager)
	{
		_animator.SetBool(_animIsArmedId, IsWeaponed);
		_animator.SetBool(_animIsMeleeId, IsMelee);
		_animator.SetTrigger(_atackAnimId);
		if (weaponAnimationManager != null)
		{
			_weaponAnimationManager = weaponAnimationManager;
			_weaponAnimationManager.Target = target;
			_weaponAnimationManager.RightHand = _rightHand;
		}
	}

	public void StartGetDamageAnim()
	{
		_animator.SetTrigger(_hitAnimId);
	}

	public void StartDeathAnim()
	{
		_animator.SetTrigger(_fallAnimId);
	}

	public void StartJumpAnim(Vector3 target, float animDuration)
	{
		_animator.SetFloat(_animJumpSpeedMultiplier, 1 / (animDuration / _jumpGroundingTime));
		_animator.SetTrigger(_animIsJumpingId);
	}

	public void StartDialogueAnim()
	{
		_animator.SetBool(_animIsTalkingId, true);
	}

	public void EndDialogueAnim()
	{
		_animator.SetBool(_animIsTalkingId, false);
	}


	/// <summary>
	/// Invoke by animator
	/// </summary>
	public void OnArrowSpawn()
	{
		if (_weaponAnimationManager != null)
		{
			_weaponAnimationManager.SpawnArrow();
		}
	}

	public void OnGetArrow()
	{
		if (_weaponAnimationManager != null)
		{
			_weaponAnimationManager.AttackAnim();
		}
	}

	public void OnThrowArrow()
	{
		if (_weaponAnimationManager != null)
		{
			_weaponAnimationManager.EndAttackAnim(OnWeaponContact);
			_weaponAnimationManager = null;
		}
	}

	/// <summary>
	/// Invoke by animation events
	/// </summary>
	public void OnWeaponContact()
	{
		OnContactEnemy.Invoke();
	}
}