using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class Ability : MonoBehaviour
{
    public Sprite AbilityIcon;
    public string Name;
    public float Cooldown;
    public float ManaCost;
    public float CastDuration;
    public float Range;
    public int Level;
    public int MaxLevel;
    public bool IsPassiveAbility = false;
    public bool IsUltimate = false;
    public CharacterBase CurrentUser { get; protected set; }

    private float lastUseTime = -Mathf.Infinity;
    public event Action OnAbilityUsed;

    protected abstract void Activate(CharacterBase user, CharacterBase target = null, Vector3? area = null);

    public virtual void Deactivate()
    {
        return;
    }

    public bool IsReadyToUse()
    {
        return Time.time >= lastUseTime + Cooldown;
    }

    public void LevelUp(CharacterBase user)
    {
        CurrentUser = user;
        Deactivate();
        if (Level < MaxLevel)
        {
            Level++;
            UpdateAbilityStats();
        }
        if (IsPassiveAbility)
        {
            Activate(CurrentUser);
        }
    }

    public void UseAbility(CharacterBase user, CharacterBase target = null, Vector3? area = null)
    {
        if (!IsReadyToUse())
        {
            Debug.Log("Способность еще не готова.");
            return;
        }

        if (user.CurrentUserStats.Mana < ManaCost)
        {
            Debug.Log("Недостаточно маны.");
            return;
        }
           
        Activate(user, target, area);
    }
    protected void UpdateUseTime()
    {
        lastUseTime = Time.time;
    }

    protected abstract void UpdateAbilityStats();

    public abstract string GetAbilityDescription();

    public float GetTimeSinceLastUse()
    {
        return Mathf.Max(0, Cooldown - (Time.time - lastUseTime));
    }
}
