using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class StanState : ICharacterState
{
    private CharacterBase character;
    private float stunDuration;
    private float stunStartTime;
    private Action onIdle;
    private Action<float> onStaned;
    public bool IsStunned;
    public StanState(CharacterBase character, float stunDuration)
    {
        this.character = character;
        this.stunDuration = stunDuration;
        IsStunned = true;
    }


    public void Enter()
    {
        character.NavMeshAgent.isStopped = true;
        character.NavMeshAgent.avoidancePriority = 0;
        character.IsCastingAbility = false;
        stunStartTime = Time.time;
        onIdle?.Invoke();
        onStaned?.Invoke(stunDuration);
        IsStunned = true;
        Debug.Log("Enter StanState");
    }

    public void Execute()
    {
        if (Time.time - stunStartTime >= stunDuration)
        {
            IsStunned = false;
            character.ChangeState(new HoldState(character));
        }
    }

    public void Exit()
    {
        character.NavMeshAgent.isStopped = false;
    }

    public void SetEvents(Action onMove, Action<IDamageable, float> onAttack, Action onIdle, Action onDie, Action<int> onAbilityActivate, Action<float> onStaned)
    {
        this.onIdle = onIdle;
        this.onStaned = onStaned;
    }
}
