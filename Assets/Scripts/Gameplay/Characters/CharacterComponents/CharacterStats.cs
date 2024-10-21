using System;
using UnityEditor;
using UnityEngine;
public class CharacterStats
{
    public CharacterStats(CharacterSettings settings, float healthMultiplier, float manaMultiplier, float defenseMultiplier)
    {
        Strength = settings.Strength;
        Agility = settings.Agility;
        Intelligence = settings.Intelligence;
        DefaultHealth = settings.DefaultHealth - Strength * healthMultiplier;
        DefaultMana = settings.DefaultMana - Intelligence * manaMultiplier;
        DefaultDefense = settings.DefaultDefense - Agility + AgilityBonus * defenseMultiplier;
        BaseAttackSpeed = settings.DefaultAttackSpeed;
        AttackType = settings.AttackType;
        DamageType = settings.DamageType;
        CharacterClass = settings.CharacterClass;
        AttackDistance = settings.AttackDistance;
        MoveSpeed = settings.DefaultMoveSpeed;
    }

    public float Strength;
    public float Agility;
    public float Intelligence;
    public float DefaultHealth;
    public float DefaultMana;
    public float DefaultMoveSpeed;
    public float DefaultAttackPower;
    public float DefaultDefense;
    public AttackType AttackType;
    public DamageType DamageType;
    public CharacterClass CharacterClass;
    private float multiplierAgilityToAttackSpeed = 0.02f;
    private float health;
    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, MaxHealth);
            HealthChangeAction?.Invoke(health, MaxHealth);
        }
    }
    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = Mathf.Max(DefaultHealth + value, 1);
    }
    private float maxHealth;
    public float HealthRegeneration;
    private float mana;
    public float Mana
    {
        get => mana;
        set
        {
            mana = Mathf.Clamp(value, 0, MaxMana);
            ManaChangeAction?.Invoke(mana, MaxMana);
        }
    }
    public float MaxMana
    {
        get => maxMana;
        set => maxMana = Mathf.Max(DefaultMana + value, 1);
    }
    private float maxMana;
    public float ManaRegeneration;
    public float AttackPower
    {
        get => attackPower;
        set => attackPower = Mathf.Max(DefaultAttackPower + value, 1);
    }
    private float attackPower;
    public float AttackDistance;
    public float BaseAttackSpeed;
    public float CalculateAttackSpeed()
    {
        float agilityContribution = (Agility + AgilityBonus) * multiplierAgilityToAttackSpeed;
        float itemAttackSpeedBonus = AttackRateBonusPercentage / 100;
        float attackSpeedMultiplier = 1 + agilityContribution + itemAttackSpeedBonus;
        return BaseAttackSpeed * attackSpeedMultiplier;
    }
    public float CalculateAttackTime()
    {
        return 1 / CalculateAttackSpeed();
    }
    public float Defense
    {
        get => defense;
        set => defense = Mathf.Max(DefaultDefense + value, 1);
    }
    public float defense;
    public float MagicResist;
    public float MoveSpeed;
    public float DodgeChance;
    public float LifeStealCoefficient;
    public float SplashRadius => splashRadius;
    private float splashRadius = 2f;
    public float SplashDamageMultiplier;
    public float StrengthBonus;
    public float AgilityBonus;
    public float IntelligenceBonus;
    public float HealthBonus => HealthAmountBonus;
    public float ManaBonus => ManaAmountBonus;
    public float AttackBonus => AttackAmountBonus + AttackPower * (AttackBonusPercentage / 100);
    public float DefenseBonus => DefenseAmountBonus + Defense * (DefenseBonusPercentage / 100);
    public float MagicResistBonus => MagicResistAmountBonus + MagicResist * (MagicResistBonusPercentage / 100);
    public float HealthRegenerationBonus => HealthRegenerationAmountBonus + HealthRegeneration * (HealthRegenerationPercentage / 100);
    public float ManaRegenerationBonus => ManaRegenerationAmountBonus + ManaRegeneration * (ManaRegenerationPercentage / 100);
    public float SplashDamage => SplashDamageMultiplierAmountBonus + (AttackPower + AttackBonus) * (SplashDamageMultiplierPercentage / 100);
    public float TakeReflectDamage(float incomingDamage)
    {
        float maxReflection = 98f;
        float coefficient = 0.051f;

        float reflectPercentage = maxReflection * (1 - Mathf.Exp(-coefficient * (Defense + DefenseAmountBonus))) / 100;

        float reflectedDamage = incomingDamage - incomingDamage * reflectPercentage;

        return Mathf.Max(reflectedDamage + ReflectDamageAmountBonus, 1);
    }
    public float TakeReflectMagicDamage(float incomingDamage)
    {
        return Mathf.Max(incomingDamage * ((100 - MagicResistBonusPercentage) / 100) - MagicResistAmountBonus, 1);
    }
    public float HealthAmountBonus;
    public float ManaAmountBonus;
    public float AttackAmountBonus;
    public float DefenseAmountBonus;
    public float MagicResistAmountBonus;
    public float HealthRegenerationAmountBonus;
    public float ManaRegenerationAmountBonus;
    public float SplashDamageMultiplierAmountBonus;
    public float ReflectDamageAmountBonus;
    public float AttackBonusPercentage;
    public float DefenseBonusPercentage;
    public float MagicResistBonusPercentage;
    public float MoveSpeedBonusPercentage;
    public float AttackRateBonusPercentage;
    public float HealthRegenerationPercentage;
    public float ManaRegenerationPercentage;
    public float SplashDamageMultiplierPercentage;
    public float ReflectDamagePercentage;
    public float CriticalMultiplier;
    public float CriticalChance;
    public Action<float, float> HealthChangeAction;
    public Action<float, float> ManaChangeAction;
}
