using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Ardunity;


[CustomEditor(typeof(GenericServo))]
public class GenericServoEditor : ArdunityObjectEditor
{	
	bool foldout = false;
    
    SerializedProperty script;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty smooth;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		smooth = serializedObject.FindProperty("smooth");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		GenericServo controller = (GenericServo)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUILayout.PropertyField(smooth, new GUIContent("Smooth"));
			EditorGUI.indentLevel--;
		}
		
		int newValue = (int)EditorGUILayout.Slider("Calibrated Angle", controller.calibratedAngle, -45f, 45f);
		if(newValue != controller.calibratedAngle)
		{
			controller.calibratedAngle = newValue;
			if(!Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}
		newValue = (int)EditorGUILayout.Slider("Min Angle", controller.minAngle, -90f, 90f);
		if(newValue != controller.minAngle)
		{
			controller.minAngle = newValue;
			if(!Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}
		newValue = (int)EditorGUILayout.Slider("Max Angle", controller.maxAngle, -90f, 90f);
		if(newValue != controller.maxAngle)
		{
			controller.maxAngle = newValue;
			if(!Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}
		newValue = (int)EditorGUILayout.Slider("Angle", controller.angle, -90f, 90f);
		if(newValue != controller.angle)
		{
			controller.angle = newValue;
			if(!Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Motor/GenericServo";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(GenericServo));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
