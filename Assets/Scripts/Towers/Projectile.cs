using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IShooter
{
    void OnTargetHit(List<Unit> unitsHit, Projectile p, int shotNumber);
}

public class Projectile : MonoBehaviour
{
    public IShooter Owner { get; set; }
    public Unit Target { get; set; }


    public float Damage { get; set; }
    public float Radius { get; set; }
    public float ArmorPen { get; set; }
    public int shotNumber;
    public bool isAboutToKill;

    public float Duration { get; set; } = 2f;
    public Vector3 StartPosition { get; set; }
    public Vector3 TargetPosition { get; set; }
    public float Gravity { get; set; } = -50f;

    private float _curTime;

    private bool _reached;

    private void FixedUpdate()
    {
        if(_reached)
        {
            return;
        }
        
        MoveParabola();

        if (_curTime >= Duration)
        {
            OnBulletHit();

            if (Target != null)
            {
                Destroy(transform.root.gameObject);
            }
            else
            {
                Destroy(transform.root.gameObject, 3f);
                _reached = true;
            }
        }
    }

    public void OnBulletHit()
    {
        if (Radius > 0)
        {
            var targets = new List<Unit>();

            var cols = Physics2D.OverlapCircleAll(TargetPosition, Radius);

            foreach (var c in cols)
            {
                var unit = c.GetComponent<Unit>();
                if (unit != null)
                {
                    targets.Add(unit);
                }
            }

            if (Owner != null)
            {
                Owner.OnTargetHit(targets, this, shotNumber);
            }
        }
        else
        {
            if (Owner != null)
            {
                Owner.OnTargetHit(new List<Unit> { Target }, this, shotNumber);
            }
        }
    }

    private void MoveParabola()
    {
        _curTime += Time.deltaTime;
        _curTime = Mathf.Clamp(_curTime, 0, Duration);

        var pos = transform.position;

        var dir = TargetPosition - StartPosition;

        var xSpeed = dir.x / Duration;
        var ySpeed = (dir.y - 0.5f * Gravity * Duration * Duration) / Duration;

        pos.x = StartPosition.x + xSpeed * _curTime;
        pos.y = StartPosition.y + ySpeed * _curTime + 0.5f * Gravity * _curTime * _curTime;

        transform.position = pos;

        var targetDir = (Vector2)StartPosition + GetDeltaPos(xSpeed, ySpeed, _curTime + 0.1f * Duration) - (Vector2)pos;

        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Vector2 GetDeltaPos(float iXSpeed, float iYSpeed, float t)
    {
        var x = iXSpeed * t;
        var y = iYSpeed * t + 0.5f * Gravity * t * t;

        return new Vector2(x, y);
    }
}
