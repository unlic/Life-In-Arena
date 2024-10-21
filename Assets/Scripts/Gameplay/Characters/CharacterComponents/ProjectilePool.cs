using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab; 
    [SerializeField] private int poolSize = 10; 

    private Queue<Projectile> projectilePool;

    private void Awake()
    {
        InitializePool();
    }
    private void InitializePool()
    {
        projectilePool = new Queue<Projectile>();

        for (int i = 0; i < poolSize; i++)
        {
            Projectile projectile = Instantiate(projectilePrefab);
            projectile.gameObject.SetActive(false);
            projectilePool.Enqueue(projectile);
        }
    }
    public Projectile GetProjectile()
    {
        if (projectilePool.Count > 0)
        {
            Projectile projectile = projectilePool.Dequeue();
            projectile.gameObject.SetActive(true);
            return projectile;
        }
        else
        {
            Projectile newProjectile = Instantiate(projectilePrefab);
            return newProjectile;
        }
    }

    public void ReturnProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        projectilePool.Enqueue(projectile);
    }

    private void OnDestroy()
    {
        foreach (var item in projectilePool)
        {
            Destroy(item.gameObject);
        }
    }
}
