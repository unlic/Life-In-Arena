using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageEffect
{
    public void ApplyEffect(CharacterBase currentCharacter, IDamageable target, float damage);
}
