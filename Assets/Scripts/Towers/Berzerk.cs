using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berzerk : MonoBehaviour
{
    public Tower Owner { get; set; }

    public float BerzerkDuration { get; set; }
    public float BerzerkAtkSpdBuffValue { get; set; }

    public bool IsBerzerking { get; private set; }

    private void Start()
    {
        Owner.AttackFinished += OnAfterAttack;
        Owner.CombatEnded += OnCombatEnded;
    }

    private void OnCombatEnded()
    {
        Owner.ExtraData["BerzerkConsecutiveShots"] = 0;
    }

    private void OnAfterAttack()
    {
        var consecutiveShots = (int)Owner.ExtraData["BerzerkConsecutiveShots"];

        if (!IsBerzerking)
        {
            consecutiveShots++;
            Owner.ExtraData["BerzerkConsecutiveShots"] = consecutiveShots;
        }

        if (consecutiveShots == 8)
        {
            Owner.AS.Modify(BerzerkAtkSpdBuffValue, BonusOperation.Percentage, BuffNames.BERZERK, BerzerkDuration, 1, () => IsBerzerking = false);

            IsBerzerking = true;

            Owner.ExtraData["BerzerkConsecutiveShots"] = 0;
        }
    }

    private void OnDestroy()
    {
        Owner.AttackFinished -= OnAfterAttack;
        Owner.CombatEnded -= OnCombatEnded;
    }
}
