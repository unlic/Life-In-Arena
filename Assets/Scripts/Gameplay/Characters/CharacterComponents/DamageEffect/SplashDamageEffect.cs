using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashDamageEffect : IDamageEffect
{
    private float splashRadius;
    private float splashDamageAmount;

    public SplashDamageEffect(float radius, float splashDamage)
    {
        splashRadius = radius;
        splashDamageAmount = splashDamage;
    }

    public void ApplyEffect(CharacterBase currentCharacter, IDamageable target, float damage)
    {
        if(target is not ISplashDamageable || splashDamageAmount == 0 || (target as CharacterBase).IsDie)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(((MonoBehaviour)target).transform.position, splashRadius);

        foreach (var hitCollider in hitColliders)
        {
            CharacterBase splashTargetCharacter = hitCollider.GetComponent<ISplashDamageable>() as CharacterBase;

            if (splashTargetCharacter != null && splashTargetCharacter != ((MonoBehaviour)target) && splashTargetCharacter.TeamId != currentCharacter.TeamId)
            {
                splashTargetCharacter.TakeSplashDamage(splashDamageAmount, currentCharacter); 
                Debug.Log($"Splash damage! Dealt {splashDamageAmount} to {splashTargetCharacter.name}");
            }
        }
    }
}
