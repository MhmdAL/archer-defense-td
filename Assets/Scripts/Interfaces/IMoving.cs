using System.Collections.Generic;
using UnityEngine;

public interface IMoving
{
    Stat MoveSpeed { get; set; }

    void OnMovementStarted();
    void OnMovementEnded();
}