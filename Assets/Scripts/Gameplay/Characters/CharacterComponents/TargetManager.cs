using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public CharacterBase CurrentCharacter;
    public IDamageable CurrentTarget;

    public IDamageable NewTarget(List<CharacterBase> attackers)
    {
        CharacterBase closestUnit = null;
        float closestDistance = float.MaxValue;

        foreach (var attacker in attackers)
        {
            if (attacker == null || attacker == this) continue;

            if (!attacker.IsDie)
            {
                float distance = Vector3.Distance(transform.position, attacker.transform.position);

                if (distance < closestDistance)
                {
                    closestUnit = attacker;
                    closestDistance = distance;
                }
            }
        }
        return closestUnit;
    }

    public void TargetSearchProcessing(IDamageable attacker, List<CharacterBase> attackers)
    {
        if (CurrentCharacter.IsMovingToPosition||CurrentCharacter.IsMovingToEnemy)
        {
            CurrentTarget = null;
            return;
        }
        if ((MonoBehaviour)CurrentTarget == null)
        {
            CurrentTarget = NewTarget(attackers);
            CurrentCharacter.ChangeState(new MovingState(CurrentCharacter, Vector3.zero, CurrentTarget));
            if((MonoBehaviour)CurrentTarget == null)
            {
                return;
            }
        }
        float distanceToCurrentTarget = Vector3.Distance(transform.position, ((MonoBehaviour)CurrentTarget).transform.position);

        if (distanceToCurrentTarget <= CurrentCharacter.CurrentUserStats.AttackDistance)
        {
            CurrentCharacter.ChangeState(new AttackState(CurrentCharacter, CurrentTarget));
            return;
        }

        if((MonoBehaviour)attacker == null)
        {
            CurrentCharacter.ChangeState(new AttackState(CurrentCharacter, CurrentTarget));
            return;
        }

        float distanceToAttacker = Vector3.Distance(transform.position, ((MonoBehaviour)attacker).transform.position);
        if (distanceToAttacker < distanceToCurrentTarget && distanceToCurrentTarget - distanceToAttacker > 1.0f)
        {
            CurrentTarget = attacker;
            if (distanceToAttacker > CurrentCharacter.CurrentUserStats.AttackDistance)
            {
                CurrentCharacter.ChangeState(new MovingState(CurrentCharacter, Vector3.zero, CurrentTarget));
            }
            else
            {
                CurrentCharacter.ChangeState(new AttackState(CurrentCharacter, CurrentTarget));
            }
        }
       
    }
}
