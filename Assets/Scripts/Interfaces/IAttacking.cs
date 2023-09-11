using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IAttacking
{
	List<TargetHitEffect> OnHitEffects { get; }
}
