using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInfoSet : MonoBehaviour
{
    [SerializeField] private ProjectilePool pool;
    [SerializeField] private Transform spawnProjectilePoint;
    public ProjectilePool GetInfoByProjectilePool()
    {
        return pool;
    }
    public Transform GetInfoBySpawnPoint()
    {
        return spawnProjectilePoint;
    }
}
