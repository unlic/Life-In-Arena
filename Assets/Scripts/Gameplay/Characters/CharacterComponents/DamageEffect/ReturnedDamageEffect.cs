using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ReturnedDamageEffect : IDamageEffect
{
    private float reflectDamageAmount;
    private float reflectDamageMultiplier;

    public ReturnedDamageEffect(float multiplier, float damageBonus)
    {
        reflectDamageMultiplier = multiplier;
        reflectDamageAmount = damageBonus;
    }

    public void ApplyEffect(CharacterBase currentCharacter, IDamageable target, float damage)
    {
        float reflectDamage = damage * (reflectDamageMultiplier /100)+ reflectDamageAmount;

        if (reflectDamage <= 0) { return; }
        target.TakeDamage(reflectDamage, currentCharacter);
        Debug.Log($"Reflect damage! Attacker takes {reflectDamage} damage.");
    }
}
