using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int Id;
    public bool IsRecipe;
    
    [SerializeField] private Item item;
    [SerializeField] private Ability ability;
    [SerializeField] private Recipe recipe;
    [SerializeField] private int price;
    // { get; private set; }
    
    public Item Item => item;
    public Ability Ability => ability;
    public Recipe Recipe => recipe;
    public int Price => price;

    private MeshRenderer meshRenderer;
    public void UseAbility(CharacterBase user, CharacterBase target = null, Vector3? area = null, bool isNotRecipt = false)
    {
        if (IsRecipe&&!isNotRecipt)
        {
            return;
        }

        if (ability == null)
            return;

        if (ability.IsPassiveAbility)
            ability.UseAbility(user, target, area);
    }

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if(meshRenderer != null)
            meshRenderer.enabled = false;
    }
    public void DeactivatedItemAbility()
    {
        if (ability == null) return;

        ability.Deactivate();

    }
    public Sprite GetIcon()
    {
        return item.icon;
    }
    public string GetDescriptionByItemStats()
    {
        List<string> bonuses = new List<string>();

        void AddBonus(string name, float value, string format = "{0}: {1}")
        {
            if (value != 0)
            {
                bonuses.Add(string.Format(format, name, value));
            }
        }

        AddBonus("Strength Bonus", Item.StrengthBonus);
        AddBonus("Agility Bonus", Item.AgilityBonus);
        AddBonus("Intelligence Bonus", Item.IntelligenceBonus);
        AddBonus("Health Amount Bonus", Item.HealthAmountBonus);
        AddBonus("Attack Amount Bonus", Item.AttackAmountBonus);
        AddBonus("Defense Amount Bonus", Item.DefenseAmountBonus);
        AddBonus("Move Speed Bonus", Item.MoveSpeedBonusPercentage * 100, "Move Speed Bonus: {1}%");
        AddBonus("Attack Speed Bonus", Item.AttackRateBonusPercentage);

        if (Item.CriticalMultiplier != 0 && Item.CriticalChance != 0)
        {
            bonuses.Add($"Critical Damage: x{Item.CriticalMultiplier} with a chance of {Item.CriticalChance}%");
        }

        if (Ability != null)
        {
            bonuses.Add($"Ability:\n{Ability.GetAbilityDescription()}");
        }

        return string.Join("\n", bonuses);
    }

    public string GetItemsNameFromRecipe()
    {
        return recipe != null ? recipe.GetItemsNameFromRecipe() : "";
    }

    public List<ItemObject> GetItemsFromRecipe()
    {
        return recipe.RequiredItems;
    }

    public List<int> GetItemsIdFromRecipe()
    {
        return recipe.GetItemsIdFromRecipe();
    }
}
