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

    [field: SerializeField]
    public TextMeshProUGUI HealthText { get; set; }

    private Image currentHpBar;

    private void Start()
    {
        Unit.HealthChanged += OnHealthChanged;
        Unit.MaxHP.ValueModified += OnMaxHealthChanged;
        Unit.OnDeath += OnUnitDied;

        currentHpBar = HPBar.GetComponent<Image>();

        UpdateVisuals();
    }

    private void OnMaxHealthChanged(float oldValue, float newValue)
    {
        UpdateVisuals();
    }

    private void OnHealthChanged()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if(Unit == null)
        {
            return;
        }

        currentHpBar.fillAmount = Unit.CurrentHP / (Unit.MaxHP.Value + Unit.CurrentShield);

        ShieldBar.transform.localScale = new Vector3(Unit.CurrentShield / (Unit.MaxHP.Value + Unit.CurrentShield), HPBar.transform.localScale.y, HPBar.transform.localScale.z);

        HealthText.text = $"{Unit.CurrentHP} / {Unit.MaxHP.Value}";
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
