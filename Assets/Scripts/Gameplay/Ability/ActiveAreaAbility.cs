using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActiveAreaAbility : Ability
{
    [SerializeField] private float damage;
    [SerializeField] private float damagePerLevel;
    [SerializeField] private float areaRadius;
    [SerializeField] private float baseManaCost;
    [SerializeField] private GameObject activationEffectPrefab;
    private string abilityDescription;
    public void Start()
    {
        UpdateAbilityStats();
    }
    protected override async void Activate(CharacterBase user, CharacterBase target = null, Vector3? area = null)
    {

        if (area!=null)
        {
            GameObject effect = Instantiate(activationEffectPrefab, new Vector3(area.Value.x, 0, area.Value.z), Quaternion.identity);
            effect.transform.localScale = new Vector3(areaRadius, areaRadius, areaRadius);
            effect.transform.SetAsFirstSibling();
            Destroy(effect, 3f);
            await Task.Delay(1000);

            List<CharacterBase> characters = CharacterManager.Instance.GetEnemiesInAria(user, area.Value, areaRadius);

            foreach (var character in characters)
            {

                if (character != null)
                {
                    if (character.TeamId != user.TeamId)
                    {
                        character.TakeDamage(damage + (Level - 1) * damagePerLevel, user, DamageType.Magic);
                    }
                    
                }
            }

            UpdateUseTime();
            user.UseMana(ManaCost);
        }
    }

    public override string GetAbilityDescription()
    {
        abilityDescription = $"{Name}\nThrows a fireball that deals damage to the area\n";

        var abilityDescriptionByLevel = new string[MaxLevel];

        for (int i = 0; i < MaxLevel; i++)
        {
            abilityDescriptionByLevel[i] = $"Level {i + 1}: Damage: {damage + i * damagePerLevel} Cooldown: {Cooldown} ManaCost: {baseManaCost + i * 5}";
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
