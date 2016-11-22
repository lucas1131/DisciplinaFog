using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ClassEditor : EditorWindow {
    public ClassRegister register;
    private int viewIndex = 1;

    [MenuItem("Window/Class Editor")]
    static void Init() {
        EditorWindow.GetWindow(typeof(ClassEditor));
    }

    void OnEnable() {
        if(EditorPrefs.HasKey("ObjectPath")) {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            register = AssetDatabase.LoadAssetAtPath(
                           objectPath,
                           typeof(ClassRegister)
                       ) as ClassRegister;
        } else {
            CreateRegister();
        }
    }

    void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Class Editor", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

    }

    void CreateRegister() {
        viewIndex = 1;
        register = ClassRegister.Create();
        if (register) {
            register.classes = new List<Class>();
            string relPath = AssetDatabase.GetAssetPath(register);
            EditorPrefs.SetString("ObjectPath", relPath);
        }
    }

    void ReadRegister() {
        string absPath = EditorUtility.OpenFilePanel("Select class data file",
                                                     "",
                                                     "");
        if (absPath.StartsWith(Application.dataPath)) {
            string relPath = absPath.Substring(
                                Application.dataPath.Length - "Assets".Length
                             );
            register = AssetDatabase.LoadAssetAtPath(
                            relPath,
                            typeof(ClassRegister)
                       ) as ClassRegister;
            if (register.classes == null)
                register.classes = new List<Class>();
            EditorPrefs.SetString("ObjectPath", relPath);
        }
    }
}
