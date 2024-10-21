using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealsBar : MonoBehaviour
{
    [SerializeField] private GameObject bar;
    [SerializeField] private Image fill;

    private float maxHealth;
    private bool showIfFullHP;
    private Camera mainCamera;
   

    private void Start()
    {
        fill.fillAmount = 1;
        mainCamera = Camera.main;    
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }
    public void SetMaxHealth(float maxHealth, bool show = false)
    {
        this.maxHealth = maxHealth;
        bar.SetActive(showIfFullHP);
        showIfFullHP = show;
    }
    public void ChangeFillAmount(float value, float max)
    {
        fill.fillAmount = value/ max;
        maxHealth = max;

        if (max == value)
        {
            bar.SetActive(showIfFullHP);
        }
        else
        {
            bar.SetActive(true);
        }
        
    }

    public void ChangeFillAmount(float value)
    {
        fill.fillAmount = value / maxHealth;

        if (maxHealth == value)
        {
            bar.SetActive(showIfFullHP);
        }
        else
        {
            bar.SetActive(true);
        }

    }
}
