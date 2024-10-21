using System;
using System.Collections;
using System.IO;
using UnityEngine;
using static UnityEditor.Progress;

public class PassiveStunAbility : Ability
{
    
    [SerializeField] private float baseStunDuration;
    [SerializeField] private float baseStunChance;    
    [SerializeField] private float stunPerLevelDuration;
    [SerializeField] private float stunPerLevelChance;
    private float currentStunDuration;
    private float currentStunChance;
    private string abilityDescription;
    private string[] abilityDescriptionByLevel;

    private IDamageEffect damageEffect;

    protected override void Activate(CharacterBase user, CharacterBase target = null, Vector3? area = null)
    {
        UpdateAbilityStats();
        CurrentUser = user;
        CurrentUser.OnAttack += ApplyDamage;
    }

    public override void Deactivate()
    {
        if (CurrentUser != null)
        {
            CurrentUser.OnAttack -= ApplyDamage;
        }
    }

    private void ApplyDamage(IDamageable target, float damage)
    {
        float rand = UnityEngine.Random.Range(0f, 100f);
        if (rand < currentStunChance)
        {
            CurrentUser.StartCoroutine(WaitToDamage(target));
        }
    }
    private IEnumerator WaitToDamage(IDamageable target)
    {
        yield return new WaitForSeconds(CurrentUser.CurrentUserStats.CalculateAttackSpeed() / 4);

        damageEffect = new StunDamageEffect(currentStunDuration);
        damageEffect.ApplyEffect(CurrentUser, target, 0);
    }
    public override string GetAbilityDescription()
    {
        abilityDescription = "\n";
        abilityDescriptionByLevel = new string[MaxLevel];

        for (int i = 0; i < MaxLevel; i++)
        {
            abilityDescriptionByLevel[i] = $"Level {i + 1}: Stun for {(baseStunDuration + i * stunPerLevelDuration)} seconds with a {baseStunChance + i * stunPerLevelChance}% chance.";
            abilityDescription += abilityDescriptionByLevel[i] + "\n";
        }

        return abilityDescription;
    }
    protected override void UpdateAbilityStats()
    {
        currentStunDuration = baseStunDuration + (Level - 1) * stunPerLevelDuration;
        currentStunChance = baseStunChance + (Level - 1) * stunPerLevelChance;
    }
}