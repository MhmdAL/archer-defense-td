using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ArtilleryArrow : MonoBehaviour
{

	[HideInInspector] public Monster target;
	[HideInInspector] public IAttacker owner;

	[HideInInspector] public float damage;
	[HideInInspector] public float radius;
	[HideInInspector] public float armorPen;
	[HideInInspector] public float speed;

	private float progress;
	private float angle;

	private Vector3 startPos;
	private Vector3 endPos;
	private Vector3 dir;

	private Transform myTransform;
	private Transform targetTransform;

	private CooldownTimer rotationChangeTimer = new CooldownTimer(0);

	void Awake()
	{
		myTransform = transform;
		startPos = myTransform.position;
	}

	void Start()
	{
		if (target)
		{
			targetTransform = target.transform;
			endPos = target.transform.position;
		}
	}

	void FixedUpdate()
	{
		if (target == null)
		{
			Destroy(myTransform.root.gameObject);
			return;
		}

		if (rotationChangeTimer.GetCooldownRemaining() <= 0)
		{
			angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			rotationChangeTimer.ResetTimer(0.5f);
		}

		myTransform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

		dir = targetTransform.position - myTransform.position;

		endPos = targetTransform.position;

		float pathLength = Distance(startPos, endPos);
		float step = speed * Time.fixedDeltaTime / pathLength;
		progress += step;

		myTransform.position = Vector3.Lerp(startPos, endPos, progress);
		if (dir.magnitude <= 1)
		{
			BulletHit();
		}
	}

	public void BulletHit()
	{
		target.Damage(damage, 0, DamageSource.Normal, owner);
		Destroy(gameObject);
	}

	public float Distance(Vector3 a, Vector3 b)
	{
		Vector3 diff = new Vector3();
		diff.x = b.x - a.x;
		diff.y = b.y - a.y;
		diff.z = b.z - a.z;
		return Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y + diff.z * diff.z);
	}
}