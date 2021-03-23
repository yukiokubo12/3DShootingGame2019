using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace PolyPerfect
{
    namespace War
    {
        [CustomEditor(typeof(VirtualCamera))]
        public class VirtualCameraInspector : Editor
        {
            SerializedProperty m_AllowZoomProp;
            SerializedProperty m_ZoomSpeedProp;
            SerializedProperty m_CenteringProp;
            SerializedProperty m_TimeUntilCenterProp;
            SerializedProperty m_CenteringSpeedProp;
            private void OnEnable()
            {
                m_AllowZoomProp = serializedObject.FindProperty("allowZoom");
                m_ZoomSpeedProp = serializedObject.FindProperty("zoomSpeed");
                m_CenteringProp = serializedObject.FindProperty("centering");
                m_TimeUntilCenterProp = serializedObject.FindProperty("timeUntilCenter");
                m_CenteringSpeedProp = serializedObject.FindProperty("centeringSpeed");
            }
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                serializedObject.Update();
                EditorGUILayout.PropertyField(m_AllowZoomProp);
                if (m_AllowZoomProp.boolValue == true)
                {
                    EditorGUILayout.PropertyField(m_ZoomSpeedProp);
                }
                EditorGUILayout.PropertyField(m_CenteringProp);
                if (m_CenteringProp.boolValue == true)
                {
                    EditorGUILayout.PropertyField(m_TimeUntilCenterProp);
                    EditorGUILayout.PropertyField(m_CenteringSpeedProp);
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
