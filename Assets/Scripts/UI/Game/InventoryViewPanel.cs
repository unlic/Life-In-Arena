using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryViewPanel : MonoBehaviour
{
    [SerializeField] private ItemInUI itemInUIPrefab; 
    [SerializeField] private RectTransform itemHolder;
    [SerializeField] private ItemLocation itemLocation;
    private int maxAmountEquipsItems = 9;
    private Hero currentHero;
    private List<ItemInUI> itemInUIs = new List<ItemInUI>();
    private void Start()
    {
        for (int i = 0; i < maxAmountEquipsItems; i++)
        {
            var item = Instantiate(itemInUIPrefab, itemHolder);
            
            itemInUIs.Add(item);
            item.SetItem(null, itemLocation);
            item.OnEquipItem += EquipItem;
            item.OnRequipItem += RequipItem;
            item.OnChangingItemsPlaces += ChangingItemsPlaces;

            item.gameObject.name = itemLocation.ToString() + i;
        }
    }
    public void SetItems(Hero hero)
    {
        currentHero = hero;
        var equippedItems = hero.GetEquippedItems();
        for (int i = 0; i < maxAmountEquipsItems; i++)
        {
            if (i < equippedItems.Count)
                itemInUIs[i].SetItem(equippedItems[i], itemLocation);
            else
                itemInUIs[i].SetItem(null, itemLocation);
        }
    }

    public void SetHero(Hero hero)
    {
        currentHero = hero;
    }

    private void EquipItem(ItemObject item, int id)
    {
        if (item != null)
            currentHero.EquipItemFronStorege(item, id);
    }

    private void RequipItem(ItemObject item)
    {
        if (item != null)
            currentHero.UnequipItem(item);
    }    
    
    private void ChangingItemsPlaces(int firstId, int secondId)
    {
        currentHero.SwapItems(firstId, secondId);
    }
}
