using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TimerManager : MonoBehaviour {

	List<MyTimer> timers;

	void Awake () {
		timers = new List<MyTimer> ();
	}
	
	void Update () {
		foreach (var item in timers.ToList()) {
			item.Duration -= Time.deltaTime;
			if (item.Duration <= 0) {
				item.TimerFinished ();
				timers.Remove (item);
			}
		}
	}

	public MyTimer StartTimer(float duration){
		MyTimer t = new MyTimer (duration);
		timers.Add (t);
		return t;
	}
}

public class MyTimer
{

	public delegate void ElapsedHandler ();
	public event ElapsedHandler TimerElapsed;

	float duration;

	public float Duration {
		get {
			return duration;
		}
		set {
			duration = value;
		}
	}

	public MyTimer(float duration){
		this.duration = duration;
	}

	public void TimerFinished(){
		if(TimerElapsed != null)
			TimerElapsed ();
	}
}
