using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image clockViewImage;
    public void SetData(float second = 0, float cooldownTime = 0)
    {
       
        if (second <= 0)
        {
            clockViewImage.gameObject.SetActive(false);
            timeText.gameObject.SetActive(false);
        }
        else
        {
            clockViewImage.gameObject.SetActive(true);
            timeText.gameObject.SetActive(true);
            timeText.text = $"{(int)second}";
            clockViewImage.fillAmount = second / cooldownTime;

        }
    }
}
