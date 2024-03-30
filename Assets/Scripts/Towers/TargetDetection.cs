
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetDetection
{
    public static List<Monster> CalculateTargets(
        Tower source,
        List<Monster> targetsInRange,
        float damage,
        float armorPen)
    {
        targetsInRange.Sort((x, y) => y.DistanceTravelled.CompareTo(x.DistanceTravelled)); // Sort enemies in range by how far they travelled

        var validTargets = targetsInRange.ToList();

        // Detect monsters in range
        // targetsInRange.Clear();
        // foreach (Monster x in allEnemies)
        // {
        //     if (IsInRange(source.transform, x, range) && !targetsInRange.Contains(x))
        //     {
        //         targetsInRange.Add(x);
        //     }
        //     else if (!IsInRange(source.transform, x, range) && targetsInRange.Contains(x))
        //     {
        //         targetsInRange.Remove(x);
        //     }
        // }

        // Remove monsters who leave range from list of targets
        // foreach (Monster t in currentTargets.ToList())
        // {
        //     if (!IsInRange(source.transform, t, range) || t.IsDead)
        //     {
        //         if (currentTargets[0] != null && currentTargets[0].targetedBy.Count > 0)
        //         {
        //             if (currentTargets[0].targetedBy.FirstOrDefault(x => x == source))
        //                 currentTargets[0].targetedBy.Remove(source);
        //         }
        //         currentTargets[currentTargets.IndexOf(t)] = null;
        //     }
        // }

        // // Set currentTargets
        // GetValidTargets(source, targetsInRange, currentTargets);

        // if (validTargets.Any() && validTargets[0] != null)
        // {
        //     if (ValueStore.Instance.monsterManagerInstance.DoesKill(validTargets[0], damage, armorPen))
        //     {
        //         validTargets[0].isAboutToDie = true;
        //     }
        // }

        return validTargets;
    }

    public static List<Monster> GetValidTargets(Tower source, List<Monster> targetsInRange)
    {
        var validTargets = new List<Monster>();

        if (targetsInRange.Any())
        { // If there is atleast 1 enemy in range
            targetsInRange.Sort((x, y) => y.DistanceTravelled.CompareTo(x.DistanceTravelled)); // Sort enemies in range by how far they travelled

            if (targetsInRange[0] != validTargets[1])
            { // If primary and secondary targets are null

                if (targetsInRange.Count > 1)
                { // If there is more than 1 enemy in range
                    Monster someTarget = targetsInRange.FirstOrDefault(x => x.isAboutToDie == false); // Try to target enemies that arent about to die
                    if (someTarget != null)
                    {
                        validTargets[0] = someTarget;
                    }
                    else
                    { // If non are found then default to the farthest enemy
                        validTargets[0] = targetsInRange[0];
                    }
                }
                else
                {
                    validTargets[0] = targetsInRange[0];
                }

            }
            else if (validTargets[0] == null && targetsInRange[0] == validTargets[1])
            { // If primary target is null and secondary target is not null
                validTargets[0] = targetsInRange[0]; // Set as primary instead of secondary
                validTargets[1] = null;
            }
            if (!validTargets[0].targetedBy.Contains(source))
            {
                validTargets[0].targetedBy.Add(source);
            }
            validTargets[0].RecheckTargeter(); // Make sure enemy isnt about to die
        }

        return validTargets;
    }

    public static bool IsInRange(Transform towerTransform, Monster m, float attackRange)
    {
        if (m)
        {
            Vector3 targetDistance = m.transform.root.position - towerTransform.position;
            if (targetDistance.magnitude < attackRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return false;
    }
}