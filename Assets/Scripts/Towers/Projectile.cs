using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Projectile : MonoBehaviour
{
    public IShooting Owner { get; set; }

    public float Damage { get; set; }
    public float Radius { get; set; }
    public float ArmorPen { get; set; }
    public int shotNumber;
    public bool isAboutToKill;

    public float Duration { get; set; } = 2f;
    public Vector3 StartPosition { get; set; }
    public Vector3 TargetPosition { get; set; }
    public float Gravity = -200;
    public float LingerTime { get; set; } = 2f;

    private float _curTime;

    private bool _reached;

    private void FixedUpdate()
    {
        if (_reached)
        {
            return;
        }

        MoveParabola();

        if (Vector3.Magnitude(transform.parent.position - TargetPosition) < .5f)
        {
            OnTargetHit(null, TargetPosition);
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
        if (_reached || !CanHitTarget)
        {
            return;
        }

        if (other.TryGetComponent<IProjectileTarget>(out var target))
        {
            target.OnProjectileHit(this, other.ClosestPoint(transform.parent.position));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_reached || !CanHitTarget)
        {
            return;
        }

        if (other.collider.TryGetComponent<IProjectileTarget>(out var target))
        {
            target.OnProjectileHit(this, other.collider.ClosestPoint(transform.parent.position));
        }
    }

    private bool CanHitTarget => _curTime / Duration >= 0.65f;

    public void OnTargetHit(IProjectileTarget target, Vector3 hitPosition)
    {
        var targets = new List<IProjectileTarget>();

        if (Radius > 0)
        {
            var cols = Physics2D.OverlapCircleAll(TargetPosition, Radius);

            foreach (var c in cols)
            {
                var u = c.GetComponent<IProjectileTarget>();
                if (u != null)
                {
                    targets.Add(u);
                }
            }
        }
        else if (target != null)
        {
            targets.Add(target);
        }

        if (targets.Any())
        {
            Owner.OnTargetHit(hitPosition, targets, this, shotNumber);

            if (target != null)
            {
                Destroy(transform.root.gameObject);
                _reached = true;
            }
            else
            {
                Destroy(transform.root.gameObject, LingerTime);
                _reached = true;
            }
        }
        else
        {
            Destroy(transform.root.gameObject, LingerTime);
            _reached = true;
        }
    }

    public static (Projectile, Vector2 dir) Fire(ProjectileSpawnData data)
    {
        var projectileObj = Instantiate(data.Prefab, data.SpawnPosition, Quaternion.identity);
        var projectile = projectileObj.GetComponentInChildren<Projectile>();

        var dir = data.TargetPosition - projectile.transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        projectileObj.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

        projectile.Owner = data.Owner;
        projectile.StartPosition = data.SpawnPosition;
        projectile.TargetPosition = data.TargetPosition;

        projectile.Radius = data.Radius;
        projectile.Duration = data.Duration;

        projectile.LingerTime = data.LingerTime;

        projectile.Damage = data.Damage;
        projectile.ArmorPen = data.ArmorPen;

        return (projectile, dir);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(TargetPosition, .25f);
        Gizmos.color = Color.white;
    }

    private void MoveParabola()
    {
        _curTime += Time.deltaTime;
        _curTime = Mathf.Clamp(_curTime, 0, Duration);

        var dir = TargetPosition - StartPosition;

        var iAngle = Mathf.Atan2(dir.y, dir.x);

        var gravityFactorX = 0;
        var gravityFactorZ = Mathf.Abs(Mathf.Cos(iAngle));

        var pos = transform.parent.position;


        var xSpeed = (dir.x - (0.5f * Gravity * Duration * Duration) * gravityFactorX) / Duration;
        var ySpeed = (dir.y - (0.5f * Gravity * Duration * Duration) * gravityFactorZ) / Duration;

        pos.x = StartPosition.x + xSpeed * _curTime + (0.5f * Gravity * _curTime * _curTime) * gravityFactorX;
        pos.y = StartPosition.y + ySpeed * _curTime + (0.5f * Gravity * _curTime * _curTime) * gravityFactorZ;

        transform.parent.position = pos;

        var targetDir = (Vector2)StartPosition + GetDeltaPos(xSpeed, ySpeed, _curTime + 0.1f * Duration, gravityFactorX, gravityFactorZ) - (Vector2)pos;

        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        transform.parent.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Vector2 GetDeltaPos(float iXSpeed, float iYSpeed, float t, float xFactor, float zFactor)
    {
        var x = iXSpeed * t + (0.5f * Gravity * t * t) * xFactor;
        var y = iYSpeed * t + (0.5f * Gravity * t * t) * zFactor;

        return new Vector2(x, y);
    }
}

public class ProjectileSpawnData
{
    public ProjectileSpawnData(IShooting owner, GameObject prefab, Vector3 spawnPosition, Vector3 targetPosition)
    {
        Owner = owner;
        Prefab = prefab;
        SpawnPosition = spawnPosition;
        TargetPosition = targetPosition;
    }

    public IShooting Owner { get; set; }
    public GameObject Prefab { get; set; }
    public Vector3 SpawnPosition { get; set; }
    public Vector3 TargetPosition { get; set; }

    public float LingerTime { get; set; }

    public float Damage { get; set; }
    public float ArmorPen { get; set; }
    public float Radius { get; set; }
    public float Duration { get; set; }
}