using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopupInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI strengthText;  
    [SerializeField] private TextMeshProUGUI agilityText;   
    [SerializeField] private TextMeshProUGUI intelligenceText;
    [SerializeField] private TextMeshProUGUI attackPowerText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(HidePopup);
    }

    public void ShowPopup(CharacterStats unitStats)
    {
        strengthText.text = $"Strength: {unitStats.Strength}" + (unitStats.StrengthBonus>0? $" + {unitStats.StrengthBonus}":"");
        agilityText.text = $"Agility: {unitStats.Agility}" + (unitStats.AgilityBonus > 0? $" + {unitStats.AgilityBonus}" : "");
        intelligenceText.text = $"Intelligence: {unitStats.Intelligence}" + (unitStats.IntelligenceBonus > 0 ? $" + {unitStats.IntelligenceBonus}" : "");
        attackPowerText.text = $"Damage: {unitStats.AttackPower}" + (unitStats.AttackBonus > 0 ? $" + {unitStats.AttackBonus}" : "");
        //attackSpeedText.text = $"Attack Speed: {unit.AttackSpeed} + {unit.AttackSpeedBonus}";
        defenseText.text = $"Defense: {unitStats.Defense} " + (unitStats.DefenseBonus > 0 ? $" + {unitStats.DefenseBonus}" : "");
        gameObject.SetActive(true);
    }

    private void HidePopup()
    {
        gameObject.SetActive(false);
    }
}
