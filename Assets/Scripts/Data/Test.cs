using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour, IShooter
{
    public GameObject ProjectilePrefab;
    public Transform TargetTransform;

    public float Duration;
    public float Gravity;

    public float FireInterval = 2f;

    private float _t;

    public List<TargetHitEffect> OnHitEffects { get; set; }

    private void Update()
    {
        _t += Time.deltaTime;
        if (_t >= FireInterval)
        {
            _t = 0;

            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        var pref = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
        var proj = pref.GetComponentInChildren<Projectile>();

        proj.Duration = Duration;
        proj.Gravity = Gravity;
        proj.StartPosition = transform.position;
        proj.TargetPosition = TargetTransform.position;
        proj.Owner = this;
    }

    public void OnTargetHit(Vector3 targetPosition, List<IProjectileTarget> unitsHit, Projectile p, int shotNumber)
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(transform.position, 1);

        Gizmos.color = Color.green;

        Gizmos.DrawSphere(TargetTransform.position, 1);
    }
}