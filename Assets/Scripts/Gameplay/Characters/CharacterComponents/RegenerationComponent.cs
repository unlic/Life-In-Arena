using System.Collections;
using UnityEngine;

public class RegenerationComponent : MonoBehaviour
{
    private CharacterStats unitStats;
    public void StartRegeneration(CharacterStats stats)
    {
        unitStats = stats;
        StartCoroutine(RegenerateHealth());
        StartCoroutine(RegenerateMana());
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            unitStats.Health += unitStats.HealthRegeneration;
        }
    }

    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            unitStats.Mana += unitStats.ManaRegeneration;
        }
    }
}
