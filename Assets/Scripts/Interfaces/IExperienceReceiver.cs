using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExperienceReceiver
{
    public int UnitLevel { get; }
    public event Action<float, float, float> OnExperiencePointsChange;
    public event Action<int> OnLevelChange;
    public void AddExperience(float amount);
}
