using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityTimer;

public class LoadingScreen : MonoBehaviour
{
    public Animator animtor;

    public void FadeIn()
    {
        animtor.SetTrigger("fadein");
    }

    public void FadeOut()
    {
        animtor.SetTrigger("fadeout");
    }

    public void FadeInOut(float duration)
    {
        animtor.SetTrigger("fadein");

        this.AttachTimer(duration, (t) =>
        {
            animtor.SetTrigger("fadeout");
        });
    }
}
