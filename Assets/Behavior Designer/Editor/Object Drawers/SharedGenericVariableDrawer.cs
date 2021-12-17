using UnityEngine;
using UnityEditor;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Editor;
using System;

[CustomObjectDrawer(typeof(GenericVariable))]
public class SharedGenericVariableDrawer : ObjectDrawer
{
    private static string[] variableNames;

    public override void OnGUI(GUIContent label)
    {
        var genericVariable = value as GenericVariable;
        EditorGUILayout.BeginVertical();
        if (FieldInspector.DrawFoldout(genericVariable.GetHashCode(), label)) {
            EditorGUI.indentLevel++;
            if (variableNames == null) {
                var variables = VariableInspector.FindAllSharedVariableTypes(true);
                variableNames = new string[variables.Count];
                for (int i = 0; i < variables.Count; ++i) {
                    variableNames[i] = variables[i].Name.Remove(0, 6); // Remove "Shared"
                }
            }

            var index = 0;
            var variableName = genericVariable.type.Remove(0, 6);
            for (int i = 0; i < variableNames.Length; ++i) {
                if (variableNames[i].Equals(variableName)) {
                    index = i;
                    break;
                }
            }

            var newIndex = EditorGUILayout.Popup("Type", index, variableNames, BehaviorDesignerUtility.SharedVariableToolbarPopup);
            var variableType = VariableInspector.FindAllSharedVariableTypes(true)[newIndex]; // FindAllSharedVariableTypes caches the result so this is quick
            if (newIndex != index) {
                index = newIndex;
                genericVariable.value = Activator.CreateInstance(variableType) as SharedVariable;
            }

            GUILayout.Space(3);
            genericVariable.type = "Shared" + variableNames[index];
            genericVariable.value = FieldInspector.DrawSharedVariable(null, new GUIContent("Value"), null, variableType, genericVariable.value);

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
    }
}
