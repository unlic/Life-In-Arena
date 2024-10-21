using System;
using UnityEngine;

public class AttackState : ICharacterState
{
    private CharacterBase character;
    private IDamageable target;
    private Action onMove;
    private Action<IDamageable, float> onAttack;
    private Action onIdle;
    private Action onDie;
    private Action<int> onAbilityActivate;
    public AttackState(CharacterBase character, IDamageable target)
    {
        this.character = character;
        this.target = target;
    }
    public void Enter()
    {
        character.NavMeshAgent.isStopped = true;
        character.IsMovingToPosition = false;
        onIdle?.Invoke();
        character.NavMeshAgent.avoidancePriority = 0;
    }

    public void Execute()
    {
        if ((MonoBehaviour)target == null || character.IsDie)
        {
            target = null;
            character.ChangeState(new HoldState(character));
            return;
        }

        float distance = Vector3.Distance(character.transform.position, ((MonoBehaviour)target).transform.position);

        if (distance <= character.CurrentUserStats.AttackDistance)
        {
            character.Attack(target);
        }
        else
        {
            character.ChangeState(new MovingState(character, Vector3.zero, target));
        }
    }

    public void Exit()
    {
        character.CurrentTarget = null;
    }

    public void SetEvents(Action onMove, Action<IDamageable, float> onAttack, Action onIdle, Action onDie, Action<int> onAbilityActivate, Action<float> OnStaned)
    {
        this.onMove = onMove;
        this.onAttack = onAttack;
        this.onIdle = onIdle;
        this.onDie = onDie;
        this.onAbilityActivate = onAbilityActivate;
    }
}
