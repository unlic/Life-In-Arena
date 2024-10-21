using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private CharacterStats currentUnitStats;
    private CharacterBase currentUnit;
    private List<ItemObject> equippedItems = new List<ItemObject>();
    private int maxHoldCountItem = 9;
    public Action OnEquippedItemsUpdate;

    private void Start()
    {
        for (int i = 0; i < maxHoldCountItem; i++)
        {
            equippedItems.Add(null);
        }
    }

    public void SetUnitStats(CharacterStats unitStats, CharacterBase unit)
    {
        currentUnitStats = unitStats;
        currentUnit = unit;
    }

    public void EquipItem(ItemObject item, int id = -1)
    {
        if (item == null) return;
        if (equippedItems.Count < id) return;

        if (item.IsRecipe)
        {
            CraftItem(item);

            return;
        }

        if(id == -1)
            id = FindFirstEmptySlot();

        if(id == -1) return;

        equippedItems[id] = item;
        ApplyBonuses(item);
    }
    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < equippedItems.Count; i++)
        {
            if (equippedItems[i] == null) return i;
        }
        return -1;
    }
    public void UnequipItem(ItemObject item)
    {
        if (item == null) return;

        int id = equippedItems.IndexOf(item);
        equippedItems[id] = null;
        RemoveBonuses(item);
    }
    public void ChangingItemsPlaces(int firstId, int secondId)
    {
        ItemObject tempItem = equippedItems[firstId];

        equippedItems[firstId] = equippedItems[secondId];
        equippedItems[secondId] = tempItem;

        OnEquippedItemsUpdate?.Invoke();
    }
    private void ApplyBonuses(ItemObject item)
    {
        var stats = item.Item;
        if (!item.IsRecipe)
        {
            item.UseAbility(currentUnit, null, null, true);
            ModifyBonuses(stats, true);
        }
        OnEquippedItemsUpdate?.Invoke();
    }

    private void RemoveBonuses(ItemObject item)
    {
        var stats = item.Item;
        if (!item.IsRecipe)
        {
            item.DeactivatedItemAbility();
            ModifyBonuses(stats, false);
        }
        OnEquippedItemsUpdate?.Invoke();
    }
    private void ModifyBonuses(Item stats, bool isAdding)
    {
        int modifier = isAdding ? 1 : -1;

        currentUnitStats.StrengthBonus += modifier * stats.StrengthBonus;
        currentUnitStats.AgilityBonus += modifier * stats.AgilityBonus;
        currentUnitStats.IntelligenceBonus += modifier * stats.IntelligenceBonus;

        currentUnitStats.HealthAmountBonus += modifier * stats.HealthAmountBonus;
        currentUnitStats.ManaAmountBonus += modifier * stats.ManaAmountBonus;
        currentUnitStats.AttackAmountBonus += modifier * stats.AttackAmountBonus;
        currentUnitStats.DefenseAmountBonus += modifier * stats.DefenseAmountBonus;
        currentUnitStats.MagicResistAmountBonus += modifier * stats.MagicResistAmountBonus;
        currentUnitStats.HealthRegenerationAmountBonus += modifier * stats.HealthRegenerationAmountBonus;
        currentUnitStats.ManaRegenerationAmountBonus += modifier * stats.ManaRegenerationAmountBonus;
        currentUnitStats.SplashDamageMultiplierAmountBonus += modifier * stats.SplashDamageAmountBonus;
        currentUnitStats.ReflectDamageAmountBonus += modifier * stats.ReflectDamageAmountBonus;

        currentUnitStats.AttackBonusPercentage += modifier * stats.AttackBonusPercentage;
        currentUnitStats.DefenseBonusPercentage += modifier * stats.DefenseBonusPercentage;
        currentUnitStats.MagicResistBonusPercentage += modifier * stats.MagicResistBonusPercentage;
        currentUnitStats.MoveSpeedBonusPercentage += modifier * stats.MoveSpeedBonusPercentage;
        currentUnitStats.AttackRateBonusPercentage += modifier * stats.AttackRateBonusPercentage;
        currentUnitStats.HealthRegenerationPercentage += modifier * stats.HealthRegenerationPercentage;
        currentUnitStats.ManaRegenerationPercentage += modifier * stats.ManaRegenerationPercentage;
        currentUnitStats.SplashDamageMultiplierPercentage += modifier * stats.SplashDamagePercentage;
        currentUnitStats.ReflectDamagePercentage += modifier * stats.ReflectDamagePercentage;
        currentUnitStats.DodgeChance += modifier * stats.DodgeChancePercentage;

        
    }
    private bool CanCraft(ItemObject itemObject)
    {
        var requiredItems = itemObject.GetItemsFromRecipe();
        Dictionary<int, int> itemCounts = new Dictionary<int, int>();

        foreach (ItemObject requiredItem in requiredItems)
        {
            if (itemCounts.ContainsKey(requiredItem.Id))
            {
                itemCounts[requiredItem.Id]++;
            }
            else
            {
                itemCounts[requiredItem.Id] = 1;
            }
        }

        foreach (var requiredItem in itemCounts)
        {
            int equippedItemCount = 0;

            foreach (ItemObject equippedItem in equippedItems)
            {
                if (equippedItem != null)
                    if (equippedItem.Id == requiredItem.Key)
                    {
                        equippedItemCount++;
                    }
            }
            if (equippedItemCount < requiredItem.Value)
            {
                return false;
            }
        }

        return true;
    }

    private void CraftItem(ItemObject itemObject)
    {
        if (CanCraft(itemObject))
        {
            var requiredItems = itemObject.GetItemsFromRecipe();
            Dictionary<int, int> itemCounts = new Dictionary<int, int>();
            foreach (ItemObject requiredItem in requiredItems)
            {
                if (itemCounts.ContainsKey(requiredItem.Id))
                {
                    itemCounts[requiredItem.Id]++;
                }
                else
                {
                    itemCounts[requiredItem.Id] = 1;
                }
            }
            List<ItemObject> removeItems = new List<ItemObject>();
            foreach (var requiredItem in itemCounts)
            {
                int itemsToRemove = requiredItem.Value;

                foreach (ItemObject equippedItem in equippedItems)
                {
                    if (equippedItem != null)
                        if (equippedItem.Id == requiredItem.Key && itemsToRemove > 0)
                        {
                            removeItems.Add(equippedItem);
                            itemsToRemove--;
                        }
                }
            }

            int id;
            foreach (ItemObject equippedItem in removeItems)
            {
                id = equippedItems.IndexOf(equippedItem);
                equippedItems[id] = null;
                RemoveBonuses(equippedItem);
                Destroy(equippedItem.gameObject);
            }
            id = FindFirstEmptySlot();
            equippedItems[id] = itemObject;
            itemObject.IsRecipe = false;
            ApplyBonuses(itemObject);

            Debug.Log("Предмет создан: " + itemObject.Item.ItemName);
        }
        else
        {
            int id = FindFirstEmptySlot();
            equippedItems[id] = itemObject;
            Debug.Log("Недостаточно предметов для создания.");
            OnEquippedItemsUpdate?.Invoke();
        }
    }
    public List<ItemObject> GetEquippedItems()
    {
        return equippedItems;
    }


}