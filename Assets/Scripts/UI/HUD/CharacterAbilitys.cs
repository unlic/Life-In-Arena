using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAbilitys : MonoBehaviour
{
    [SerializeField] List<CharacterAbilityButton> abilityIcon;
    [SerializeField] private Sprite emptyImage;
    
    [SerializeField] private PlayerControls playerMoveControls;
    List<Ability> userAbilities;

    

    private void Start()
    {
        for (int i = 0; i < abilityIcon.Count; i++)
        {
            abilityIcon[i].LevelUpButtonAction += ActivateUpLevelButton;  
            abilityIcon[i].AbilityClicked += UseAbility;  
            abilityIcon[i].AbilityChoosed += AbilityChoosed;  
        }
    }
    public void SetUserAbility(List<Ability> abilitys, CharacterBase unit)
    {
        for (int i = 0; i < abilityIcon.Count; i++)
        {
            if (abilitys.Count > i)
            {
                abilityIcon[i].SetItem(abilitys[i], unit);
            }
            else
            {
                abilityIcon[i].SetItem(emptyImage);
            }
        }
    }

    public void ActivateUpLevelButton()
    {
        for (int i = 0; i < abilityIcon.Count; i++)
        {
            abilityIcon[i].LeveledUp();
        }
    }

    private void UseAbility(CharacterBase unit,Ability ability) 
    {
        playerMoveControls.UseAbility(unit, ability);
    }

    private void AbilityChoosed(CharacterAbilityButton button)
    {
        for (int i = 0; i < abilityIcon.Count; i++)
        {
            if (abilityIcon[i] != button)
            {
                abilityIcon[i].DeactivateButton();
            }
        }
    }
}
