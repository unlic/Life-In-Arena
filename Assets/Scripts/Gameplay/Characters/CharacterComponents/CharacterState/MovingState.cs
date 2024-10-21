using System;
using UnityEngine;
public class MovingState : ICharacterState
{
    private CharacterBase character;
    private Vector3 movePosition;
    private IDamageable target;
    private MonoBehaviour targetMonoBehaviour;
    private Action onMove;
    private Action<IDamageable, float> onAttack;
    private Action onIdle;
    private Action onDie;
    private Action<int> onAbilityActivate;

    private float marginError = 1f;
    public MovingState(CharacterBase character, Vector3 movePosition, IDamageable target = null)
    {
        this.character = character;
        this.movePosition = movePosition;
        this.target = target;
    }
    public void Enter()
    {
        if (target != null)
        {
            targetMonoBehaviour = ((MonoBehaviour)target);
            movePosition = targetMonoBehaviour.transform.position;
           
            character.IsMovingToPosition = false;
        }
        else
        {
            character.CurrentTarget = null;
            character.IsMovingToPosition = true;
        }

        if (movePosition != null)
        {
            character.NavMeshAgent.SetDestination(movePosition);
            character.NavMeshAgent.isStopped = false;
            onMove?.Invoke();
        }

        character.NavMeshAgent.avoidancePriority = 50;
    }

    public void Execute()
    {
        if (targetMonoBehaviour != null && targetMonoBehaviour.transform.position != movePosition)
        {
            movePosition = targetMonoBehaviour.transform.position;
            character.NavMeshAgent.SetDestination(movePosition);
        }

        if (!character.NavMeshAgent.pathPending && character.NavMeshAgent.remainingDistance <= character.CurrentUserStats.AttackDistance - marginError)
        {
            if (targetMonoBehaviour != null)
            {
                float distance = Vector3.Distance(targetMonoBehaviour.transform.position, character.transform.position);

                if (distance > character.CurrentUserStats.AttackDistance * 1.2f)
                {
                    character.NavMeshAgent.avoidancePriority = 20;
                }
                else if (character.CurrentUserStats.AttackDistance > distance)
                {
                    character.NavMeshAgent.avoidancePriority = 0; 
                    character.ChangeState(new AttackState(character, target));
                }
            }

            if (!character.NavMeshAgent.hasPath)
            {
                if (target == null)
                {
                    character.ChangeState(new HoldState(character));
                }
                else
                {
                    character.ChangeState(new AttackState(character, target));
                }
            }
        }
    }

    public void Exit()
    {
        character.IsMovingToPosition = false;
        character.NavMeshAgent.isStopped = true;
        character.NavMeshAgent.ResetPath();
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
