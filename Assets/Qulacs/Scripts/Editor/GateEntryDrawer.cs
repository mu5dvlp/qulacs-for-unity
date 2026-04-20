using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GateEntry))]
public class GateEntryDrawer : PropertyDrawer
{
    static readonly GateType[] TwoQubitGates = { GateType.CNOT };

    static bool IsTwoQubit(GateType g) => g is GateType.CNOT;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var gateTypeProp = property.FindPropertyRelative("gateType");
        bool twoQubit = IsTwoQubit((GateType)gateTypeProp.enumValueIndex);
        int rows = twoQubit ? 3 : 2;
        return EditorGUIUtility.singleLineHeight * rows + EditorGUIUtility.standardVerticalSpacing * (rows - 1) + EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var gateTypeProp = property.FindPropertyRelative("gateType");
        var controlProp = property.FindPropertyRelative("controlIndex");
        var targetProp = property.FindPropertyRelative("targetIndex");

        EditorGUI.BeginProperty(position, label, property);

        float lineH = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        var rect = new Rect(position.x, position.y, position.width, lineH);
        EditorGUI.PropertyField(rect, gateTypeProp);

        bool twoQubit = IsTwoQubit((GateType)gateTypeProp.enumValueIndex);

        if (twoQubit)
        {
            rect.y += lineH + spacing;
            EditorGUI.PropertyField(rect, controlProp);
        }

        rect.y += lineH + spacing;
        EditorGUI.PropertyField(rect, targetProp);

        EditorGUI.EndProperty();
    }
}
