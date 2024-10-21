using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    private List<CharacterBase> allCharacters = new List<CharacterBase>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterCharacter(CharacterBase character)
    {
        if (!allCharacters.Contains(character))
        {
            allCharacters.Add(character);
        }
    }

    public void UnregisterCharacter(CharacterBase character)
    {
        if (allCharacters.Contains(character))
        {
            allCharacters.Remove(character);
        }
    }

    public List<CharacterBase> GetEnemiesInRange(CharacterBase source, float range)
    {
        List<CharacterBase> enemiesInRange = new List<CharacterBase>();
        foreach (CharacterBase character in allCharacters)
        {
            if (character.TeamId != source.TeamId)
            {
                float distance = Vector3.Distance(source.transform.position, character.transform.position);
                if (distance <= range)
                {
                    enemiesInRange.Add(character);
                }
            }
        }
        return enemiesInRange;
    } 
    
    public List<CharacterBase> GetAlliesInRange(CharacterBase source, float range)
    {
        List<CharacterBase> enemiesInRange = new List<CharacterBase>();
        foreach (CharacterBase character in allCharacters)
        {
            if (character.TeamId == source.TeamId)
            {
                float distance = Vector3.Distance(source.transform.position, character.transform.position);
                if (distance <= range)
                {
                    enemiesInRange.Add(character);
                }
            }
        }
        return enemiesInRange;
    }
    public List<CharacterBase> GetEnemiesInAria(CharacterBase source, Vector3 area, float range)
    {
        List<CharacterBase> enemiesInRange = new List<CharacterBase>();
        foreach (CharacterBase character in allCharacters)
        {
            if (character.TeamId != source.TeamId)
            {
                float distance = Vector3.Distance(area, character.transform.position);
                if (distance <= range)
                {
                    enemiesInRange.Add(character);
                }
            }
        }
        return enemiesInRange;
    }
    public List<CharacterBase> GetEnemies(CharacterBase source)
    {
        List<CharacterBase> enemiesInRange = new List<CharacterBase>();
        foreach (CharacterBase character in allCharacters)
        {
            if (character.TeamId != source.TeamId)
            {
                enemiesInRange.Add(character);
            }
        }
        return enemiesInRange;
    }

}
