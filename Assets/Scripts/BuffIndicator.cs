using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuffIndicator : MonoBehaviour
{

    [HideInInspector] public BuffIndicatorPanel owner;

    [HideInInspector] public BuffIndicatorType type;

    [HideInInspector] public CooldownTimer cd;

    private bool permanent = false;

    public Image indicatorImage;
    public Image cooldownImage;

    public Sprite atkRangeIndicator, atkDmgIndicator, atkSpdIndicator;

    void Start()
    {
        if (type == BuffIndicatorType.ATTACK_DAMAGE)
        {
            indicatorImage.sprite = atkDmgIndicator;
        }
        else if (type == BuffIndicatorType.ATTACK_RANGE)
        {
            indicatorImage.sprite = atkRangeIndicator;
        }
        else if (type == BuffIndicatorType.ATTACK_SPEED)
        {
            indicatorImage.sprite = atkSpdIndicator;
        }

        if (cd == null)
        {
            permanent = true;
            cooldownImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!permanent)
        {
            if (cd.GetCooldownRemaining() <= 0)
            {
                owner.indicators.Remove(this);

                Destroy(gameObject);
                return;
            }

            cooldownImage.fillAmount = cd.GetCooldownRemaining() / cd.duration;
        }
    }

}
