using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private const string ATTACK = "Attack";
    private const string IDLE = "Idle";
    private const string RUN = "IsMoving";
    private const string DIE = "Die";
    private const string ABILITY = "Ability";
    private const string ANIMATION_DURATION = "AnimationDuration";
    private const string ATTACK_SPEED_MULTIPLIER = "AttackSpeedMultiplier";

    private float attackAnivatiomDuration;
    private float attackDuration;
    private CharacterBase unit;

    private void Start()
    {
        attackAnivatiomDuration = animator.GetFloat(ANIMATION_DURATION);
        unit = GetComponent<CharacterBase>();
        unit.OnMove += PlayMoveAnimation;
        unit.OnAttack += PlayAttackAnimation;
        unit.OnIdle += PlayIdleAnimation;
        unit.OnDie += PlayDieAnimation;
        unit.OnAbilityActivate += PlayAbilityAnimation;
    }
    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }
    private void PlayMoveAnimation()
    {
        animator.SetBool(RUN, true);
    }

    private void PlayDieAnimation()
    {
        animator.Play(DIE);
        
    }

    private void PlayAttackAnimation(IDamageable target, float duration)
    {
        if (attackDuration != duration)
        {
            animator.SetFloat(ATTACK_SPEED_MULTIPLIER, attackAnivatiomDuration / duration);
            attackDuration = duration;
        }
        animator.Play(ATTACK);
    }

    private void PlayIdleAnimation()
    {
        animator.SetBool(RUN, false);
    }
    private void PlayAbilityAnimation(int index)
    {
        animator.Play($"{ABILITY} {index + 1}");
    }
    private void OnDestroy()
    {
        unit.OnMove-= PlayMoveAnimation;
        unit.OnAttack -= PlayAttackAnimation;
        unit.OnIdle -= PlayIdleAnimation;
        unit.OnDie -= PlayDieAnimation;
        unit.OnAbilityActivate -= PlayAbilityAnimation;
    }
}
