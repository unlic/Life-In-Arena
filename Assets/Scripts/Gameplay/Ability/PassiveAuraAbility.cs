using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAuraAbility : Ability
{
    
    [SerializeField] private float attackBonusPercentage = 30f;
    [SerializeField] private float attackBonusPerLevelPercentage = 25f;
    [SerializeField] private float currentAttackBonusPercentage;
    [SerializeField] private float areaRadius;
    private List<CharacterBase> affectedUnits = new List<CharacterBase>();
    private Coroutine auraCoroutine;
    private string abilityDescription;
    protected override void Activate(CharacterBase user, CharacterBase target = null, Vector3? area = null)
    {
        auraCoroutine = StartCoroutine(CheckAuraEffect(user));
    }
    public override void Deactivate()
    {
        if (Level > 0)
        {
            
            if (auraCoroutine != null)
            {
                StopCoroutine(auraCoroutine);
                auraCoroutine = null;
            }

            RemoveAuraFromAllUnits();
        }
            
    }
    public override string GetAbilityDescription()
    {

        abilityDescription = $"{Name}\nIncreases damage of allies in range\n";

        var abilityDescriptionByLevel = new string[MaxLevel];

        for (int i = 0; i < MaxLevel; i++)
        {
            abilityDescriptionByLevel[i] = $"Level {i + 1}: Damage: {attackBonusPercentage + i * attackBonusPerLevelPercentage}%";
        }
        for (int i = 0; i < MaxLevel; i++)
        {
            abilityDescription += abilityDescriptionByLevel[i] + "\n";
        }
        if (MaxLevel == 1)
        {
            abilityDescription.Replace("Level", "");
        }

        return abilityDescription;
    }

    protected override void UpdateAbilityStats()
    {
        currentAttackBonusPercentage = attackBonusPercentage + (Level - 1) * attackBonusPerLevelPercentage;
    }

    private IEnumerator CheckAuraEffect(CharacterBase owner)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if(owner == null)
                break;

            List<CharacterBase> charactersInArea = CharacterManager.Instance.GetAlliesInRange(owner, areaRadius);
            foreach (var characterInArea in charactersInArea)
            {
                if (characterInArea != null && !affectedUnits.Contains(characterInArea))
                {
                    ApplyAuraEffect(characterInArea);
                    affectedUnits.Add(characterInArea);
                }
            }

            for (int i = affectedUnits.Count - 1; i >= 0; i--)
            {
                if (Vector3.Distance(owner.transform.position, affectedUnits[i].transform.position) > areaRadius)
                {
                    RemoveAuraEffect(affectedUnits[i]);
                    affectedUnits.RemoveAt(i);
                }
            }
        }
    }

    private void ApplyAuraEffect(CharacterBase unit)
    {
        if (unit == null) return;
        unit.CurrentUserStats.AttackBonusPercentage += currentAttackBonusPercentage;
        Debug.Log("Aura from item applied to " + unit.name);
    }

    private void RemoveAuraEffect(CharacterBase unit)
    {
        if (unit == null) return;
        unit.CurrentUserStats.AttackBonusPercentage -= currentAttackBonusPercentage;
        Debug.Log("Aura from item removed from " + unit.name);
    }

    private void RemoveAuraFromAllUnits()
    {
        foreach (var unit in affectedUnits)
        {
            RemoveAuraEffect(unit);
        }
        affectedUnits.Clear();
    }
}
