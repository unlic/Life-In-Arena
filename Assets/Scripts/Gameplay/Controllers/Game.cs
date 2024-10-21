using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
    [SerializeField] private UIController ui;
    [SerializeField] private ShopsCanvas shopsCanvas;
    [SerializeField] private Hero heroPrefab;
    [SerializeField] private EnemysHolder enemys;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private List<Transform> enemySpawnPoints;

    [SerializeField] private int amountSpawnEnemy = 10;
    [SerializeField] private int spawnDelay = 100;
    [SerializeField] private int startCountGold = 550;
    [SerializeField] private int countAddPowerPoints = 3;

    private Hero hero;
    private int level = 1;
    private int amountDieEnemy = 0;
    private bool isSkipTime = false;

    public Hero Hero => hero;

    public void StartGame(CharacterSettings chooseCharacter)
    {
        SpawnHero(chooseCharacter, enemySpawnPoints[0]);

        ui.OnSkipTimer += SkipWaitingTime;
        shopsCanvas.OnBuyItem += EquipBuyItem;

        hero.OnLevelChange += HeroLevelUp;
        hero.OnGoldAdded += UserKillEnemy;

        StartCoroutine(SpawnEnemyDelay());
    }

    private void OnDestroy()
    {
        ui.OnSkipTimer -= SkipWaitingTime;
        shopsCanvas.OnBuyItem -= EquipBuyItem;

        if (hero != null)
        {
            hero.OnLevelChange -= HeroLevelUp;
            hero.OnGoldAdded -= UserKillEnemy;
        }
    }

    private void SkipWaitingTime()
    {
        isSkipTime = true;
        ui.SetTimeInFieldViewInSeconds(0);
        shopsCanvas.SetTimeInFieldViewInSeconds(0);
    }

    private IEnumerator SpawnEnemyDelay()
    {
        NewLevelLoad();

        if (!enemys.HasEnemiesForNextLevel(level))
        {
            GameOver();
            yield break;
        }

        int second = 0;
        ui.SetTimeInFieldViewInSeconds(spawnDelay);
        shopsCanvas.SetTimeInFieldViewInSeconds(spawnDelay);

        while (second < spawnDelay)
        {
            yield return new WaitForSeconds(1);

            if (isSkipTime)
            {
                break;
            }

            
            second++;

            int timeRemaining = spawnDelay - second;
            ui.SetTimeInFieldViewInSeconds(timeRemaining);
            shopsCanvas.SetTimeInFieldViewInSeconds(timeRemaining);
        }

        isSkipTime = false;
        SpawnEnemy();
    }

    private void HeroLevelUp(int level)
    {
        CurrencyManager.Instance.AddPowerPoints(countAddPowerPoints);
    }

    private void SpawnEnemy()
    {
        CharacterSettings enemySettings = enemys.GetEnemyByLevel(level);

        if (enemySettings == null)
        {
            GameOver();
            return;
        }

        int spawnPointsCount = enemySpawnPoints.Count;
        if (spawnPointsCount == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        int enemiesPerPoint = amountSpawnEnemy / spawnPointsCount;
        int extraEnemies = amountSpawnEnemy % spawnPointsCount;

        foreach (Transform spawnPoint in enemySpawnPoints)
        {
            SpawnEnemiesAtPoint(enemySettings, spawnPoint, enemiesPerPoint + (extraEnemies-- > 0 ? 1 : 0));
        }

        amountDieEnemy = 0;
    }

    private void SpawnHero(CharacterSettings heroSettings, Transform spawnPoint)
    {
        hero = Instantiate(heroPrefab, spawnPoint.position, spawnPoint.rotation);
        hero.InitCharacter(heroSettings);
    }

    private void SpawnEnemiesAtPoint(CharacterSettings enemySettings, Transform spawnPoint, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Enemy>();
            enemy.InitCharacter(enemySettings);
            enemy.OnDie += EnemyDie;
        }
    }

    private void EnemyDie()
    {
        amountDieEnemy++;
        if (amountDieEnemy >= amountSpawnEnemy)
        {
            level++;
            StartCoroutine(SpawnEnemyDelay());
        }
    }

    private void NewLevelLoad()
    {
        hero.Heal(hero.CurrentUserStats.MaxHealth);
        hero.ManaAdd(hero.CurrentUserStats.MaxMana);
        CurrencyManager.Instance.AddGold(startCountGold);
        CurrencyManager.Instance.AddPowerPoints(countAddPowerPoints);
    }

    private void GameOver()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void UserKillEnemy(int gold)
    {
        CurrencyManager.Instance.AddGold(gold);
    }

    private void EquipBuyItem(ItemObject item)
    {
        if (hero.HasInventorySpace())
        {
            hero.EquipItem(item);
        }
        else
        {
            CurrencyManager.Instance.AddGold(item.Price);
        }
    }

    private void UnequipItem(ItemObject item)
    {
        hero.UnequipItem(item);
    }
}
