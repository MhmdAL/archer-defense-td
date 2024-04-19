using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent<T> : ScriptableObject
{
    private Action<T> _event;

    public void Subscribe(Action<T> action)
    {
        _event += action;
    }

    public void Raise(T type)
    {
        _event?.Invoke(type);
    }
}
