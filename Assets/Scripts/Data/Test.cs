using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform StartTransform;
    public Transform TargetTransform;

    public float Duration;
    public float Speed;
    public float Gravity;

    private float _t;

    private Vector3 Dir;

    private void Start()
    {
        transform.position = StartTransform.position;

        Dir = TargetTransform.position - StartTransform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _t = 0;
        }

        _t += Time.deltaTime;
        _t = Mathf.Clamp(_t, 0, Duration);

        var pos = transform.position;

        var xSpeed = Dir.x / Duration;
        var ySpeed = (Dir.y - 0.5f * Gravity * Duration * Duration) / Duration;

        pos.x = StartTransform.position.x + xSpeed * _t;
        pos.y = StartTransform.position.y + ySpeed * _t + 0.5f * Gravity * _t * _t;

        transform.position = pos;

        var targetDir = (Vector2)StartTransform.position + GetDeltaPos(xSpeed, ySpeed, _t + 0.1f * Duration) - (Vector2)pos;

        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Vector2 GetDeltaPos(float iXSpeed, float iYSpeed, float t)
    {
        var targetDirX = iXSpeed * t;
        var targetDirY = iYSpeed * t + 0.5f * Gravity * t * t;

        return new Vector2(targetDirX, targetDirY);
    }
}