using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObstacle : MonoBehaviour, IDamageable, IImmuneToSpells, IImmuneToCriticalDamage
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    [SerializeField] HealsBar healsBar;

    private void Start()
    {
        healsBar.SetMaxHealth(maxHealth);
    }
    public int TakeDamage(float amount, IDamageable attacker, DamageType attackType)
    {
        health -= amount;

        if (health <= 0)
        {
            DestroyObstacle();
        }

        healsBar.ChangeFillAmount(health);

        return 0;
    }

    private void DestroyObstacle()
    {
        Destroy(gameObject);
    }
    public GameObject GetCurrentGameObject()
    {
        return gameObject;
    }
}
