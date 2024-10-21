using System;
using UnityEngine;

[Serializable]
public class Item
{
    // Basic stats
    public string ItemName;
    public Sprite icon;

    // Attribute bonuses
    public float StrengthBonus;
    public float AgilityBonus;
    public float IntelligenceBonus;

    // Flat bonuses
    public float HealthAmountBonus;
    public float ManaAmountBonus;
    public float HealthRegenerationAmountBonus;
    public float ManaRegenerationAmountBonus;
    public float AttackAmountBonus;
    public float SplashDamageAmountBonus;
    public float DefenseAmountBonus;
    public float MagicResistAmountBonus;
    public float ReflectDamageAmountBonus;

    // Percentage-based bonuses
    public float HealthRegenerationPercentage;
    public float ManaRegenerationPercentage;
    public float AttackBonusPercentage;
    public float AttackRateBonusPercentage;
    public float SplashDamagePercentage;
    public float DefenseBonusPercentage;
    public float MagicResistBonusPercentage;
    public float DodgeChancePercentage;
    public float ReflectDamagePercentage;
    public float MoveSpeedBonusPercentage;

    // Critical hit bonuses
    public float CriticalMultiplier; 
    public float CriticalChance;     
}