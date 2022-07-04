using UnityEngine;
using System.Collections;

public class PopUpAttribute : PropertyAttribute{
	public string[] items;

	public PopUpAttribute(string[] values){
		items = values;
	}
}
