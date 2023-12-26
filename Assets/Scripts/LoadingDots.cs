using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class LoadingDots : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public AnimationCurve curve;
    public float period;
    private float _currentTime;
    private bool _downDirection;

    private void Update()
    {
        if (_downDirection)
        {
            _currentTime -= Time.deltaTime;
        }
        else
        {
            _currentTime += Time.deltaTime;
        }

        if (_downDirection && _currentTime < 0)
        {
            _currentTime = 0;
            _downDirection = false;
        }
        else if (!_downDirection && _currentTime > period)
        {
            _currentTime = period;
            _downDirection = true;
        }

        loadingText.text = GetDots();
    }

    private string GetDots()
    {
        var stage = (int)curve.Evaluate(_currentTime / period);

        // var stage = (_currentTime / period) switch
        // {
        //     >= 0.0f and < 0.25f => 0,
        //     >= 0.25f and < 0.5f => 1,
        //     >= 0.5f and < 0.75f => 2,
        //     >= 0.75f and <= 1.0f => 3,
        //     _ => 3
        // };

        return new string('.', stage);
    }
}
