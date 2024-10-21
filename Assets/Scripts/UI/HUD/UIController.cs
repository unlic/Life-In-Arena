using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image experienceBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button openPopupButton;

    [SerializeField] private PopupInfo popupInfo;
    [SerializeField] private Backpack backpack;
    [SerializeField] private CharacterAbilitys userAbility;


    [SerializeField] private Button openShopButton;
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private Button skipTimerButton;
    [SerializeField] private CurrencyAndTimeViewPanel currencyAndTimeViewPanel;

    public Action<int> ChangeStateLevelUpButtons;
    public Action OnSkipTimer;
    public Action OnOpenShop;
    private CharacterBase currentUnit;
    private void Awake()
    {
        openPopupButton.onClick.AddListener(OpenInfoPopup);
        skipTimerButton.onClick.AddListener(OnSkipTimerClick);
        openShopButton.onClick.AddListener(OnOpenShopClick);
    }


    private void SubscribeToEvents()
    {
        if (currentUnit != null)
        {
            currentUnit.CurrentUserStats.HealthChangeAction += HealthChange;
            currentUnit.CurrentUserStats.ManaChangeAction += ManaChange;
            

            var experienceReceiver = currentUnit as IExperienceReceiver;

            if (experienceReceiver != null)
            {
                experienceReceiver.OnExperiencePointsChange += ExperiencePointsChange;
                experienceReceiver.OnLevelChange += LevelChange;
            }

            var hasInventory = currentUnit as IHasInventory;

            if (hasInventory != null)
            {
                hasInventory.OnEquippedItemsChange += OnEquipsItemsChange;
            }
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (currentUnit != null)
        {
            currentUnit.CurrentUserStats.HealthChangeAction -= HealthChange;
            currentUnit.CurrentUserStats.ManaChangeAction -= ManaChange;

            var experienceReceiver = currentUnit as IExperienceReceiver;
            if (experienceReceiver != null)
            {
                experienceReceiver.OnExperiencePointsChange -= ExperiencePointsChange;
                experienceReceiver.OnLevelChange -= LevelChange;
            }
            var hasInventory = currentUnit as IHasInventory;

            if (hasInventory != null)
            {
                hasInventory.OnEquippedItemsChange -= OnEquipsItemsChange;
            }
        }
    }
    public void SetDataInView(CharacterBase unit)
    {
        UnsubscribeFromEvents();

        currentUnit = unit;
        SubscribeToEvents();
        currentUnit.TakeData();

        userAbility.SetUserAbility(currentUnit.GetUserAbilitys(), currentUnit);

        var hasInventory = currentUnit as IHasInventory;

        if (hasInventory != null)
        {
            backpack.SetItemsInBackpack(hasInventory.GetEquippedItems(), currentUnit);
        }

        var experienceReceiver = currentUnit as IExperienceReceiver;

        if (experienceReceiver != null)
        {
            LevelChange(experienceReceiver.UnitLevel);
        }
    }
    public void SetTimeInFieldViewInSeconds(int second)
    {
        currencyAndTimeViewPanel.SetTimeInFieldViewInSeconds(second);

        if (second > 0)
        {
            skipTimerButton.gameObject.SetActive(true);
            openShopButton.gameObject.SetActive(true);
        }
        else
        {
            skipTimerButton.gameObject.SetActive(false);
            openShopButton.gameObject.SetActive(false);
            shopCanvas.SetActive(false);
        }
    }
    private void HealthChange(float amount, float max)
    {
        healthBar.fillAmount = amount / max;
        healthText.text = $"{Mathf.FloorToInt(amount)}/{Mathf.FloorToInt(max)}";
    }

    private void ManaChange(float amount, float max)
    {
        manaBar.fillAmount = amount / max;
        manaText.text = $"{Mathf.FloorToInt(amount)}/{Mathf.FloorToInt(max)}";
    }

    private void ExperiencePointsChange(float amount, float max, float previousLevel)
    {
        experienceBar.fillAmount = (amount - previousLevel) / (max- previousLevel);
        experienceText.text = $"{Mathf.FloorToInt(amount)}/{Mathf.FloorToInt(max)}";
    }

    private void LevelChange(int level)
    {
        levelText.text = $"{level}";
        userAbility.ActivateUpLevelButton();
    }

    private void OpenInfoPopup()
    {
        popupInfo.ShowPopup(currentUnit.CurrentUserStats);
    }

    private void OnEquipsItemsChange()
    {
        var hasInventory = currentUnit as IHasInventory;

        if (hasInventory != null)
        {
            backpack.SetItemsInBackpack(hasInventory.GetEquippedItems(), currentUnit);
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void OnSkipTimerClick()
    {
        OnSkipTimer?.Invoke();
    }

    private void OnOpenShopClick()
    {
        shopCanvas.SetActive(true);
        OnOpenShop?.Invoke();
    }
}
