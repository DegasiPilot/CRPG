using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimatorManager : MonoBehaviour
{
    [System.NonSerialized] public UnityEvent OnAttackContactEvent = new();
    public Animator Animator;
    public float JumpSpeedMultiplier;

    private readonly int _atackAnimId = Animator.StringToHash("Attack");
    private readonly int _hitAnimId = Animator.StringToHash("Hit");
    private readonly int _fallAnimId = Animator.StringToHash("Fall");
    private readonly int _animSpeedVId = Animator.StringToHash("speedv");
    private readonly int _animSpeedHId = Animator.StringToHash("speedh");
    private readonly int _animIsArmedId = Animator.StringToHash("IsArmed");
    private readonly int _animIsMovingId = Animator.StringToHash("IsMoving");
    private readonly int _animIsJumpingId = Animator.StringToHash("IsJumping");
    private readonly int _animJumpSpeedMultiplier = Animator.StringToHash("JumpSpeedMultiplier");
    private readonly int _animIsTalkingId = Animator.StringToHash("IsTalking");

    private void Awake()
    {
        Animator.applyRootMotion = false;
        Animator.enabled = true;
    }

    public void AttackContactEvent()
    {
        OnAttackContactEvent.Invoke();
    }

    public void SetVelocity(float velocityV)
    {
        Animator.SetFloat(_animSpeedVId, velocityV);
    }

    public void SetVelocity(Vector2 velocity)
    {
        Debug.Log("Velocity is" + velocity);
        Animator.SetFloat(_animSpeedVId, velocity.y);
        Animator.SetFloat(_animSpeedHId, velocity.x);
        Animator.SetBool(_animIsMovingId, Mathf.Abs(velocity.x + velocity.y) > 0);
    }

    public void StartAttackAnim(bool IsWeaponed)
    {
        Animator.SetBool(_animIsArmedId, IsWeaponed);
        Animator.SetTrigger(_atackAnimId);
    }

    public void StartGetDamageAnim()
    {
        Animator.SetTrigger(_hitAnimId);
    }

    public void StartDeathAnim()
    {
        Animator.SetTrigger(_fallAnimId);
    }

    public void StartJumpAnim(Vector3 target)
    {
        Animator.SetTrigger(_animIsJumpingId);
        Animator.SetFloat(_animJumpSpeedMultiplier, Vector3.Distance(target, transform.position) * JumpSpeedMultiplier);
    }

    public void StartDialogueAnim()
    {
        Animator.SetBool(_animIsTalkingId, true);
    }

    public void EndDialogueAnim()
    {
        Animator.SetBool(_animIsTalkingId, false);
    }
}