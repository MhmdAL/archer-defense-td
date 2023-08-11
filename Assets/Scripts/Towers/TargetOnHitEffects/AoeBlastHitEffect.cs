using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

[CreateAssetMenu]
public class AoeBlastHitEffect : TargetHitEffect
{
    [SerializeField]
    private GameObject BlastPrefab;

    [SerializeField]
    private float BlastDuration;

    public override void OnTargetHit(TargetHitData data)
    {
        var blast = Instantiate(BlastPrefab, data.HitPosition, Quaternion.identity);

        blast.transform.localScale = new Vector3(data.HitRadius * 2, data.HitRadius * 2, 1);

        blast.GetComponent<Blast>().AttachTimer(BlastDuration, (t) => Destroy(blast), (f =>
        {
            var alpha = 1 - (f / BlastDuration);

            var spirte = blast.GetComponent<SpriteRenderer>();

            var color = spirte.color;
            color.a = alpha;
            spirte.color = color;
        }));
    }
}