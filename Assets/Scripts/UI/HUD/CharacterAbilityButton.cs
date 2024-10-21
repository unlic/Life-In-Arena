using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterAbilityButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject activeImage;
    [SerializeField] private CooldownView cooldownView;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private Image levelIndicatorImage;
    [SerializeField] private List<Sprite> levelsIndicatorSprites;

    public Action<CharacterBase, Ability> AbilityClicked;
    public Action LevelUpButtonAction;
    public Action<CharacterAbilityButton> AbilityChoosed;

    private Ability currentAbility;
    private CharacterBase currentUnit;
    private Image icon;
    private Camera mainCamera;
    private bool isHolding = false;
    private float holdTime = 0f;
    private const float holdThreshold = 1.5f;

    private Color availableColor = new Color(0.5754716f, 1, 0.6122565f);
    private Color notAvailableColor = new Color(1, 0.6202026f, 0.5764706f);
    private bool isAbilityChoose = false;
    private void Awake()
    {
        icon = GetComponent<Image>();
        levelIndicatorImage.gameObject.SetActive(false);
        mainCamera = Camera.main;
        levelUpButton.onClick.AddListener(UpLevelAbility);
    }
    private void Update()
    {
        if (currentAbility == null || currentUnit == null)
        {
            levelIndicatorImage.gameObject.SetActive(false);
            icon.color = new Color(1, 1, 1, 0.2f);
            cooldownView.SetData();
            return;
        }
            

        UpdateIconColor();

        if (Input.GetMouseButtonDown(0) && isAbilityChoose && !currentAbility.IsPassiveAbility)
        {
            if (IsPointerOverUIObject())
            {

                return;
            }

            UseAbility();
        }


        cooldownView.SetData(currentAbility.GetTimeSinceLastUse(), currentAbility.Cooldown);
    }

    private void UpdateIconColor()
    {
        if (currentAbility.Level == 0)
        {
            icon.color = new Color(1, 1, 1, 0.2f);
            levelIndicatorImage.gameObject.SetActive(false);
        }
        else if (currentAbility.IsPassiveAbility)
        {
            icon.color = Color.gray;
            levelIndicatorImage.gameObject.SetActive(true);
            levelIndicatorImage.sprite = levelsIndicatorSprites[currentAbility.Level - 1];
        }
        else
        {
            icon.color = currentAbility.ManaCost > currentUnit.CurrentUserStats.Mana ? notAvailableColor : availableColor;
            levelIndicatorImage.gameObject.SetActive(true);
            levelIndicatorImage.sprite = levelsIndicatorSprites[currentAbility.Level - 1];
        }
    }
    private void UpLevelAbility()
    {
        Hero hero = currentUnit as Hero;

        if(hero == null)
        {
            levelIndicatorImage.gameObject.SetActive(false);
            return;
        }
        if (hero.TryUseLevelPoint())
        {
            currentAbility.LevelUp(hero);
        }
        if (currentAbility.Level == 0)
        {
            levelIndicatorImage.gameObject.SetActive(false);
        }
        else
        {
            levelIndicatorImage.gameObject.SetActive(true);
            levelIndicatorImage.sprite = levelsIndicatorSprites[currentAbility.Level - 1];
        }
        LevelUpButtonAction?.Invoke();
    }
    private void UseAbility()
    {
        AbilityClicked?.Invoke(currentUnit, currentAbility);
        isAbilityChoose = false;
        activeImage.SetActive(isAbilityChoose);
    }
    public void SetItem(Ability item, CharacterBase unit)
    {
        currentAbility = item;
        icon.sprite = item.AbilityIcon;
        currentUnit = unit;
        LeveledUp();
    }

    public void SetItem(Sprite empty)
    {
        icon.sprite = empty;
        currentAbility = null;
        LeveledUp();
    }
    public void DeactivateButton()
    {
        isAbilityChoose = false;
        activeImage.SetActive(isAbilityChoose);
    }
    public void LeveledUp()
    {
        levelUpButton.gameObject.SetActive(false);

        if (currentUnit == null || currentAbility == null)
        {
            return;
        }

        Hero hero = currentUnit as Hero;

        if (hero == null)
        {
            levelIndicatorImage.gameObject.SetActive(false);
            return;
        }

        if (!hero.HasUnusedLevelPoints())
        {
            return;
        }
        if (currentAbility.IsUltimate)
        { 
            if (currentAbility.Level == 0 && hero.UnitLevel < 5)
            {
                return;
            }
            else if (currentAbility.Level == 1 && hero.UnitLevel < 9)
            {
                return;
            }
            else if (currentAbility.Level == 2 && hero.UnitLevel < 11)
            {
                return;
            }
            if (currentAbility.Level >= currentAbility.MaxLevel)
            {
                return;
            }
        }
        else
        {
            if ((hero.UnitLevel - currentAbility.Level * 2 <= 0 && currentAbility.Level > 0) || currentAbility.Level >= currentAbility.MaxLevel)
            {
                return;
            }
        }


        levelUpButton.gameObject.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTime = 0f;
        StartCoroutine(HoldItem());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        holdTime = 0f;
    }

    private IEnumerator HoldItem()
    {
        while (isHolding)
        {
            holdTime += Time.deltaTime;

            if (holdTime >= holdThreshold)
            {
                HandleLongPress();
                yield break;
            }

            yield return null;
        }

        HandleShortPress();
    }

    private void HandleShortPress()
    {
        if (currentAbility == null)
        {
            return;
        }
        if (currentAbility.Level == 0)
        {
            return;
        }
        if (currentAbility.ManaCost > currentUnit.CurrentUserStats.Mana || currentAbility.IsPassiveAbility || currentAbility.GetTimeSinceLastUse() > 0)
        {
            return;
        };

        isAbilityChoose = !isAbilityChoose;

        activeImage.SetActive(isAbilityChoose);
        AbilityChoosed?.Invoke(this);
    }

    private void HandleLongPress()
    {
        
    }


    private bool IsPointerOverUIObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }
}
