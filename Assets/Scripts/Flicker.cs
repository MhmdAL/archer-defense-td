using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[ExecuteInEditMode]
public class Flicker : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public float FlickerSpeed;
    public MinMaxCurve AlphaCurve;

    private void Update()
    {
        var t = (Mathf.Sin(Time.time * FlickerSpeed) + 1) / 2;

        var col = Renderer.color;
        col.a = AlphaCurve.curve.Evaluate(t);
        Renderer.color = col;
    }
}
