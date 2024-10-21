using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character", order = 51)]
public class CharacterSettings : ScriptableObject
{
    public Sprite Avatar;
    public GameObject AnimationObject;
    public float Strength;
    public float Agility;
    public float Intelligence;
    public float DefaultHealth;
    public float DefaultMana;
    public float DefaultMoveSpeed;
    public float DefaultAttackSpeed;
    public float DefaultAttackPower;
    public float DefaultDefense;
    public float AttackDistance;
    public AttackType AttackType;
    public DamageType DamageType;
    public CharacterClass CharacterClass;
    public List<Ability> CharacterAbilities;
}
