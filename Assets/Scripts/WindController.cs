using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class WindController : MonoBehaviour
{

    [MinMaxSlider(0.2f, 2f, true)]
    public Vector2 WindStrength;

    public Material WindMaterial;

    public float WindInterval = 10;

    private float _timer = 55;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > WindInterval)
        {
            _timer = 0;

            var windStrength = UnityEngine.Random.Range(WindStrength.x, WindStrength.y);

            WindMaterial.SetFloat("_WindStrength", windStrength);
        }
    }
}
