using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : CharacterBase, IHasInventory, IExperienceReceiver, IGoldReceiver, IPowerPointReceiver
{
    [SerializeField] private Transform inventaryHolder;

    private float levelUpThreshold = 200;
    private float amountPointsPreviousLevel = 0;
    private float experiencePoints;
    private int unitLevel;
    private int usedLevelPoints = 0;
    private ItemManager itemManager;

    public event Action<float, float, float> OnExperiencePointsChange;
    public event Action<int> OnLevelChange;
    public event Action<int> OnGoldAdded;
    public event Action<int> OnPowerPointAdded;
    public event Action OnEquippedItemsChange;


    private const float STEP_TO_LEVEL_UP = 100;
    private const float FIRST_LEVEL_EXPERIENCE_POINTS = 200;
    public float ExperiencePoints
    {
        get => experiencePoints;
        private set
        {
            experiencePoints = Mathf.Max(value, 0);
            OnExperiencePointsChange?.Invoke(experiencePoints, levelUpThreshold, amountPointsPreviousLevel);
            while (experiencePoints >= levelUpThreshold)
            {
                LevelUp();
            }
        }
    }
    public int UnitLevel => unitLevel;
    public override void InitCharacter(CharacterSettings settings)
    {
        base.InitCharacter(settings);
        itemManager.OnEquippedItemsUpdate += EquippedItemsChanged;
    }
    public virtual void AddExperience(float amount)
    {
        ExperiencePoints += amount;
    }
    protected override void InitializeData()
    {
        base.InitializeData();
        unitLevel = 1;
        ExperiencePoints = 0;
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        itemManager = gameObject.AddComponent<ItemManager>();
    }
    protected override void SetupComponents()
    {
        base.SetupComponents();
        itemManager.SetUnitStats(CurrentUserStats, this);
    }

    public bool TryUseLevelPoint()
    {
        if (usedLevelPoints < unitLevel)
        {
            usedLevelPoints++;
            return true;
        }

        return false;
    }
    public bool HasUnusedLevelPoints() => usedLevelPoints < unitLevel;
    protected virtual void LevelUp()
    {
        unitLevel++;
        float currentExperienceToNextLevel = levelUpThreshold;
        levelUpThreshold += FIRST_LEVEL_EXPERIENCE_POINTS + STEP_TO_LEVEL_UP * (unitLevel - 1);
        amountPointsPreviousLevel = currentExperienceToNextLevel;
        ExperiencePoints = ExperiencePoints;
        CurrentUserStats.Strength += 2;
        CurrentUserStats.Agility += 2;
        CurrentUserStats.Intelligence += 2;
        CurrentUserStats.AttackPower = CalculateAttackPower();
        RecalculateStats();
        OnLevelChange?.Invoke(unitLevel);
    }

    protected override void ApplyAttackDamageEffects(CharacterBase currentCharacter, IDamageable target, float damage)
    {
        float criticalMultiplier = target is IImmuneToCriticalDamage ? 1 : CalculateCriticalMultiplier();

        IDamageEffect damageEffect = criticalMultiplier > 1 ? new CriticalDamageEffect(criticalMultiplier) : new HormalDamageEffect();
        IDamageEffect splashEffect = new SplashDamageEffect(CurrentUserStats.SplashRadius, CurrentUserStats.SplashDamage * criticalMultiplier);

        splashEffect.ApplyEffect(currentCharacter, target, damage);
        damageEffect.ApplyEffect(currentCharacter, target, damage);
    }
    public override void TakeData()
    {
        base.TakeData();
        OnExperiencePointsChange?.Invoke(experiencePoints, levelUpThreshold, amountPointsPreviousLevel);
    }
    public float CalculateCriticalMultiplier()
    {
        float finalCriticalMultiplier = 1.0f;
        float totalChance = 0;

        foreach (ItemObject itemObject in itemManager.GetEquippedItems())
        {
            if (itemObject != null)
            {
                var item = itemObject.Item;
                float rand = UnityEngine.Random.Range(0f, 100f);
                if (rand < item.CriticalChance)
                {
                    finalCriticalMultiplier = Mathf.Max(finalCriticalMultiplier, item.CriticalMultiplier);
                    totalChance += item.CriticalChance;
                }
            }
        }
        return finalCriticalMultiplier;
    }
    public void AddGold(int amount)
    {
        OnGoldAdded?.Invoke(amount);
    }
    public void AddPowerPoint(int amount)
    {
        OnPowerPointAdded?.Invoke(amount);
    }
    public void EquipItem(ItemObject item, int id = -1)
    {
        var equipItem = Instantiate(item, inventaryHolder);
        itemManager.EquipItem(equipItem, id);
        RecalculateStats();
    }
    public void EquipItemFronStorege(ItemObject item, int id)
    {
        itemManager.EquipItem(item, id);
        RecalculateStats();
    }
    public void UnequipItem(ItemObject item)
    {
        itemManager.UnequipItem(item);
        RecalculateStats();
    }    
    public List<ItemObject> GetEquippedItems()
    {
        return itemManager.GetEquippedItems();
    }
    public bool HasInventorySpace()
    {
        return itemManager.FindFirstEmptySlot() != -1;
    }
    public void SwapItems(int firstId, int secondId)
    {
        itemManager.ChangingItemsPlaces(firstId, secondId);
    }
    private void EquippedItemsChanged()
    {
        OnEquippedItemsChange?.Invoke();
    }
    protected override void OnDestroy()
    {
        itemManager.OnEquippedItemsUpdate -= EquippedItemsChanged;
        Destroy(itemManager);
        base.OnDestroy();
    }
}
