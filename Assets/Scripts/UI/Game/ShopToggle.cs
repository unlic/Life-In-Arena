using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopToggle : MonoBehaviour
{
    [SerializeField] private StoreList panelType;

    [SerializeField] private ShopsCanvas shopsCanvas;

    private Toggle toggle;
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(ChangePanel);
    }
    public void ChangePanel(bool isOn)
    {
        if (isOn) { shopsCanvas.ChangeStorePanelAction?.Invoke(panelType); }
    }
}
