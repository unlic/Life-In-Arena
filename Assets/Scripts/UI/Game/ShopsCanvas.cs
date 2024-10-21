using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopsCanvas : MonoBehaviour
{
    
    [SerializeField] private StoreList panelType;
    [SerializeField] private List<ItemObject> ItemObjectShop1;
    [SerializeField] private List<ItemObject> ItemObjectShop2;
    [SerializeField] private List<ItemObject> ItemObjectShop3;
    [SerializeField] private List<ItemObject> ItemObjectShop4;
    [SerializeField] private List<ItemObject> ItemObjectShop5;
    [SerializeField] private List<ItemObject> ItemObjectShop6;
    [SerializeField] private ShopItem storeItemPrefab;
    [SerializeField] private List<ShopItem> currentShopItem;
    [SerializeField] private Button buyButton;
    [SerializeField] private RectTransform itemsContent;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private CurrencyAndTimeViewPanel currencyAndTimeViewPanel;
   
    [SerializeField] private Game gameController;
    [SerializeField] private InventoryViewPanel inventoryViewPanel;
    [SerializeField] private InventoryViewPanel storegeViewPanel;

    private ItemObject chooseItem;
    private Toggle toggle;
    private Hero hero;
    public Action<ItemObject> OnBuyItem;
    public Action<StoreList> ChangeStorePanelAction;
    void Start()
    {
        SpawnItems(ItemObjectShop1);
        buyButton.onClick.AddListener(BuyItem);
        hero = gameController.Hero;
        ChangeStorePanelAction += OnToggleSwitch;
        if (hero != null) 
        { 
            hero.OnEquippedItemsChange += OnEquipsItems;
            inventoryViewPanel.SetHero(hero);
            storegeViewPanel.SetHero(hero);
        }
    }

    private void OnToggleSwitch(StoreList panelsType)
    {
        switch (panelsType)
        {
            case StoreList.Shop1:
                SpawnItems(ItemObjectShop1);
                break;
            case StoreList.Shop2:
                SpawnItems(ItemObjectShop2);
                break;
            case StoreList.Shop3:
                SpawnItems(ItemObjectShop3);
                break;
            case StoreList.Shop4:
                SpawnItems(ItemObjectShop4);
                break;
            case StoreList.Shop5:
                SpawnItems(ItemObjectShop5);
                break;
            case StoreList.Shop6:
                SpawnItems(ItemObjectShop6);
                break;
        }
    }
    private void OnEquipsItems()
    {
        inventoryViewPanel.SetItems(hero);
        Debug.Log("OnEquipsItems");
    }
    private void SpawnItems(List<ItemObject> items)
    {
        foreach (var item in currentShopItem)
        {
            Destroy(item.gameObject);
        }
        currentShopItem.Clear();

        foreach (ItemObject item in items)
        {
            var shopItem = Instantiate(storeItemPrefab, itemsContent);
            shopItem.SelectItemAction += SetDescription;
            currentShopItem.Add(shopItem);
            shopItem.Init(item);
        }
    }

    private void SetDescription(ItemObject item)
    {
        chooseItem = item;
        string description = item.GetDescriptionByItemStats();
        string descriptionRecipe = item.GetItemsNameFromRecipe();
        descriptionText.text = description + "\n" + descriptionRecipe;
        priceText.text = $"Price: {item.Price}";

    }

    public void SetTimeInFieldViewInSeconds(int second)
    {
        currencyAndTimeViewPanel.SetTimeInFieldViewInSeconds(second);
    }

    private void BuyItem()
    {
        if (CurrencyManager.Instance.SpendGold(chooseItem.Price))
        {
            OnBuyItem?.Invoke(chooseItem);
        }
    }
}
