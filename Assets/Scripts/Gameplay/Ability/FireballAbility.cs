using System;
using UnityEngine;
public class FireballAbility : Ability
{
    [SerializeField] private float baseDamage;
    [SerializeField] private float damagePerLevel;
    [SerializeField] private float baseManaCost;
    [SerializeField] private GameObject activationEffectPrefab;
    private string abilityDescription;



    public void Start()
    {
        UpdateAbilityStats();
    }
    protected override void Activate(CharacterBase user, CharacterBase target = null, Vector3? area = null)
    {
        if (target != null)
        {
            if (target is ITeam targetUnit && targetUnit.TeamId == user.TeamId)
            {
                Debug.Log("Не можете атаковать своих!");
                return;
            }

            GameObject effect = Instantiate(activationEffectPrefab, target.transform.position, Quaternion.identity);
            Destroy(effect, 0.5f);

            float damage = baseDamage + (Level - 1) * damagePerLevel;
            target.TakeDamage(damage, user, DamageType.Magic);

            IDamageEffect damageEffect = new StunDamageEffect(4);
            damageEffect.ApplyEffect(user, target, 0);
            user.UseMana(ManaCost);
            UpdateUseTime();
        }
    }
    public override string GetAbilityDescription()
    {

        abilityDescription = $"{Name}\nThrows a fireball that deals damage to the target\n";

        var abilityDescriptionByLevel = new string[MaxLevel];

        for (int i = 0; i < MaxLevel; i++)
        {
            abilityDescriptionByLevel[i] = $"Level {i + 1}: Damage: {baseDamage + i * damagePerLevel} Cooldown: {Cooldown} ManaCost: {baseManaCost + i * 5}";
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
        ManaCost = baseManaCost + (Level - 1) * 5;
    }
}
