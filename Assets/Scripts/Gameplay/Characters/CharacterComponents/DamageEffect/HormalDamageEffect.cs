using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HormalDamageEffect : IDamageEffect
{
    public void ApplyEffect(CharacterBase currentCharacter, IDamageable target, float damage)
    {
        int damageDone = target.TakeDamage(damage, currentCharacter, currentCharacter.CurrentUserStats.DamageType);
        currentCharacter.Heal(damageDone * currentCharacter.CurrentUserStats.LifeStealCoefficient);
    }
}
