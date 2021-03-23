using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PolyPerfect
{
    namespace War
    {
        public class InspectorWindow : Editor
        {
            protected SerializedProperty currentProperty;
            protected void DrawProperties(SerializedProperty prop, bool drawChildren)
            {
                string lastPropPath = string.Empty;
                foreach (SerializedProperty p in prop)
                {
                    Debug.Log(p.CountInProperty() + " " + p.displayName);
                    if (p.isArray && (p.propertyType == SerializedPropertyType.Generic))
                    {
                        EditorGUILayout.BeginHorizontal();
                        p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                        EditorGUILayout.EndHorizontal();

                        if (p.isExpanded)
                        {
                            EditorGUI.indentLevel++;
                            DrawProperties(p, drawChildren);
                            EditorGUI.indentLevel--;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                        lastPropPath = p.propertyPath;
                        EditorGUILayout.PropertyField(p, drawChildren);
                    }
                }
            }
        }
    }
}
