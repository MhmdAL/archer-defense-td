using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityTimer;

public class ExplosiveBarrel : MonoBehaviour, IAttacking, IFocusable, IProjectileTarget
{
    public Transform DetonationPoint;
    public float ExplosionRadius = 5;
    public float ExplosionDamage = 50;
    public GameObject ExplosionVFX;
    public AudioClip ExplosionSFX;

    [SerializeField]
    private GameObject blastCircle;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private List<TargetHitEffect> onHitEffects;
    public List<TargetHitEffect> OnHitEffects => onHitEffects;

    public bool HasFocus => throw new System.NotImplementedException();

    public void Detonate()
    {
        var cols = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);

        var unitsHit = new List<IProjectileTarget>();
        var barrelsHit = new List<ExplosiveBarrel>();

        foreach (var col in cols)
        {
            if (col.TryGetComponent<IProjectileTarget>(out var unit))
            {
                unitsHit.Add(unit);
            }

            if (col.TryGetComponent<ExplosiveBarrel>(out var barrel) && barrel != this)
            {
                barrelsHit.Add(barrel);
            }
        }

        foreach (var onhitEffect in OnHitEffects)
        {
            onhitEffect.OnTargetHit(new AttackData
            {
                Owner = this,
                Targets = unitsHit,
                HitRadius = ExplosionRadius,
                HitPosition = transform.position,
                Damage = ExplosionDamage,
                ArmorPen = 0
            });
        }

        var explosion = Instantiate(ExplosionVFX, DetonationPoint.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(ExplosionRadius, ExplosionRadius, 1);

        audioSource.PlayOneShot(ExplosionSFX, GlobalManager.GlobalVolumeScale);

        foreach (var barrel in barrelsHit)
        {
            barrel.AttachTimer(0.3f, x => barrel.Detonate());
        }

        blastCircle.SetActive(true);

        blastCircle.transform.DOScale(ExplosionRadius / 2f, 0.5f);

        Destroy(gameObject, .5f);
    }

    public void Focus()
    {
        Debug.Log("focuesd barrel");

        Detonate();
    }

    public void UnFocus()
    {

    }

    public void Highlight()
    {
        
    }

    public void UnHighlight()
    {

    }

    public void OnProjectileHit(Projectile p, Vector2 hitpoint)
    {
        if (p.Owner is CrossBow)
        {
            Detonate();

            Destroy(p.gameObject);
        }
    }
}
