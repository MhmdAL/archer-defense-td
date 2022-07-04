using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public interface IModifiable {
	
	List<Modifier> Modifiers{ get; set;}
	List<Stat> Stats{ get; set;}
			
	void AddModifier(Modifier m, StackOperation s, int stackLimit);

	void RemoveModifier (Name name);

	Modifier GetModifier (Name name);

	void AdjustStats();

	void OnModifierEnded (Modifier m);
}
