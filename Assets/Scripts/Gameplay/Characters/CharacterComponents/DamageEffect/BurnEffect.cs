using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : IDamageEffect
{
    private float burnDamage;
    private float burnDuration;

    public BurnEffect(float damage, float duration)
    {
        burnDamage = damage;
        burnDuration = duration;
    }

    public void ApplyEffect(CharacterBase currentCharacter, IDamageable target, float damage)
    {
        currentCharacter.StartCoroutine(ApplyBurn(currentCharacter, target, burnDamage, burnDuration));
    }

    private IEnumerator ApplyBurn(CharacterBase currentCharacter,IDamageable target, float damagePerSecond, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if ((MonoBehaviour)target == null)
                break;
            target.TakeDamage(damagePerSecond, currentCharacter); 
            elapsedTime += 1f;
            yield return new WaitForSeconds(1f);


        }
    }
}