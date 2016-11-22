using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ClassRegister : ScriptableObject {
    public List<Class> classes;

    [MenuItem("Assets/Create/Class Register")]
    public static ClassRegister Create() {
        ClassRegister reg = CreateInstance<ClassRegister>();

        AssetDatabase.CreateAsset(reg, "Assets/Classes.asset");
        AssetDatabase.SaveAssets();
        return reg;
    }
}
