using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject recipeIcon;
    private ItemObject itemObject;
    private ItemLocation itemLocation;

    public Action<ItemObject, int> OnEquipItem;
    public Action<ItemObject> OnRequipItem;
    public Action<int,int> OnChangingItemsPlaces;
    public ItemLocation ItemLocation => itemLocation;
    public ItemObject ItemObject
    {
        set => itemObject = value;
        get => itemObject;
    }

    public void SetItem(ItemObject item, ItemLocation location = ItemLocation.None)
    {
        if(location != ItemLocation.None)
        {
            itemLocation = location;
        }
        ItemObject = item;
        if (item == null) 
        {
            icon.color = Color.clear;
            recipeIcon.SetActive(false);
        }
        else
        {
            icon.sprite = ItemObject.Item.icon;
            icon.color = Color.white;
            recipeIcon.SetActive(item.IsRecipe);
        }
    }
    public void RequipItem()
    {
        if (ItemObject == null)
            return;

        OnRequipItem?.Invoke(ItemObject);
    }

    public void EquipItem(int to)
    {
        if (ItemObject == null)
            return;

        OnEquipItem?.Invoke(ItemObject, to);
    }

    public void ChangingItemsPlaces(int firstId, int secondId)
    {
        OnChangingItemsPlaces?.Invoke(firstId, secondId);
    }
}
