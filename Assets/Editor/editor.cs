using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(generator))]

public class editor : Editor
{
    public override void OnInspectorGUI() {
        generator gen = (generator)target;
        
        if (DrawDefaultInspector())
        {
            if (gen.autoUpdate)
            {
                gen.displayMapInEditor();
            }
        }
        if (GUILayout.Button("Generate")) {
            gen.displayMapInEditor();
        }
    }
}
