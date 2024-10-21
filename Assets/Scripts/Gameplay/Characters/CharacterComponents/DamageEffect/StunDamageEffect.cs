using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunDamageEffect : IDamageEffect
{
    float duration;
    public StunDamageEffect(float duration)
    {
        this.duration = duration;
    }
    public void ApplyEffect(CharacterBase currentCharacter, IDamageable target, float damage)
    {
        CharacterBase targerCharacter = target as CharacterBase;
        target.TakeDamage(damage);

        if(targerCharacter != null)
        {
            targerCharacter.ChangeState(new StanState(targerCharacter, duration));
        }
    }
}
