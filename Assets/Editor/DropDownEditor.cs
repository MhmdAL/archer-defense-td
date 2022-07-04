using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PopUpAttribute))]
public class DropDownEditor : PropertyDrawer {
	
	string[] choices;

	public override void OnGUI (UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
	{
		//base.OnGUI (position, property, label);
		
		EditorGUI.BeginProperty (position, label, property);
			
		choices = ((PopUpAttribute)attribute).items;

		position = EditorGUI.PrefixLabel (position, EditorGUIUtility.GetControlID(UnityEngine.FocusType.Passive), GUIContent.none);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		var choicesRect = new Rect (position.x, position.y, 125, position.height);
		//var valueRect = new Rect (position.x + 135, position.y, 30, position.height);

		//EditorGUI.PropertyField (choicesRect, property.FindPropertyRelative ("methodName"), GUIContent.none);
		//choiceIndex = EditorGUILayout.Popup (choiceIndex, choices);
		int choiceIndex = 0;
		for (; choiceIndex < choices.Length - 1; choiceIndex++) {
			if (choices [choiceIndex].Equals (property.stringValue))
				break;
		}
		choiceIndex = EditorGUI.Popup(choicesRect, choiceIndex, choices);

		//if (choiceIndex < choices.Length)
		property.stringValue = choices [choiceIndex];
		//EditorGUI.PropertyField (valueRect, property.FindPropertyRelative ("value"), GUIContent.none);

		//property.FindPropertyRelative ("methodName").stringValue = choices [choiceIndex];

		EditorGUI.indentLevel = indent;



		EditorGUI.EndProperty ();

		//var b = (BoolEvaluator)target.;
		//b.methodName = choices [choiceIndex];
		//EditorUtility.SetDirty (property.objectReferenceValue);
	}
}
