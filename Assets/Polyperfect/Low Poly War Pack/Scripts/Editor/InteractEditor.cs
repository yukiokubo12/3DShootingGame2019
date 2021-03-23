using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PolyPerfect
{
    namespace War
    {
        [CustomEditor(typeof(InteractableObject))]
        public class CustomUIEditor : InspectorWindow
        {
            SerializedProperty m_UIHealthProp;
            SerializedProperty m_UITextProp;
            SerializedProperty m_UISliderProp;
            SerializedProperty m_CanvasProp;
            SerializedProperty m_PlayerSlotProp;
            SerializedProperty m_GetOutPointProp;
            SerializedProperty m_BlowUpFXProp;
            SerializedProperty m_VirtualCameraProp;
            SerializedProperty m_StopInteractingKeyProp;
            protected virtual void OnEnable()
            {
                m_UIHealthProp = serializedObject.FindProperty("uIHealthType");
                m_UISliderProp = serializedObject.FindProperty("uISlider");
                m_UITextProp = serializedObject.FindProperty("uIText");
                m_CanvasProp = serializedObject.FindProperty("canvas");
                m_PlayerSlotProp = serializedObject.FindProperty("playerSlot");
                m_GetOutPointProp = serializedObject.FindProperty("getOutPoint");
                m_BlowUpFXProp = serializedObject.FindProperty("blowUpFX");
                m_VirtualCameraProp = serializedObject.FindProperty("virtualCamera");
                m_StopInteractingKeyProp = serializedObject.FindProperty("stopInteractingKey");
            }
            public void DrawHealthGUI()
            {


                EditorGUILayout.LabelField("Health UI", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_UIHealthProp);
                if (m_UIHealthProp.intValue == (int)UIHealth.Text)
                {
                    EditorGUILayout.PropertyField(m_UITextProp);
                }
                if (m_UIHealthProp.intValue == (int)UIHealth.Slider)
                {
                    EditorGUILayout.PropertyField(m_UISliderProp);
                }
                if (m_UIHealthProp.intValue == (int)UIHealth.TextAndSlider)
                {
                    EditorGUILayout.PropertyField(m_UITextProp);
                    EditorGUILayout.PropertyField(m_UISliderProp);

                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            public void DrawInteractGUI()
            {
                //  DrawFoldoutHeader(serializedObject.FindProperty("playerSlot"), ref folded,"Interaction setup", "stopInteractingKey");
                EditorGUILayout.LabelField("Interaction setup", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_CanvasProp);
                EditorGUILayout.PropertyField(m_PlayerSlotProp);
                EditorGUILayout.PropertyField(m_GetOutPointProp);
                EditorGUILayout.PropertyField(m_BlowUpFXProp);
                EditorGUILayout.PropertyField(m_VirtualCameraProp);
                EditorGUILayout.PropertyField(m_StopInteractingKeyProp);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            public void DrawFoldoutHeader(SerializedProperty property, ref bool folded, string header, string lastProperty)
            {
                GUIStyle style = EditorStyles.foldout;
                FontStyle previousStyle = style.fontStyle;
                style.fontStyle = FontStyle.Bold;

                // calculate the rect values for where to draw the line in the inspector
                folded = EditorGUILayout.Foldout(folded, header, true, style);
                style.fontStyle = previousStyle;
                if (folded)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(property, true);
                    while (property.Next(false))
                    {
                        // Debug.Log(property.name + " " + property.isArray + " " + property.hasChildren);
                        EditorGUILayout.PropertyField(property.serializedObject.FindProperty(property.name), true);
                        if (property.name == lastProperty) { break; }
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}