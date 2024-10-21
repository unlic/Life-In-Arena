using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class BackpackItems : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private CooldownView cooldownView;
    [SerializeField] private GameObject activeImage;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject recipeIcon;
    private ItemObject currentItem;
    
    private Camera mainCamera;
    private bool isHolding = false;
    private float holdTime = 0f;
    private const float holdThreshold = 1.5f;
    private CharacterBase currentUnit;
    private Color availableColor = new Color(0.5754716f, 1, 0.6122565f);
    private Color notAvailableColor = new Color(1, 0.6202026f, 0.5764706f);
    private bool isAbilityChoose = false;

    public Action<BackpackItems> AbilityChoosed;
    public Action<CharacterBase, Ability> AbilityClicked;
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
       
        if (currentItem == null)
        {
            icon.color = Color.clear;
            cooldownView.SetData();
            return;
        }
        
        if (currentItem.Ability == null)
        {
            icon.color = Color.white;
            cooldownView.SetData();
            return;
        }

        if (Input.GetMouseButtonDown(0) && isAbilityChoose && !currentItem.Ability.IsPassiveAbility)
        {
            if (IsPointerOverUIObject())
            {

                return;
            }

            UseAbility();
        }

        if (!currentItem.Ability.IsPassiveAbility )
        {
            icon.color = currentItem.Ability.ManaCost > currentUnit.CurrentUserStats.Mana ? notAvailableColor : availableColor;
        }
        else
        {
            icon.color = Color.gray;
        }
        cooldownView.SetData(currentItem.Ability.GetTimeSinceLastUse(), currentItem.Ability.Cooldown);
    }
    private void UseAbility()
    {
        AbilityClicked?.Invoke(currentUnit, currentItem.Ability);
        isAbilityChoose = false;
        activeImage.SetActive(isAbilityChoose);
    }
    public void SetItem(ItemObject item, CharacterBase unit)
    {
        currentItem = item;
        icon.sprite = item.GetIcon();
        currentUnit = unit;
        recipeIcon.SetActive(item.IsRecipe);
    }

    public void SetItem(Sprite empty)
    {
        currentItem = null;
        icon.sprite = empty;
        currentUnit = null;
        recipeIcon.SetActive(false);

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
        if (currentItem == null)
        {
            return;
        }

        if (currentItem.Ability == null)
        {
            return;
        }

        if (currentItem.Ability.ManaCost > currentUnit.CurrentUserStats.Mana || currentItem.Ability.IsPassiveAbility || currentItem.Ability.GetTimeSinceLastUse() > 0)
        {
            return;
        };

        isAbilityChoose = !isAbilityChoose;
        activeImage.SetActive(isAbilityChoose);
        AbilityChoosed?.Invoke(this);
    }

    public void DeactivateButton()
    {
        isAbilityChoose = false;
        activeImage.SetActive(isAbilityChoose);
    }
    private void HandleLongPress()
    {
        if (currentItem != null)
        {
            Debug.Log(currentItem.GetDescriptionByItemStats());
        }
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
