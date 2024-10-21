using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasInventory
{
    public event Action OnEquippedItemsChange;
    public void EquipItem(ItemObject item, int id = -1);
    public void EquipItemFronStorege(ItemObject item, int id);
    public void UnequipItem(ItemObject item);
    public List<ItemObject> GetEquippedItems();
    public bool HasInventorySpace();
    public void SwapItems(int firstId, int secondId);
    public float CalculateCriticalMultiplier();
}
