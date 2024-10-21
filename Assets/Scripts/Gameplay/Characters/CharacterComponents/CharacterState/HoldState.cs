using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldState : ICharacterState
{
    private CharacterBase character;
    private Action onMove;
    private Action<IDamageable, float> onAttack;
    private Action onIdle;
    private Action onDie;
    private Action<int> onAbilityActivate;
    public HoldState(CharacterBase character)
    {
        this.character = character;
    }
    public void Enter()
    {
        character.NavMeshAgent.isStopped = true;
        character.IsMovingToPosition = false;
        onIdle?.Invoke();
        character.NavMeshAgent.avoidancePriority = 0;

        if (character.CharacterState != CharacterState.Hold)
            character.ChangeState(new DefenceState(character));
    }
    public void Execute()
    {

        if (character.IsMovingToPosition) return;

        List<CharacterBase> enemiesInRange = CharacterManager.Instance.GetEnemiesInRange(character, character.CurrentUserStats.AttackDistance);

        if (enemiesInRange.Count == 0)
            return;

        IDamageable target = character.TargetManager.NewTarget(enemiesInRange);

        if (character.IsWithinAttackRange(target))
        {
            character.ChangeState(new AttackState(character, target));
        }
    }

    public void Exit()
    {

    }

    public void SetEvents(Action onMove, Action<IDamageable, float> onAttack, Action onIdle, Action onDie, Action<int> onAbilityActivate,Action<float> OnStaned)
    {
        this.onMove = onMove;
        this.onAttack = onAttack;
        this.onIdle = onIdle;
        this.onDie = onDie;
        this.onAbilityActivate = onAbilityActivate;
    }
}
