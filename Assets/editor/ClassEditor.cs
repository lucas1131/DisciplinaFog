using UnityEngine;
using UnityEditor;
using System;
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
        if (register != null && GUILayout.Button("Show Classes")) {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = register;
        }
        if (GUILayout.Button("Open Class Data")) {
            ReadRegister();
        }
        if (GUILayout.Button("New Class Data")) {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = register;
        }
        GUILayout.EndHorizontal();

        if (register != null) {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
                if (viewIndex > 1)
                    viewIndex--;
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
                if (viewIndex < register.classes.Count)
                    viewIndex++;

            GUILayout.Space(60);
            if (GUILayout.Button("Add Class", GUILayout.ExpandWidth(false)))
                AddItem();
            if (GUILayout.Button("Delete Class", GUILayout.ExpandWidth(false)))
                DeleteItem(viewIndex - 1);
            GUILayout.EndHorizontal();

            if (register.classes.Count > 0) {
                GUILayout.BeginHorizontal();
                viewIndex = Mathf.Clamp(
                                EditorGUILayout.IntField(
                                    "Current Item",
                                    viewIndex,
                                    GUILayout.ExpandWidth(false)
                                ),
                                1,
                                register.classes.Count
                            );
                EditorGUILayout.LabelField(
                    "of   "
                    + register.classes.Count
                    + "  items",
                    "",
                    GUILayout.ExpandWidth(false)
                );
                // TODO fazer isso pra todos os campos de classes
                UpdateMoveCost(viewIndex-1);
                GUILayout.EndHorizontal();
            }
        }
    }

    void AddItem() {

    }

    void DeleteItem(int i) {

    }

    void UpdateMoveCost(int idx) {
        Class c = register.classes[idx];

        GUILayout.BeginHorizontal();
        GUILayout.Label("Movement Cost", EditorStyles.boldLabel);
        foreach (Terrains t in Enum.GetValues(typeof(Terrains))) {
            c.moveCost[(int)t] = EditorGUILayout.IntField(
                                     t.ToString(),
                                     999
                                 );
        }
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
