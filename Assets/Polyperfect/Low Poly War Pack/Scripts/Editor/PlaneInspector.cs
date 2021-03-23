using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace PolyPerfect
{
    namespace War
    {
        [CustomEditor(typeof(Plane))]
        [CanEditMultipleObjects]
        public class PlaneInspector : CustomUIEditor
        {
            int selected = 0;
            int selectedMain = 0;
            GUIContent[] mainTabs = new GUIContent[2];
            GUIContent[] tabs = new GUIContent[6];
            SerializedProperty m_ParachuteProp;
            SerializedProperty m_WeaponsProp;
            SerializedProperty m_WeaponsCenterProp;
            SerializedProperty m_MaxSpeedProp;
            SerializedProperty m_EngineTorqueProp;
            SerializedProperty m_BrakePowerProp;
            SerializedProperty m_MaxSteeringAngleProp;
            SerializedProperty m_PlaneHorizontalProp;
            SerializedProperty m_PlaneVerticalProp;
            SerializedProperty m_RightWingProp;
            SerializedProperty m_LeftWingProp;
            SerializedProperty m_RightHorizontalStabilizerProp;
            SerializedProperty m_LeftHorizontalStabilizerProp;
            SerializedProperty m_VerticalStabilizerProp;
            SerializedProperty m_RudderProp;
            SerializedProperty m_RightElevatorProp;
            SerializedProperty m_LeftElevatorProp;
            SerializedProperty m_RightAileronProp;
            SerializedProperty m_LeftAileronProp;
            SerializedProperty m_WheelsProp;
            SerializedProperty m_SteeringCurveProp;
            SerializedProperty m_AerodynamicCurvesProp;
            SerializedProperty m_SpeedTextProp;
            SerializedProperty m_CenterOfMassProp;
            SerializedProperty m_PropellersProp;
            SerializedProperty m_CrosshairGunProp;
            SerializedProperty m_HealthProp;
            SerializedProperty m_TrailsProp;
            SerializedProperty m_TrailEmitingSpeedProp;
            SerializedProperty m_EngineThrottleUIProp;
            SerializedProperty m_EngineThrottleTextProp;
            SerializedProperty m_RollProp;
            SerializedProperty m_YawProp;
            SerializedProperty m_PitchProp;
            SerializedProperty m_BackTurningWheelProp;
            protected override void OnEnable()
            {
                base.OnEnable();
                tabs[0] = new GUIContent("", Resources.Load<Texture>("Wheel"), "Wheels setup");
                tabs[1] = new GUIContent("", Resources.Load<Texture>("Wing"), "Wings and controll surfaces");
                tabs[2] = new GUIContent("", Resources.Load<Texture>("Armament"), "Armament");
                tabs[3] = new GUIContent("", Resources.Load<Texture>("Engine"), "Engine");
                tabs[4] = new GUIContent("", Resources.Load<Texture>("UI"), "User Interface");
                tabs[5] = new GUIContent("", Resources.Load<Texture>("Interaction"), "Interaction setup");
                mainTabs[0] = new GUIContent(" Set Up", Resources.Load<Texture>("Setup"));
                mainTabs[1] = new GUIContent(" Statistics", Resources.Load<Texture>("Statistics"));
                SetUpProperties();
            }
            void SetUpProperties()
            {
                m_ParachuteProp = serializedObject.FindProperty("parachute");
                m_WeaponsProp = serializedObject.FindProperty("weapons");
                m_WeaponsCenterProp = serializedObject.FindProperty("weaponsCenter");
                m_MaxSpeedProp = serializedObject.FindProperty("maxSpeed");
                m_EngineTorqueProp = serializedObject.FindProperty("engineTorque");
                m_BrakePowerProp = serializedObject.FindProperty("brakePower");
                m_MaxSteeringAngleProp = serializedObject.FindProperty("maxSteeringAngle");
                m_PlaneHorizontalProp = serializedObject.FindProperty("planeHorizontal");
                m_PlaneVerticalProp = serializedObject.FindProperty("planeVertical");
                m_RightWingProp = serializedObject.FindProperty("rightWing");
                m_LeftWingProp = serializedObject.FindProperty("leftWing");
                m_RightHorizontalStabilizerProp = serializedObject.FindProperty("rightHorizontalStabilizer");
                m_LeftHorizontalStabilizerProp = serializedObject.FindProperty("leftHorizontalStabilizer");
                m_VerticalStabilizerProp = serializedObject.FindProperty("verticalStabilizer");
                m_RudderProp = serializedObject.FindProperty("rudder");
                m_RightElevatorProp = serializedObject.FindProperty("rightElevator");
                m_LeftElevatorProp = serializedObject.FindProperty("leftElevator");
                m_RightAileronProp = serializedObject.FindProperty("rightAileron");
                m_LeftAileronProp = serializedObject.FindProperty("leftAileron");
                m_WheelsProp = serializedObject.FindProperty("wheels");
                m_SteeringCurveProp = serializedObject.FindProperty("steeringCurve");
                m_AerodynamicCurvesProp = serializedObject.FindProperty("aerodynamicCurves");
                m_SpeedTextProp = serializedObject.FindProperty("speedText");
                m_CenterOfMassProp = serializedObject.FindProperty("centerOfMass");
                m_PropellersProp = serializedObject.FindProperty("propellers");
                m_CrosshairGunProp = serializedObject.FindProperty("crosshairGun");
                m_HealthProp = serializedObject.FindProperty("health");
                m_TrailsProp = serializedObject.FindProperty("trails");
                m_TrailEmitingSpeedProp = serializedObject.FindProperty("trailEmitingSpeed");
                m_EngineThrottleUIProp = serializedObject.FindProperty("engineThrottleUI");
                m_EngineThrottleTextProp = serializedObject.FindProperty("engineThrottleText");
                m_PitchProp = serializedObject.FindProperty("pitchRange");
                m_YawProp = serializedObject.FindProperty("yawRange");
                m_RollProp = serializedObject.FindProperty("rollRange");
                m_BackTurningWheelProp = serializedObject.FindProperty("backTurningWheel");
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

                        EditorGUILayout.PropertyField(m_BackTurningWheelProp, false);
                        EditorGUILayout.PropertyField(m_WheelsProp, true);

                        break;
                    case 1:
                        EditorGUILayout.PropertyField(m_PlaneHorizontalProp, true);
                        EditorGUILayout.PropertyField(m_PlaneVerticalProp, true);
                        EditorGUILayout.PropertyField(m_RightWingProp, true);
                        EditorGUILayout.PropertyField(m_LeftWingProp, true);
                        EditorGUILayout.PropertyField(m_RightHorizontalStabilizerProp, true);
                        EditorGUILayout.PropertyField(m_LeftHorizontalStabilizerProp, true);
                        EditorGUILayout.PropertyField(m_VerticalStabilizerProp, true);
                        EditorGUILayout.PropertyField(m_RudderProp, true);
                        EditorGUILayout.PropertyField(m_RightElevatorProp, true);
                        EditorGUILayout.PropertyField(m_LeftElevatorProp, true);
                        EditorGUILayout.PropertyField(m_RightAileronProp, true);
                        EditorGUILayout.PropertyField(m_LeftAileronProp, true);
                        break;
                    case 2:
                        EditorGUILayout.PropertyField(m_WeaponsProp, true);
                        EditorGUILayout.PropertyField(m_WeaponsCenterProp, true);

                        break;
                    case 3:
                        EditorGUILayout.PropertyField(m_PropellersProp, true);

                        break;
                    case 4:
                        DrawHealthGUI();
                        EditorGUILayout.PropertyField(m_CrosshairGunProp);
                        EditorGUILayout.PropertyField(m_SpeedTextProp);
                        EditorGUILayout.PropertyField(m_EngineThrottleUIProp);
                        EditorGUILayout.PropertyField(m_EngineThrottleTextProp);
                        break;
                    case 5:
                        DrawInteractGUI();
                        EditorGUILayout.PropertyField(m_TrailsProp, true);
                        EditorGUILayout.PropertyField(m_ParachuteProp);
                        break;
                }

            }
            void DoStatistics()
            {
                EditorGUILayout.PropertyField(m_HealthProp);
                EditorGUILayout.PropertyField(m_EngineTorqueProp);
                EditorGUILayout.PropertyField(m_BrakePowerProp);
                EditorGUILayout.PropertyField(m_MaxSpeedProp);
                EditorGUILayout.PropertyField(m_MaxSteeringAngleProp);
                EditorGUILayout.PropertyField(m_SteeringCurveProp);
                EditorGUILayout.PropertyField(m_AerodynamicCurvesProp);
                EditorGUILayout.PropertyField(m_TrailEmitingSpeedProp);
                EditorGUILayout.PropertyField(m_CenterOfMassProp);
                EditorGUILayout.PropertyField(m_RollProp);
                EditorGUILayout.PropertyField(m_YawProp);
                EditorGUILayout.PropertyField(m_PitchProp);
            }
        }
    }
}