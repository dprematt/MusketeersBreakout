using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(generator))]

public class editor : Editor
{
    public override void OnInspectorGUI() {
        generator gen = (generator)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate")) {
            gen.SkeletonGenerator();
        }
    }
}
