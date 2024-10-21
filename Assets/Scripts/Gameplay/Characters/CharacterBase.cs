using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
public abstract class CharacterBase : MonoBehaviour, IDamageable, ISplashDamageable, ITeam
{
    [SerializeField] private CharacterSettings unitSettings;
    [SerializeField] private int teamID;
    [SerializeField] private HealsBar healsBar;
    [SerializeField] private List<ItemObject> items;
    [SerializeField] private DamageText damageTextPrefab;
    [SerializeField] private Transform spawnAnimationObject;
    [SerializeField] private Transform abilityHolder;

    public CharacterState CharacterState;
    public ICharacterState currentState;

    private ProjectilePool projectilePool;
    private Transform projectileSpawnPoint;
    private RegenerationComponent regenerationComponent;
    private DeadObject deadObject;
    private Camera mainCamera;
    public IDamageable CurrentTarget { get; set; }
    private bool hasReceivedNewCommand = false;
    int selectedIndexAbility;
    private float lastAttackTime = 0;
    protected List<CharacterBase> attackers = new List<CharacterBase>();
    public CharacterStats CurrentUserStats { get; private set; }
    public List<Ability> Abilities = new List<Ability>();
    public int TeamId { get => teamID; set => teamID = value; }
    public bool IsMovingToPosition { get; set; }
    public bool IsMovingToEnemy { get; set; }
    public bool IsDie { get; protected set; }
    public bool IsCastingAbility { get; set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public TargetManager TargetManager { get; private set; }

    public event Action OnMove;
    public Action<float> OnTakeDamage;
    public event Action<IDamageable, float> OnAttack;
    public event Action OnIdle;
    public event Action OnDie;
    public event Action<float> OnStaned;
    public event Action<int> OnAbilityActivate;

    private const float HEALTH_MULTIPLIER = 12f;
    private const float MANA_MULTIPLIER = 8f;
    private const float DEFENSE_MULTIPLIER = 0.1f;
    private const float REGENERATION_MULTIPLIER = 0.2f;
    private void OnEnable()
    {
        CharacterManager.Instance.RegisterCharacter(this);
    }
    protected virtual void Awake()
    {
        mainCamera = Camera.main;
       
    }
    protected virtual void Update()
    {
        currentState?.Execute();
    }
    public virtual void InitCharacter(CharacterSettings settings)
    {
        unitSettings = settings;
        InitializeComponents();
        InitializeData();
        SetupComponents();
        mainCamera = Camera.main;
        CurrentUserStats.HealthChangeAction += HealthChange;
        ChangeState(new HoldState(this));
    }
    protected virtual void InitializeComponents()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        regenerationComponent = gameObject.AddComponent<RegenerationComponent>();
        deadObject = gameObject.AddComponent<DeadObject>();
        TargetManager = gameObject.AddComponent<TargetManager>();

    }
    protected virtual void SetupComponents()
    {
        
        regenerationComponent.StartRegeneration(CurrentUserStats);
        healsBar.SetMaxHealth(CurrentUserStats.MaxHealth);
        TargetManager.CurrentCharacter = this;
    }
    protected virtual void InitializeData()
    {
        CurrentUserStats = new CharacterStats(unitSettings, HEALTH_MULTIPLIER, MANA_MULTIPLIER, DEFENSE_MULTIPLIER);

        InitializeAbilities();

        if (unitSettings.AnimationObject != null)
        {
            var animationObject = Instantiate(unitSettings.AnimationObject, spawnAnimationObject.transform.position, spawnAnimationObject.transform.rotation);
            animationObject.transform.SetParent(transform);
            gameObject.GetComponent<CharacterAnimation>().SetAnimator(animationObject.GetComponent<Animator>());
            if (CurrentUserStats.AttackType == AttackType.Ranged)
            {
                projectilePool = animationObject.GetComponent<ProjectileInfoSet>().GetInfoByProjectilePool();
                projectileSpawnPoint = animationObject.GetComponent<ProjectileInfoSet>().GetInfoBySpawnPoint();
            }
        }

        CurrentUserStats.DefaultAttackPower = unitSettings.DefaultAttackPower - CalculateAttackPower();

        RecalculateStats();
        CurrentUserStats.Health = CurrentUserStats.MaxHealth;
        CurrentUserStats.Mana = CurrentUserStats.MaxMana;
    }
    protected virtual void RecalculateStats()
    {
        float oldMaxHealth = CurrentUserStats.MaxHealth;
        float oldMaxMana = CurrentUserStats.MaxMana;

        CurrentUserStats.MaxHealth = (CurrentUserStats.Strength + CurrentUserStats.StrengthBonus) * HEALTH_MULTIPLIER + CurrentUserStats.HealthBonus;
        CurrentUserStats.HealthRegeneration = (CurrentUserStats.Strength + CurrentUserStats.StrengthBonus) * REGENERATION_MULTIPLIER + CurrentUserStats.HealthRegenerationBonus;
        CurrentUserStats.MaxMana = (CurrentUserStats.Intelligence + CurrentUserStats.IntelligenceBonus) * MANA_MULTIPLIER + CurrentUserStats.ManaBonus;
        CurrentUserStats.ManaRegeneration = (CurrentUserStats.Intelligence + CurrentUserStats.IntelligenceBonus) * REGENERATION_MULTIPLIER;
        CurrentUserStats.Defense = (CurrentUserStats.Agility + CurrentUserStats.AgilityBonus) * DEFENSE_MULTIPLIER;
        CurrentUserStats.AttackPower = CalculateAttackPower();
        NavMeshAgent.speed = CurrentUserStats.MoveSpeed;
        ApplyHealthAndManaChanges(oldMaxHealth, oldMaxMana);
    }
    private void InitializeAbilities()
    {
        if (unitSettings.CharacterAbilities == null) return;

        foreach (var ability in unitSettings.CharacterAbilities)
        {
            Abilities.Add(Instantiate(ability, abilityHolder));
        }
    }
    private void ApplyHealthAndManaChanges(float oldMaxHealth, float oldMaxMana)
    {
        float healthDelta = CurrentUserStats.MaxHealth - oldMaxHealth;
        if (healthDelta > 0) Heal(healthDelta);

        float manaDelta = CurrentUserStats.MaxMana - oldMaxMana;
        if (manaDelta > 0) ManaAdd(manaDelta);
    }
    public void ChangeState(ICharacterState newState)
    {
        if (currentState != null)
        {
            if (currentState is StanState stanState && stanState.IsStunned && newState is not StanState)
            {
                return; 
            }

            if (IsCastingAbility)
            {
                return;
            }
            currentState.Exit();
        }
        currentState = newState;
        currentState.SetEvents(OnMove, OnAttack, OnIdle, OnDie, OnAbilityActivate, OnStaned);
        currentState.Enter();
    }

    public virtual int TakeDamage(float damage, IDamageable attacker, DamageType attackType)
    {
        if (!attackers.Contains(attacker))
        {
            attackers.Add(attacker as CharacterBase);
        }

        if (HandleDodge())
            return 0;

        float effectiveDamage = attackType == DamageType.Magic ? CurrentUserStats.TakeReflectMagicDamage(damage) : CurrentUserStats.TakeReflectDamage(damage);

        float stealHP = Mathf.Clamp(effectiveDamage, 0, CurrentUserStats.Health);

        ApplyDefenceDamageEffects(this, attacker, damage);

        CurrentUserStats.Health -= effectiveDamage;
        OnTakeDamage?.Invoke(effectiveDamage);
        return HandleDeathOrDamageReturn(attacker, effectiveDamage);
    }

    private int HandleDeathOrDamageReturn(IDamageable attacker, float effectiveDamage)
    {
        if (CurrentUserStats.Health <= 0)
        {
            Die();
            return Mathf.RoundToInt(Mathf.Clamp(effectiveDamage, 0, CurrentUserStats.Health));
        }
        if(CurrentTarget != null)
        {
            TargetManager.CurrentTarget = CurrentTarget;
            return Mathf.RoundToInt(Mathf.Clamp(effectiveDamage, 0, CurrentUserStats.Health));
        }
        TargetManager.TargetSearchProcessing(attacker, attackers);
        return Mathf.RoundToInt(Mathf.Clamp(effectiveDamage, 0, CurrentUserStats.Health));
    }
    protected bool HandleDodge()
    {
        float totalDodgeChance = CalculateTotalDodgeChance();

        if (UnityEngine.Random.Range(0f, 100f) < totalDodgeChance)
        {
            return true;
        }
        return false;
    }
    private void HealthChange(float amount, float maxHealth)
    {
        healsBar.ChangeFillAmount(amount, maxHealth);
    }


    protected virtual void PerformAttack(IDamageable target)
    {
        float attackTime = CurrentUserStats.CalculateAttackTime();
        RotateTowardsTarget(target);
        if (Time.time - lastAttackTime >= attackTime && IsWithinAttackRange(target))
        {
            OnAttack?.Invoke(target, attackTime);
            StartCoroutine(CurrentUserStats.AttackType == AttackType.Ranged
                ? FireProjectile(target, attackTime)
                : DealDamage(target, CurrentUserStats.AttackPower + CurrentUserStats.AttackBonus, attackTime / 4));
            lastAttackTime = Time.time;
        }
    }
    private void RotateTowardsTarget(IDamageable target)
    {
        Vector3 directionToTarget = ((MonoBehaviour)target).transform.position - transform.position;
        directionToTarget.y = 0;
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    private IEnumerator FireProjectile(IDamageable target, float attackTime)
    {
        yield return new WaitForSeconds(attackTime/5);

        var projectile = projectilePool.GetProjectile();
        projectile.transform.position = projectileSpawnPoint.position;
        projectile.transform.rotation = projectileSpawnPoint.rotation;
        projectile.SetTarget((MonoBehaviour)target, CurrentUserStats.AttackPower + CurrentUserStats.AttackBonus, this, projectilePool);
    }



    public List<Ability> GetUserAbilitys()
    {
        return Abilities;
    }
    protected virtual float CalculateAttackPower()
    {
        switch(CurrentUserStats.CharacterClass)
        {
            case CharacterClass.Warrior:
                return (CurrentUserStats.StrengthBonus + CurrentUserStats.Strength) * 2f;            
            case CharacterClass.Assassin:
                return (CurrentUserStats.AgilityBonus + CurrentUserStats.Agility) * 2f;            
            case CharacterClass.Mage:
                return (CurrentUserStats.IntelligenceBonus + CurrentUserStats.Intelligence) * 2f;
            default:
                 return (CurrentUserStats.StrengthBonus + CurrentUserStats.Strength) * 2f;
        }
    }

    public virtual void Attack(IDamageable target)
    {
        if (IsDie || IsCastingAbility || ((MonoBehaviour)target) == this || (target is ITeam unit && unit.TeamId == TeamId))
            return;

        CurrentTarget = target;
        PerformAttack(target);
    }

    protected virtual void ApplyAttackDamageEffects(CharacterBase currentCharacter, IDamageable target, float damage)
    {
        IDamageEffect damageEffect = new HormalDamageEffect();
        IDamageEffect splashEffect = new SplashDamageEffect(CurrentUserStats.SplashRadius, CurrentUserStats.SplashDamage);

        splashEffect.ApplyEffect(currentCharacter, target, damage);
        damageEffect.ApplyEffect(currentCharacter, target, damage);

    }
    protected void ApplyDefenceDamageEffects(CharacterBase currentCharacter, IDamageable target, float damage)
    {
        IDamageEffect returnedEffect = new ReturnedDamageEffect(CurrentUserStats.ReflectDamagePercentage, CurrentUserStats.ReflectDamageAmountBonus);

        returnedEffect.ApplyEffect(currentCharacter, target, damage);
    }
    protected IEnumerator DealDamage(IDamageable target, float damage, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (IsDie)
            yield break;
        ApplyAttackDamageEffects(this, target, damage);
    }

    public int TakeSplashDamage(float amount, CharacterBase attacker)
    {
        return TakeDamage(amount, attacker, DamageType.Physical);
    }
    public bool HasReceivedNewCommand()
    {
        bool commandReceived = hasReceivedNewCommand;
        hasReceivedNewCommand = false;
        return commandReceived;
    }

    public void SetHasReceivedNewCommand(bool state)
    {
        hasReceivedNewCommand = state;
    }

    protected float CalculateTotalDodgeChance()
    {
        float dodgeChanceCoefficient = 0.25f;
        return CurrentUserStats.DodgeChance + CurrentUserStats.Agility * dodgeChanceCoefficient;
    }
    public virtual void UseMana(float amount)
    {
        CurrentUserStats.Mana -= amount;
    }
    public virtual void ClearAttackTarget()
    {
        CurrentTarget = null;
    }
    public virtual void Heal(float amount)
    {
        CurrentUserStats.Health += amount;
    }
    public virtual void ManaAdd(float amount)
    {
        CurrentUserStats.Mana += amount;
    }

    private void ShowCriticalDamageText(float damage)
    {
        DamageText damageTexts = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, transform);
        damageTexts.SetDamageText(damage);
    }

    protected virtual void Die()
    {
        IsDie = true;
        OnDie?.Invoke();
        deadObject.Die();
        Destroy(this);
    }
    public void SetUnitState(CharacterState state)
    {
        CharacterState = state;
    }
    public void SetTeam(int newTeamID)
    {
        TeamId = newTeamID;
    }
    public virtual void TakeData()
    {
        CurrentUserStats.HealthChangeAction.Invoke(CurrentUserStats.Health, CurrentUserStats.MaxHealth);
        CurrentUserStats.ManaChangeAction.Invoke(CurrentUserStats.Mana, CurrentUserStats.MaxMana);
    }
    public bool IsWithinAttackRange(IDamageable target)
    {
        return Vector3.Distance(transform.position, ((MonoBehaviour)target).transform.position) <= CurrentUserStats.AttackDistance;
    }

    private void OnDisable()
    {
        CharacterManager.Instance.UnregisterCharacter(this);
    }
    protected virtual void OnDestroy()
    {
        CurrentUserStats.HealthChangeAction -= HealthChange;
        Destroy(regenerationComponent);
        Destroy(NavMeshAgent);
        Destroy(TargetManager);
        Destroy(healsBar.gameObject);
    }
}
