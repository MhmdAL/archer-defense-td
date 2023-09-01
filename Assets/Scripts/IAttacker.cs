using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IAttacker
{
	List<TargetHitEffect> OnHitEffects { get; }
}
