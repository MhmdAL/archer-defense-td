
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetDetection
{
    public static void CalculateTargets(
        Tower source,
        List<Monster> allEnemies,
        List<Monster> targetsInRange,
        List<Monster> currentTargets,
        float range,
        float damage,
        float armorPen)
    {
        // Detect monsters in range
        targetsInRange.Clear();
        foreach (Monster x in allEnemies)
        {
            if (IsInRange(source.transform, x, range) && !targetsInRange.Contains(x))
            {
                targetsInRange.Add(x);
            }
            else if (!IsInRange(source.transform, x, range) && targetsInRange.Contains(x))
            {
                targetsInRange.Remove(x);
            }
        }

        // Remove monsters who leave range from list of targets
        foreach (Monster t in currentTargets.ToList())
        {
            if (!IsInRange(source.transform, t, range) || t.IsDead)
            {
                if (currentTargets[0] != null && currentTargets[0].targetedBy.Count > 0)
                {
                    if (currentTargets[0].targetedBy.FirstOrDefault(x => x == source))
                        currentTargets[0].targetedBy.Remove(source);
                }
                currentTargets[currentTargets.IndexOf(t)] = null;
            }
        }
        
        // Set currentTargets
        SetTargets(source, targetsInRange, currentTargets);

        if (currentTargets[0] != null)
        {
            if (ValueStore.Instance.monsterManagerInstance.DoesKill(currentTargets[0], damage, armorPen))
            {
                currentTargets[0].isAboutToDie = true;
            }
        }
    }

    public static void SetTargets(Tower source, List<Monster> targetsInRange, List<Monster> currentTargets)
    {
        if (targetsInRange.Count > 0)
        { // If there is atleast 1 enemy in range
            targetsInRange.Sort((x, y) => y.distanceTravelled.CompareTo(x.distanceTravelled)); // Sort enemies in range by how far they travelled

            if (currentTargets[0] == null && targetsInRange[0] != currentTargets[1])
            { // If primary and secondary targets are null

                if (targetsInRange.Count > 1)
                { // If there is more than 1 enemy in range
                    Monster someTarget = targetsInRange.FirstOrDefault(x => x.isAboutToDie == false); // Try to target enemies that arent about to die
                    if (someTarget != null)
                    {
                        currentTargets[0] = someTarget;
                    }
                    else
                    { // If non are found then default to the farthest enemy
                        currentTargets[0] = targetsInRange[0];
                    }
                }
                else
                {
                    currentTargets[0] = targetsInRange[0];
                }

            }
            else if (currentTargets[0] == null && targetsInRange[0] == currentTargets[1])
            { // If primary target is null and secondary target is not null
                currentTargets[0] = targetsInRange[0]; // Set as primary instead of secondary
                currentTargets[1] = null;
            }
            if (!currentTargets[0].targetedBy.Contains(source))
            {
                currentTargets[0].targetedBy.Add(source);
            }
            currentTargets[0].RecheckTargeter(); // Make sure enemy isnt about to die
        }
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