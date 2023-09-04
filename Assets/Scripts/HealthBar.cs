using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [field: SerializeField]
    public Unit Unit { get; set; }

    [field: SerializeField]
    public GameObject HPBar { get; set; }

    [field: SerializeField]
    public GameObject ShieldBar { get; set; }

    private Image currentHpBar;

    private void Awake()
    {
        Unit.HealthChanged += OnHealthChanged;
        Unit.OnDeath += OnUnitDied;

        currentHpBar = HPBar.GetComponent<Image>();
    }

    private void OnHealthChanged()
    {
        currentHpBar.fillAmount = Unit.CurrentHP / (Unit.MaxHP.Value + Unit.CurrentShield);

        ShieldBar.transform.localScale = new Vector3(Unit.CurrentShield / (Unit.MaxHP.Value + Unit.CurrentShield), HPBar.transform.localScale.y, HPBar.transform.localScale.z);
    }

    private void OnUnitDied(Unit u, DamageSource ds)
    {
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Unit.HealthChanged -= OnHealthChanged;
        Unit.OnDeath -= OnUnitDied;
    }
}
