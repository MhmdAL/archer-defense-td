using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IShooter, IFocusable
{
    public Transform DetonationPoint;
    public float ExplosionRadius = 5;
    public float ExplosionDamage = 50;
    public GameObject ExplosionVFX;

    [SerializeField]
    private List<TargetHitEffect> onHitEffects;
    public List<TargetHitEffect> OnHitEffects => onHitEffects;

    public bool HasFocus => throw new System.NotImplementedException();

    public void OnTargetHit(Vector3 TargetPosition, List<Unit> unitsHit, Projectile p, int shotNumber)
    {
        foreach (var onhitEffect in OnHitEffects)
        {
            onhitEffect.OnTargetHit(new TargetHitData
            {
                Owner = this,
                Targets = unitsHit,
                HitRadius = ExplosionRadius,
                HitPosition = transform.position,
                Damage = ExplosionDamage,
                ArmorPen = 0
            });
        }
    }

    public void Focus()
    {
        Debug.Log("focuesd barrel");

        var cols = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);

        var unitsHit = new List<Unit>();

        foreach (var c in cols)
        {
            var u = c.GetComponent<Unit>();
            if (u != null)
            {
                unitsHit.Add(u);
            }
        }

        OnTargetHit(DetonationPoint.position, unitsHit, null, 0);

        var explosion = Instantiate(ExplosionVFX, DetonationPoint.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(ExplosionRadius, ExplosionRadius, 1);


        Destroy(gameObject, .5f);
    }

    public void UnFocus()
    {

    }
}
