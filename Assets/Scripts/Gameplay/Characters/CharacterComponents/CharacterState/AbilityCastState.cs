using System;
using UnityEngine;

public class AbilityCastState : ICharacterState
{
    private CharacterBase character;
    private Ability ability;
    private CharacterBase target;
    private Vector3? area;
    private float castStartTime;
    private float castDuration;
    private float range;
    private Vector3 targetPosition;
    private bool isCastingStarted;
    private Action onMove;
    private Action onIdle;
    private Action<int> onAbilityActivate;

    private int indexUnitAbility = 0;

    public AbilityCastState(CharacterBase character, Ability ability, CharacterBase target = null, Vector3? area = null)
    {
        this.character = character;
        this.ability = ability;
        this.target = target;
        this.area = area;
        this.isCastingStarted = false;
        indexUnitAbility = character.Abilities.IndexOf(ability);
    }

    public void Enter()
    {
        character.NavMeshAgent.isStopped = true;
        character.IsCastingAbility = true;
        castDuration = ability.CastDuration;
        range = ability.Range;
        character.SetHasReceivedNewCommand(false);
        targetPosition = target != null ? target.transform.position : area.GetValueOrDefault();
    }

    public void Execute()
    {
        if (Vector3.Distance(character.transform.position, targetPosition) <= range)
        {
            
            if (!isCastingStarted)
            {
                ability.UseAbility(character, target, area);
                castStartTime = Time.time; 
                isCastingStarted = true;
                onIdle?.Invoke();
                character.NavMeshAgent.isStopped = true;
            }
            if (Time.time - castStartTime >= castDuration)
            {
                Debug.Log("Каст завершен.");
                character.IsCastingAbility = false;
                onAbilityActivate?.Invoke(indexUnitAbility);
                character.ChangeState(new HoldState(character));
                return;
            }
        }
        else
        {
            onMove?.Invoke();
            character.NavMeshAgent.SetDestination(targetPosition);
            character.NavMeshAgent.isStopped = false;
        }

        if (character.HasReceivedNewCommand()&&!isCastingStarted)
        {
            Debug.Log("Способность отменена.");
            character.IsCastingAbility = false;
            character.ChangeState(new HoldState(character));
        }
    }

    public void Exit()
    {
        character.NavMeshAgent.isStopped = false;
        character.IsCastingAbility = false;
    }

    public void SetEvents(Action onMove, Action<IDamageable, float> onAttack, Action onIdle, Action onDie, Action<int> onAbilityActivate, Action<float> OnStaned)
    {
        this.onMove = onMove;
        this.onIdle = onIdle;
        this.onAbilityActivate = onAbilityActivate;
    }
}
