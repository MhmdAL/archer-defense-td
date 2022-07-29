using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthy
{
    Action HealthChanged { get; set; }
    Stat MaxHP { get; }
    float CurrentHP { get; }
}