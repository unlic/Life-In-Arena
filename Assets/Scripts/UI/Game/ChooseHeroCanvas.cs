using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ChooseHeroCanvas : MonoBehaviour
{
    [SerializeField] private List<CharacterSettings> heroList = new List<CharacterSettings>();

    [SerializeField] private HeroViewItem heroViewItemPrefab;

    [SerializeField] private RectTransform contentWarrior;
    [SerializeField] private RectTransform contentAssassin;
    [SerializeField] private RectTransform contentMage;

    [SerializeField] private HeroInfoPage heroInfoPage;
    [SerializeField] private Game gameController;

    [SerializeField] private Button openInfoButton;
    [SerializeField] private Button chooseHeroButton;
    [SerializeField] private Button chooseHeroInInfoPageButton;
    [SerializeField] private Button backButton;


    private List<HeroViewItem> heroViewItems = new List<HeroViewItem>();

    private CharacterSettings selectedHero;

    private void Start()
    {
        openInfoButton.onClick.AddListener(ShowHeroInfo);
        chooseHeroButton.onClick.AddListener(ChooseHeroInfo);
        chooseHeroInInfoPageButton.onClick.AddListener(ChooseHeroInfo);
        backButton.onClick.AddListener(CloseInfoPage);


        foreach (CharacterSettings hero in heroList)
        {
            switch (hero.CharacterClass)
            {
                case CharacterClass.Warrior:
                    heroViewItems.Add(Instantiate(heroViewItemPrefab, contentWarrior)); break;
                case CharacterClass.Assassin:
                    heroViewItems.Add(Instantiate(heroViewItemPrefab, contentAssassin)); break;
                case CharacterClass.Mage:
                    heroViewItems.Add(Instantiate(heroViewItemPrefab, contentMage)); break;

            }

            heroViewItems[heroViewItems.Count - 1].OnSelectHero += SelectHero;
            heroViewItems[heroViewItems.Count - 1].SetHeroSetting(hero);
        }
    }

    private void SelectHero(CharacterSettings character)
    {
        selectedHero = character;

        foreach (HeroViewItem hero in heroViewItems)
        {
            hero.UnselectHero(selectedHero);
        }

    }

    private void ShowHeroInfo()
    {
        heroInfoPage.ShowInfoPage(selectedHero);
    }

    private void ChooseHeroInfo()
    {
        gameController.StartGame(selectedHero);
        gameObject.SetActive(false);
    }   
    
    private void CloseInfoPage()
    {
        heroInfoPage.HideInfoPage();
        
    }
}