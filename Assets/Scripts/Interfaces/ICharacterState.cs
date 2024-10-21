using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterState
{
    public void Enter();
    public void Execute();
    public void Exit();
    public void SetEvents(Action onMove, Action<IDamageable, float> onAttack, Action onIdle, Action onDie, Action<int> onAbilityActivate, Action<float> OnStaned);
}
