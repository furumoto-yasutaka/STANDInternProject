using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BattleStageSelectButtonGenerator))]
public class BattleStageSelectButtonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BattleStageSelectButtonGenerator generator = target as BattleStageSelectButtonGenerator;

        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            generator.Clear();
        }
    }
}
#endif
