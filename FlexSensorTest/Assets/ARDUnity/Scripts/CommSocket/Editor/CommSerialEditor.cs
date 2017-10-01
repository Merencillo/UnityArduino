using UnityEngine;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(CommSerial))]
public class CommSerialEditor : ArdunityObjectEditor
{
    bool foldout = false;
    
    SerializedProperty baudrate;
    SerializedProperty dtrReset;
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnErrorClosed;
    SerializedProperty OnStartSearch;
    SerializedProperty OnStopSearch;

    void OnEnable()
	{
        baudrate = serializedObject.FindProperty("baudrate");
        dtrReset = serializedObject.FindProperty("dtrReset");
        OnOpen = serializedObject.FindProperty("OnOpen");
        OnClose = serializedObject.FindProperty("OnClose");
        OnOpenFailed = serializedObject.FindProperty("OnOpenFailed");
        OnErrorClosed = serializedObject.FindProperty("OnErrorClosed");
        OnStartSearch = serializedObject.FindProperty("OnStartSearch");
        OnStopSearch = serializedObject.FindProperty("OnStopSearch");
    }
	
	public override void OnInspectorGUI()
	{
        this.serializedObject.Update();

#if UNITY_STANDALONE
        CommSerial socket = (CommSerial)target;
        GUI.enabled = !socket.IsOpen;

#if (UINTY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        EditorGUILayout.LabelField(string.Format("Port Name: {0}", socket.device.name));
#elif (UINTY_STANDALONE_OSX || UNITY_EDITOR_OSX)
        EditorGUILayout.LabelField(string.Format("Port Name: {0}", socket.device.address));
#endif
        EditorGUILayout.BeginHorizontal();
        int index = -1;
        string[] list = new string[socket.foundDevices.Count];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = socket.foundDevices[i].name;
            if (socket.device.Equals(socket.foundDevices[i]))
                index = i;
        }
        index = EditorGUILayout.Popup(" ", index, list);
        if (index >= 0)
            socket.device = new CommDevice(socket.foundDevices[index]);
        if (GUILayout.Button("Search", GUILayout.Width(60f)) == true)
            socket.StartSearch();
        EditorGUILayout.EndHorizontal();
        
        GUI.enabled = true;
#else
        EditorGUILayout.HelpBox("This component only can work on standalone platform(windows, osx, linux..)", MessageType.Error);
#endif
        
        EditorGUILayout.PropertyField(baudrate, new GUIContent("Baudrate"));
        EditorGUILayout.PropertyField(dtrReset, new GUIContent("DTR Reset"));

        foldout = EditorGUILayout.Foldout(foldout, "Events");
        if (foldout)
        {
            EditorGUILayout.PropertyField(OnOpen, new GUIContent("OnOpen"));
            EditorGUILayout.PropertyField(OnClose, new GUIContent("OnClose"));
            EditorGUILayout.PropertyField(OnOpenFailed, new GUIContent("OnOpenFailed"));
            EditorGUILayout.PropertyField(OnErrorClosed, new GUIContent("OnErrorClosed"));
            EditorGUILayout.PropertyField(OnStartSearch, new GUIContent("OnStartSearch"));
            EditorGUILayout.PropertyField(OnStopSearch, new GUIContent("OnStopSearch"));
        }

        this.serializedObject.ApplyModifiedProperties();
	}
    
    static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
    {
        string menuName = "Unity/Add CommSocket/CommSerial";
        
        if(Selection.activeGameObject != null)
            menu.AddItem(new GUIContent(menuName), false, func, typeof(CommSerial));
        else
            menu.AddDisabledItem(new GUIContent(menuName));
    }
}
