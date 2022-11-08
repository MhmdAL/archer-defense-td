using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IShooter
{
    void OnTargetHit(List<Unit> unitsHit, Projectile p, int shotNumber);
}

public class Projectile : MonoBehaviour
{
    public IShooter Owner { get; set; }

    public float Damage { get; set; }
    public float Radius { get; set; }
    public float ArmorPen { get; set; }
    public int shotNumber;
    public bool isAboutToKill;

    public float Duration { get; set; } = 2f;
    public Vector3 StartPosition { get; set; }
    public Vector3 TargetPosition { get; set; }
    public float Gravity { get; set; } = -175f;

    private float _curTime;

    private bool _reached;

    private void Update()
    {
        if (_reached)
        {
            return;
        }

        MoveParabola();

        if (Vector3.Magnitude(transform.position - TargetPosition) < .5f)
        {
            OnTargetHit(null);
        }
    }

    public void Reset(Vector2 startPos, Vector2 targetPos)
    {
        StartPosition = startPos;
        TargetPosition = targetPos;

        _curTime = 0;
        _reached = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_reached)
        {
            return;
        }
        
        var unit = other.GetComponent<Unit>();
        if(unit != null)
        {
            unit.OnProjectileHit(this, other.ClosestPoint(transform.position));
        }
    }

    public void OnTargetHit(Unit unit)
    {
        var targets = new List<Unit>();

        if (unit == null && Radius > 0)
        {
            var cols = Physics2D.OverlapCircleAll(TargetPosition, Radius);

            foreach (var c in cols)
            {
                var u = c.GetComponent<Unit>();
                if (unit != null)
                {
                    targets.Add(unit);
                }
            }
        }
        else if (unit != null)
        {
            targets.Add(unit);
        }

        if (targets.Any())
        {
            Owner.OnTargetHit(targets, this, shotNumber);
            Destroy(transform.root.gameObject);
        }
        else
        {
            Destroy(transform.root.gameObject, 60f);
            _reached = true;
        }
    }

    private void MoveParabola()
    {
        _curTime += Time.deltaTime;
        _curTime = Mathf.Clamp(_curTime, 0, Duration);

        var pos = transform.position;

        var dir = TargetPosition - StartPosition;

        var testDir = new Vector3(dir.magnitude, 0);

        var xSpeed = testDir.x / Duration;
        var ySpeed = (testDir.y - 0.5f * Gravity * Duration * Duration) / Duration;

        var targetDir = (Vector2)StartPosition + GetDeltaPos(xSpeed, ySpeed, _curTime + 0.1f * Duration) - (Vector2)pos;

        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        var horDisplacement = xSpeed * _curTime;
        var verDisplacement = ySpeed * _curTime + 0.5f * Gravity * _curTime * _curTime;

        pos.x = StartPosition.x + horDisplacement;
        pos.y = StartPosition.y + verDisplacement;

        transform.position = pos;

        // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Vector2 GetDeltaPos(float iXSpeed, float iYSpeed, float t)
    {
        var x = iXSpeed * t;
        var y = iYSpeed * t + 0.5f * Gravity * t * t;

        return new Vector2(x, y);
    }
}
