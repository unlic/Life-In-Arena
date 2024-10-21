using System.Collections.Generic;
using UnityEngine;

public class HeroInfoPage : MonoBehaviour
{
    [SerializeField] private List<SpellInfo> spellInfos;


    public void ShowInfoPage(CharacterSettings character)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < spellInfos.Count; i++) 
        {
            spellInfos[i].SetSpellInfo(character.CharacterAbilities[i]);
        }
    }

    public void HideInfoPage()
    {
        gameObject.SetActive(false);
    }
}
