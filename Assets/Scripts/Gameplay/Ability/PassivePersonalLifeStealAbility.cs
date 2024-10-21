using System;
using UnityEngine;

public class PassivePersonalLifeStealAbility : Ability
{
    
    [SerializeField] private float lifeStealCoefficientPerLevel;
    [SerializeField] private float baseLifeStealCoefficient;
    private float currentLifeStealCoefficient;
    private string abilityDescription;
    private string[] abilityDescriptionByLevel;
    
    protected override void Activate(CharacterBase user, CharacterBase target = null, Vector3? area = null)
    {
        UpdateAbilityStats();
        user.CurrentUserStats.LifeStealCoefficient += currentLifeStealCoefficient;
        CurrentUser = user;
    }

    public override void Deactivate()
    {
        if(Level>0)
            CurrentUser.CurrentUserStats.LifeStealCoefficient -= baseLifeStealCoefficient + (Level - 1) * lifeStealCoefficientPerLevel;
    }

    public override string GetAbilityDescription()
    {
        abilityDescription = "Life Steal\nPassive Ability\n";
        abilityDescriptionByLevel = new string[MaxLevel];

        for (int i = 0; i < MaxLevel; i++)
        {
            abilityDescriptionByLevel[i] = $"Level {i + 1}: Steals {(baseLifeStealCoefficient + i * lifeStealCoefficientPerLevel) * 100}% of damage dealt to living creatures.";
        }
        for (int i = 0; i < MaxLevel; i++)
        {
            abilityDescription += abilityDescriptionByLevel[i]+ "\n";
        }
        if(MaxLevel == 1)
        {
            abilityDescription.Replace("Level", "");
        }

        return abilityDescription;
    }
    protected override void UpdateAbilityStats()
    {
        currentLifeStealCoefficient = baseLifeStealCoefficient + (Level - 1) * lifeStealCoefficientPerLevel;
    }
}