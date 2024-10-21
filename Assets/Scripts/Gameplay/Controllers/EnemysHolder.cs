using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemysHolder : MonoBehaviour
{
    [SerializeField] private List<CharacterSettings> enemiesPrefabs = new List<CharacterSettings>();
    public CharacterSettings GetEnemyByLevel(int level)
    {
        int id = level - 1;

        if (HasEnemiesForNextLevel(level))
            return enemiesPrefabs[id];

        return null;
    }

    public bool HasEnemiesForNextLevel(int level)
    {
        int id = level - 1;

        if (level > enemiesPrefabs.Count || id < 0)
            return false;

        return true;
    }


}
