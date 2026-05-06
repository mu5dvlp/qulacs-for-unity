using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuantumStateSetter))]
public class QuantumStateSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("qubitObject"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mode"));

        var mode = (StateSetMode)serializedObject.FindProperty("mode").enumValueIndex;

        if (mode == StateSetMode.Bloch)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("thetaDeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("phiDeg"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("alpha"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("betaReal"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("betaImag"));
        }

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(4);

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("Apply"))
            ((QuantumStateSetter)target).Apply();
        EditorGUI.EndDisabledGroup();
    }
}
