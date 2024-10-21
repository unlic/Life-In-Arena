using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyAndTimeViewPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI powerPointText;
    private void Start()
    {
        CurrencyManager.Instance.OnGoldChanged += OnGoldChanged;
        CurrencyManager.Instance.OnPowerPointsChanged += OnPowerPointsChanged;
    }
    public void SetTimeInFieldViewInSeconds(int second)
    {
        if (second < 10)
        {
            timeText.color = Color.red;
        }
        else
        {
            timeText.color = Color.white;
        }

        string minuteString = $"{second / 60}";
        string secondString = (second % 60 < 10) ? $"0{second % 60}" : $"{second % 60}";

        timeText.text = $"{minuteString}:{secondString}";
    }

    private void OnGoldChanged(int value)
    {
        goldText.text = $"{value}";
    }

    private void OnPowerPointsChanged(int value)
    {
        powerPointText.text = $"{value}";
    }
}
