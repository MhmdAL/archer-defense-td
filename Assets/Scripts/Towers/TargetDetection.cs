
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetDetection
{
    private static float CalculatePriority(Monster m, Tower source, Vector2? focusDir)
    {
        var priority = m.DistanceTravelled;

        if (focusDir != null)
        {
            var targetDir = (m.transform.position - source.transform.position).normalized;

            var dot = Vector2.Dot(focusDir.Value, targetDir);

            priority *= dot;
        }

        return priority;
    }

    public static List<Monster> CalculateTargets(
        Tower source,
        List<Monster> targetsInRange,
        List<Monster> currentTargets,
        float? targetFocusAngle,
        float damage,
        float armorPen)
    {
        Vector2? focusDir = targetFocusAngle != null ? new Vector2(Mathf.Cos(targetFocusAngle.Value), Mathf.Sin(targetFocusAngle.Value)) : null;

        var prioDict = new Dictionary<Monster, float>();

        foreach (var target in targetsInRange)
        {
            var priorityMultiplier = currentTargets.Contains(target) ? 10 : 1;

            prioDict[target] = priorityMultiplier * CalculatePriority(target, source, focusDir);
        }

        targetsInRange.Sort((x, y) =>
        {
            return prioDict[y].CompareTo(prioDict[x]);
        });

        var validTargets = targetsInRange.ToList();

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