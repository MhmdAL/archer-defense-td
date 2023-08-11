using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [field: SerializeField]
    public Unit Unit { get; set; }

    [field: SerializeField]
    public GameObject HPBar { get; set; }

    [field: SerializeField]
    public GameObject ShieldBar { get; set; }

    private void Awake()
    {
        Unit.HealthChanged += OnHealthChanged;
        Unit.Died += OnUnitDied;
    }

    private void OnHealthChanged()
    {
        HPBar.transform.localScale = new Vector3(Unit.CurrentHP / (Unit.MaxHP.Value + Unit.CurrentShield), HPBar.transform.localScale.y, HPBar.transform.localScale.z);

        ShieldBar.transform.localScale = new Vector3(Unit.CurrentShield / (Unit.MaxHP.Value + Unit.CurrentShield), HPBar.transform.localScale.y, HPBar.transform.localScale.z);
    }

    private void OnUnitDied()
    {
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Unit.HealthChanged -= OnHealthChanged;
        Unit.Died -= OnUnitDied;
    }
}
