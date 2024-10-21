using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DefenceState : ICharacterState
{
    private CharacterBase character;
    private float range;
    private Action onMove;
    private Action<IDamageable, float> onAttack;
    private Action onIdle;
    private Action onDie;
    private Action<int> onAbilityActivate;
    public DefenceState(CharacterBase character)
    {
        this.character = character;
        range = Mathf.Max(4, character.CurrentUserStats.AttackDistance);
    }

    public void Enter()
    {
        Debug.Log("Entering Defence State");
        character.NavMeshAgent.isStopped = true;
        onIdle?.Invoke();
    }
    public void Execute()
    {
        if (character.IsMovingToPosition) return;

        List<CharacterBase> enemiesInRange = CharacterManager.Instance.GetEnemiesInRange(character, range);

        if(enemiesInRange.Count==0) return;

        IDamageable target = character.TargetManager.NewTarget(enemiesInRange);

        if (character.IsWithinAttackRange(target))
        {
            character.ChangeState(new AttackState(character, target));
        }
        else
        {
            character.ChangeState(new MovingState(character, Vector3.zero, target));
        }
    }

    public void Exit()
    {
        Debug.Log("Exit Idle State");
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
