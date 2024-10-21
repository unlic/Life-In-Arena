using System;
using UnityEngine;
using UnityEngine.UI;

public class HeroViewItem : MonoBehaviour
{
    [SerializeField] private Image iconHero;
    [SerializeField] private Button buttonSelect;

    [SerializeField] private GameObject selectedImage;

    public Action<CharacterSettings> OnSelectHero;

    private CharacterSettings character;

    public void SetHeroSetting(CharacterSettings settings)
    {
        character = settings;

        iconHero.sprite = character.Avatar;

        buttonSelect.onClick.AddListener(SelectHero);
    }

    private void SelectHero()
    {
        selectedImage.SetActive(true);
        OnSelectHero?.Invoke(character);
    }

    public void UnselectHero(CharacterSettings settings)
    {
        selectedImage.SetActive(settings == character);
    }
}
