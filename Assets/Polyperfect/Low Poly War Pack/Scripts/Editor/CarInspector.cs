using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace PolyPerfect
{
    namespace War
    {
        [CustomEditor(typeof(Car))]
        public class CarInspector : CustomUIEditor
        {
            int selected = 0;
            int selectedMain = 0;
            GUIContent[] mainTabs = new GUIContent[2];
            GUIContent[] tabs = new GUIContent[4];
            SerializedProperty m_HealthProp;
            SerializedProperty m_MotorTourqeProp;
            SerializedProperty m_BrakePowerProp;
            SerializedProperty m_MaxSpeedProp;
            SerializedProperty m_MaxSteeringAngleProp;
            SerializedProperty m_AxlesProp;
            SerializedProperty m_CenterOfMassProp;
            SerializedProperty m_SteeringCurveProp;
            SerializedProperty m_SpeedTextProp;
            SerializedProperty m_LightsProp;
            protected override void OnEnable()
            {
                base.OnEnable();
                tabs[0] = new GUIContent("", Resources.Load<Texture>("Wheel"), "Wheels setup");
                tabs[1] = new GUIContent("", Resources.Load<Texture>("Light"), "Lights setup");
                tabs[2] = new GUIContent("", Resources.Load<Texture>("UI"), "User Interface");
                tabs[3] = new GUIContent("", Resources.Load<Texture>("Interaction"), "Interaction setup");
                mainTabs[0] = new GUIContent(" Set Up", Resources.Load<Texture>("Setup"));
                mainTabs[1] = new GUIContent(" Statistics", Resources.Load<Texture>("Statistics"));
                SetUpProperties();
            }
            void SetUpProperties()
            {
                m_HealthProp = serializedObject.FindProperty("health");
                m_MotorTourqeProp = serializedObject.FindProperty("motorTourqe");
                m_BrakePowerProp = serializedObject.FindProperty("brakePower");
                m_MaxSpeedProp = serializedObject.FindProperty("maxSpeed");
                m_MaxSteeringAngleProp = serializedObject.FindProperty("maxSteeringAngle");
                m_AxlesProp = serializedObject.FindProperty("axles");
                m_CenterOfMassProp = serializedObject.FindProperty("centerOfMass");
                m_SteeringCurveProp = serializedObject.FindProperty("steeringCurve");
                m_SpeedTextProp = serializedObject.FindProperty("speedText");
                m_LightsProp = serializedObject.FindProperty("lights");
            }
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                selectedMain = GUILayout.Toolbar(selectedMain, mainTabs);
                if (selectedMain == 0)
                    DoSetUp();
                else
                    DoStatistics();
                serializedObject.ApplyModifiedProperties();
            }
            void DoSetUp()
            {
                selected = GUILayout.Toolbar(selected, tabs);
                switch (selected)
                {
                    case 0:
                        EditorGUILayout.PropertyField(m_AxlesProp, true);
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(m_LightsProp, true);
                        break;
                    case 2:
                        DrawHealthGUI();
                        EditorGUILayout.PropertyField(m_SpeedTextProp);
                        break;
                    case 3:
                        DrawInteractGUI();
                        break;
                }
            }
            void DoStatistics()
            {
                EditorGUILayout.PropertyField(m_HealthProp);
                EditorGUILayout.PropertyField(m_MotorTourqeProp);
                EditorGUILayout.PropertyField(m_BrakePowerProp);
                EditorGUILayout.PropertyField(m_MaxSpeedProp);
                EditorGUILayout.PropertyField(m_MaxSteeringAngleProp);
                EditorGUILayout.PropertyField(m_SteeringCurveProp);
                EditorGUILayout.PropertyField(m_CenterOfMassProp);
            }
        }
    }
}