using System;
using System.Collections;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int Gold { get; private set; }
    public int PowerPoints { get; private set; }

    public Action<int> OnGoldChanged;
    public Action<int> OnPowerPointsChanged;

    private void Awake()
    {


        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Gold = 0;
            PowerPoints = 0;
        }
    }

    private void Start()
    {
        StartCoroutine(GoldGenerationRoutine());
    }

    private IEnumerator GoldGenerationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            AddGold(1);
        }
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }
        else
        {
            Debug.LogError("Not enough gold!");
            return false;
        }
    }

    public void AddPowerPoints(int amount)
    {
        PowerPoints += amount;
        OnPowerPointsChanged?.Invoke(PowerPoints);
    }

    public bool SpendPowerPoints(int amount)
    {
        if (PowerPoints >= amount)
        {
            PowerPoints -= amount;
            OnPowerPointsChanged?.Invoke(PowerPoints);
            return true;
        }
        else
        {
            Debug.LogError("Not enough power points!");
            return false;
        }
    }
}
