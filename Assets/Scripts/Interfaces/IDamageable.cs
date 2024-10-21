using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    public int TakeDamage(float amount, IDamageable attacker = null, DamageType attackType = DamageType.Physical);

}
