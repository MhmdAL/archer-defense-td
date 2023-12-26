using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Monster))]
public class SpriteByHealth : MonoBehaviour
{
    public List<SpriteByHealthConfig> spriteByHealthConfigs;

    private Monster _target;


    private void Awake()
    {
        _target = GetComponent<Monster>();

        _target.HealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged()
    {

    }

    private void LateUpdate()
    {
        var percentHealth = _target.CurrentHP / _target.MaxHP.Value;

        var stage = percentHealth >= 0.66f ? 0 : percentHealth >= 0.33f ? 1 : 2;

        foreach (var config in spriteByHealthConfigs)
        {
            if (config.sprites.Any())
            {
                config.spriteRenderer.sprite = config.sprites[Mathf.Min(config.sprites.Count - 1, stage)];
            }

            if (config.transforms.Any())
            {
                var transformChange = config.transforms[Mathf.Min(config.transforms.Count - 1, stage)];

                if (transformChange.changeRotation)
                {
                    config.spriteRenderer.transform.localRotation = Quaternion.AngleAxis(transformChange.rotation, Vector3.forward);
                }

                if (transformChange.changePosition)
                {
                    config.spriteRenderer.transform.localPosition = transformChange.position;
                }
            }
        }
    }
}

[Serializable]
public class SpriteByHealthConfig
{
    public SpriteRenderer spriteRenderer;
    public List<Sprite> sprites;
    public List<TransformChange> transforms;
}

[Serializable]
public class TransformChange
{
    public bool changeRotation;
    public float rotation;

    public bool changePosition;
    public Vector3 position;
}