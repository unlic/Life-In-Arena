using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Backpack : MonoBehaviour
{
    [SerializeField] private List<BackpackItems> backpackItems;
    [SerializeField] private Sprite emptyImage;
    [SerializeField] private PlayerControls playerMoveControls;
    private void Start()
    {
        for (int i = 0; i < backpackItems.Count; i++)
        {
            backpackItems[i].AbilityChoosed += AbilityChoosed;
            backpackItems[i].AbilityClicked += UseAbility;
        }
    }
    public void SetItemsInBackpack(List<ItemObject> items, CharacterBase unit)
    {
        for (int i = 0; i < backpackItems.Count; i++)
        {
            if(items.Count<=i)
            {
                backpackItems[i].SetItem(emptyImage);
                return;
            }
               

            if (items[i] != null)
            {
                backpackItems[i].SetItem(items[i], unit);
            }
            else
            {
                backpackItems[i].SetItem(emptyImage);
            }
        }

    }
    private void UseAbility(CharacterBase unit, Ability ability)
    {
        playerMoveControls.UseAbility(unit, ability);
    }
    private void AbilityChoosed(BackpackItems button)
    {
        for (int i = 0; i < backpackItems.Count; i++)
        {
            if (backpackItems[i] != button)
            {
                backpackItems[i].DeactivateButton();
            }
        }
    }
}
