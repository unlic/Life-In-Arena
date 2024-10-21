using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image icon;

    private Button itemButton;
    private ItemObject item;
    public Action<ItemObject> SelectItemAction;
    public void Init(ItemObject itemObject)
    { 
        itemButton = GetComponent<Button>();
        itemButton.onClick.AddListener(SetDescription);
        item = itemObject;
        if (item != null) 
        { 
        
        }

        icon.sprite = itemObject.GetIcon();
    }

    private void SetDescription()
    {
        SelectItemAction?.Invoke(item);
    }

}
