using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaterEffects : MonoBehaviour
{
    [SerializeField] private GameObject levelUpEffect;
    [SerializeField] private GameObject bloodExplosion;
    [SerializeField] private GameObject stanEffect;

    private CharacterBase character;
    GameObject stunEffect;
    private void Start()
    {
        character = GetComponent<CharacterBase>();

        if (character as Hero)
            (character as Hero).OnLevelChange += LevelUp;
        character.OnTakeDamage += TakeDamage;
        character.OnStaned += StanEffect;

        character.OnDie += CharaterDie;
    }
    private void LevelUp(int level)
    {
        GameObject effect = Instantiate(levelUpEffect, character.transform.position, Quaternion.identity);

        Destroy(effect, 2);
    }

    private void TakeDamage(float damage)
    {
        GameObject effect = Instantiate(bloodExplosion, character.transform.position, Quaternion.identity);

        Destroy(effect, 0.5f);

    }
    private void StanEffect(float duration)
    {
        Destroy(stunEffect);

        stunEffect = Instantiate(stanEffect, new Vector3(character.transform.position.x, 2.9f, character.transform.position.z), Quaternion.identity);

        Destroy(stunEffect, duration);
    }
    private void CharaterDie()
    {
        Destroy(stunEffect);
        Destroy(this);
    }
    private void OnDestroy()
    {
        if (character as Hero)
            (character as Hero).OnLevelChange -= LevelUp;
        character.OnTakeDamage -= TakeDamage;
        character.OnDie -= CharaterDie;
        character.OnStaned -= StanEffect;
    }
}
