using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
    public float LingerTime { get; set; } = 2f;

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
        if (_reached)
        {
            return;
        }

        if (other.TryGetComponent<IProjectileTarget>(out var target))
        {
            target.OnProjectileHit(this, other.ClosestPoint(transform.position));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_reached)
        {
            return;
        }

        if (other.collider.TryGetComponent<IProjectileTarget>(out var target))
        {
            target.OnProjectileHit(this, other.collider.ClosestPoint(transform.position));
        }
    }

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

    public static Projectile Fire(ProjectileSpawnData data)
    {
        var projectile = Instantiate(data.Prefab, data.SpawnPosition, Quaternion.identity);

        var dir = data.TargetPosition - projectile.transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

        projectile.Owner = data.Owner;
        projectile.StartPosition = data.SpawnPosition;
        projectile.TargetPosition = data.TargetPosition;

        projectile.Radius = data.Radius;
        projectile.Duration = data.Duration;

        projectile.LingerTime = data.LingerTime;

        projectile.Damage = data.Damage;
        projectile.ArmorPen = data.ArmorPen;

        return projectile;
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

public class ProjectileSpawnData
{
    public ProjectileSpawnData(IShooter owner, Projectile prefab, Vector3 spawnPosition, Vector3 targetPosition)
    {
        Owner = owner;
        Prefab = prefab;
        SpawnPosition = spawnPosition;
        TargetPosition = targetPosition;
    }

    public IShooter Owner { get; set; }
    public Projectile Prefab { get; set; }
    public Vector3 SpawnPosition { get; set; }
    public Vector3 TargetPosition { get; set; }

    public float LingerTime { get; set; }

    public float Damage { get; set; }
    public float ArmorPen { get; set; }
    public float Radius { get; set; }
    public float Duration { get; set; }
}