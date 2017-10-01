using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Ardunity;


[CustomEditor(typeof(AnalogOutput))]
public class AnalogOutputEditor : ArdunityObjectEditor
{
	bool foldout = false;
    
    SerializedProperty script;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty defaultValue;
	SerializedProperty resetOnStop;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		defaultValue = serializedObject.FindProperty("defaultValue");
		resetOnStop = serializedObject.FindProperty("resetOnStop");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		AnalogOutput controller = (AnalogOutput)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUILayout.PropertyField(defaultValue, new GUIContent("defaultValue"));
			EditorGUILayout.PropertyField(resetOnStop, new GUIContent("resetOnStop"));
			EditorGUI.indentLevel--;
		}
		
		float newValue = EditorGUILayout.Slider("Value", controller.Value, 0f, 1f);
		if(newValue != controller.Value)
		{
			controller.Value = newValue;
			if(!Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Basic/AnalogOutput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(AnalogOutput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
