using UnityEngine;

public class CriticalDamageEffect : IDamageEffect
{
    private float criticalMultiplier;

    public CriticalDamageEffect(float multiplier)
    {
        criticalMultiplier = multiplier;
    }

    public void ApplyEffect(CharacterBase currentCharacter, IDamageable target, float damage)
    {
        float finalDamage = damage * criticalMultiplier;
        int damageDone = target.TakeDamage(finalDamage, currentCharacter, currentCharacter.CurrentUserStats.DamageType);
        currentCharacter.Heal(damageDone * currentCharacter.CurrentUserStats.LifeStealCoefficient);
    }
}