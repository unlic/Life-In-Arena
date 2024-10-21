using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellInfo : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    
    public void SetSpellInfo(Ability spellInfo)
    {
        icon.sprite = spellInfo.AbilityIcon;
        text.text = spellInfo.GetAbilityDescription();
    }
}
