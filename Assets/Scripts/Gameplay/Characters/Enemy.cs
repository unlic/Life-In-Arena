using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Enemy : CharacterBase
{
    [SerializeField] private int experienceToGiveMultiplier;
    [SerializeField] private int goldRewardForKill;
    private int experienceToGive = 20;
    private float radius = 12f;
    private Vector3 deathPosition;
    private int lastHitUnitTeamId;

    public Action<IDamageable> EnemUnitTakeDamage;

    private List<CharacterBase> currentAttackTargets;

    public void SetAttackTargets(List<CharacterBase> targets)
    {
        currentAttackTargets = targets;
    }

    public override int TakeDamage(float damage, IDamageable attacker, DamageType attackType)
    {
        if (IsDie)
            return 0;

        EnemUnitTakeDamage?.Invoke(attacker);

        if (attacker != null)
            RefreshAttackTarget(attacker);

        if (!attackers.Contains(attacker))
        {
            attackers.Add(attacker as CharacterBase);
        }

        if (HandleDodge())
            return 0;

        float effectiveDamage = CurrentUserStats.TakeReflectDamage(damage);

        if (attackType == DamageType.Magic)
        {
            effectiveDamage = CurrentUserStats.TakeReflectMagicDamage(damage);
        }

        float stealHP = Mathf.Clamp(effectiveDamage, 0, CurrentUserStats.Health);

        ApplyDefenceDamageEffects(this, attacker, damage);

        OnTakeDamage?.Invoke(damage);

        CurrentUserStats.Health -= effectiveDamage;

        if (CurrentUserStats.Health <= 0)
        {
            deathPosition = transform.position;
            lastHitUnitTeamId = (attacker as CharacterBase).TeamId;
            Die();
            return Mathf.RoundToInt(stealHP);
        }

        if (CurrentTarget == null)
        {
            CurrentTarget = attacker;
            if (NavMeshAgent != null && NavMeshAgent.isOnNavMesh)
            {
                ChangeState(new MovingState(this, Vector3.zero, CurrentTarget));
                IsMovingToPosition = true;
            }
        }
        else if (CurrentTarget == attacker)
        {
            float distance = Vector3.Distance(transform.position, ((MonoBehaviour)CurrentTarget).transform.position);
            if (distance <= CurrentUserStats.AttackDistance) 
            {
                ChangeState(new AttackState(this, CurrentTarget));
            }
            else if (NavMeshAgent != null && NavMeshAgent.isOnNavMesh)
            {
                ChangeState(new MovingState(this, Vector3.zero, CurrentTarget));
            }
        }

        return Mathf.RoundToInt(stealHP);
    }
    protected override void InitializeData()
    {
        base.InitializeData();
    }

    private void RefreshAttackTarget(IDamageable attacker)
    {
        if (IsDie)
            return;

        Vector3 currentPosition = transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(currentPosition, radius);

        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.EnemUnitTakeDamage?.Invoke(attacker);
            }
        }
    }

    private void GiveExperienceToNearbyUnits()
    {
        experienceToGive *= experienceToGiveMultiplier;

        var enemeis = CharacterManager.Instance.GetEnemiesInRange(this, radius);

        List<IExperienceReceiver> experienceReceiversList = new List<IExperienceReceiver>();

        foreach (CharacterBase eneme in enemeis)
        {
            IExperienceReceiver experienceReceiver = eneme as IExperienceReceiver;
            if (experienceReceiver != null)
            {
                experienceReceiversList.Add(experienceReceiver);
            }
        }

        if (experienceReceiversList.Count > 0)
        {
            experienceToGive /= experienceReceiversList.Count;

            foreach (IExperienceReceiver experienceReceiver in experienceReceiversList)
            {
                experienceReceiver.AddExperience(experienceToGive);
            }
        }
    }

    private void GiveGoldToUser()
    {
        var enemeis = CharacterManager.Instance.GetEnemies(this);
        List<Hero> goldReceiversList = new List<Hero>();

        foreach (CharacterBase eneme in enemeis)
        {
            Hero goldReceiver = eneme as Hero;

            if (goldReceiver != null && lastHitUnitTeamId == goldReceiver.TeamId)
            {
                goldReceiver.AddGold(goldRewardForKill);
                return;
            }
        }
    }

    protected override void Die()
    {
        GiveExperienceToNearbyUnits();
        GiveGoldToUser();
        base.Die();
    }
}
