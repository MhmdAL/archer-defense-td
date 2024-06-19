using EPOOutline;
using UnityEngine;

[RequireComponent(typeof(Tower))]
public class TowerFocusable : Focusable
{
    public GameObject rangeCircle;
    public LineRenderer rangeCircleRenderer;
    public LineRenderer focusAreaRenderer;
    private Tower _tower;

    private void Awake()
    {
        _tower = GetComponent<Tower>();
    }

    private void Update()
    {
        if (HasFocus)
        {
            UpdateRangeCircle();

            if (Input.GetKeyDown(KeyCode.F))
            {
                var mousePos = Input.mousePosition.ToWorldPosition(Camera.main);
                mousePos.z = 0;

                var dir = mousePos - transform.position;

                _tower.targetFocusAngle = Mathf.Atan2(dir.y, dir.x);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _tower.targetFocusAngle = null;
            }

            if (Input.GetMouseButtonDown(1))
            {
                _tower.SetAttackMode(TowerAttackMode.Manual);

                _tower.AttackCooldownTimer.Resume();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                _tower.SetAttackMode(TowerAttackMode.Auto);

                _tower.AttackCooldownTimer.Pause();
            }

            if (Input.GetMouseButton(1))
            {
                _tower.SetCombatMode(CombatMode.InCombat);
            }
        }
    }

    public override void Focus()
    {
        base.Focus();

        Highlight();

        rangeCircle.SetActive(true);
    }

    public override void UnFocus()
    {
        base.UnFocus();

        UnHighlight();

        rangeCircle.SetActive(false);
    }

    private void UpdateRangeCircle()
    {
        const float focusAngleRange = 180f;

        rangeCircleRenderer.DrawCircle(transform.position, _tower.AR.Value);

        if (_tower.targetFocusAngle != null)
        {
            var ang = Mathf.Rad2Deg * _tower.targetFocusAngle.Value;
            if (ang < 0)
            {
                ang += 360;
            }

            focusAreaRenderer.DrawArc(ang - focusAngleRange / 2, ang + focusAngleRange / 2, transform.position, _tower.AR.Value);
        }
        else
        {
            focusAreaRenderer.positionCount = 0;
        }
    }

}
