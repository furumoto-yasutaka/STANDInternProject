using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(StageSelectCourceGenerator))]
public class StageSelectCourceGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StageSelectCourceGenerator generator = target as StageSelectCourceGenerator;

        if (GUILayout.Button("Generate"))
        {
            generator.GenerateCource();
        }
        if (GUILayout.Button("Clear"))
        {
            generator.ClearCource();
        }
    }
}
#endif