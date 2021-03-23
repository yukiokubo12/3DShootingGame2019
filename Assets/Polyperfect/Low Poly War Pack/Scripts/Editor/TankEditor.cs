using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace PolyPerfect
{
    namespace War
    {
        [CustomEditor(typeof(TankController))]
        public class TankEditor : CustomUIEditor
        {
            int selected = 0;
            int selectedMain = 0;
            GUIContent[] mainTabs = new GUIContent[2];
            GUIContent[] tabs = new GUIContent[4];
            SerializedProperty m_HealthProp;
            SerializedProperty m_MotorTourqeProp;
            SerializedProperty m_BrakePowerProp;
            SerializedProperty m_MaxSpeedProp;
            SerializedProperty m_TurnSpeedProp;
            SerializedProperty m_TurretRotSpeedProp;
            SerializedProperty m_GunRotSpeedProp;
            SerializedProperty m_ElevationProp;
            SerializedProperty m_DepressionProp;
            SerializedProperty m_TankTurretProp;
            SerializedProperty m_TankGunProp;
            SerializedProperty m_RightBeltRendererProp;
            SerializedProperty m_LeftBeltRendererProp;
            SerializedProperty m_CenterOfMassProp;
            SerializedProperty m_LeftBeltWheelsProp;
            SerializedProperty m_RightBeltWheelsProp;
            SerializedProperty m_CosmeticWheelsProp;
            SerializedProperty m_WeaponProp;
            SerializedProperty m_CrosshairGunProp;
            SerializedProperty m_SpeedTextProp;
            SerializedProperty m_ReloadingUIProp;
            SerializedProperty m_AudioParamsProp;
            protected override void OnEnable()
            {
                base.OnEnable();
                tabs[0] = new GUIContent("", Resources.Load<Texture>("Belt"), "Belts setup");
                tabs[1] = new GUIContent("", Resources.Load<Texture>("Armament"), "Armament");
                tabs[2] = new GUIContent("", Resources.Load<Texture>("UI"), "User interface");
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
                m_TurnSpeedProp = serializedObject.FindProperty("turnSpeed");
                m_TurretRotSpeedProp = serializedObject.FindProperty("turretRotSpeed");
                m_GunRotSpeedProp = serializedObject.FindProperty("gunRotSpeed");
                m_ElevationProp = serializedObject.FindProperty("elevation");
                m_DepressionProp = serializedObject.FindProperty("depression");
                m_TankTurretProp = serializedObject.FindProperty("tankTurret");
                m_TankGunProp = serializedObject.FindProperty("tankGun");
                m_RightBeltRendererProp = serializedObject.FindProperty("rightBeltRenderer");
                m_LeftBeltRendererProp = serializedObject.FindProperty("leftBeltRenderer");
                m_CenterOfMassProp = serializedObject.FindProperty("centerOfMass");
                m_LeftBeltWheelsProp = serializedObject.FindProperty("leftBeltWheels");
                m_RightBeltWheelsProp = serializedObject.FindProperty("rightBeltWheels");
                m_CosmeticWheelsProp = serializedObject.FindProperty("cosmeticWheels");
                m_WeaponProp = serializedObject.FindProperty("weapon");
                m_CrosshairGunProp = serializedObject.FindProperty("crosshairGun");
                m_SpeedTextProp = serializedObject.FindProperty("speedText");
                m_ReloadingUIProp = serializedObject.FindProperty("reloadingUI");
                m_AudioParamsProp = serializedObject.FindProperty("audioParams");
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
                        EditorGUILayout.LabelField("Right belt",EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(m_RightBeltRendererProp);
                        EditorGUILayout.PropertyField(m_RightBeltWheelsProp, true);
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Left belt", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(m_LeftBeltRendererProp);
                        EditorGUILayout.PropertyField(m_LeftBeltWheelsProp, true);
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(m_CosmeticWheelsProp, true);
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(m_TankGunProp);
                        EditorGUILayout.PropertyField(m_WeaponProp);

                        break;
                    case 2:
                        DrawHealthGUI();
                        EditorGUILayout.PropertyField(m_CrosshairGunProp);
                        EditorGUILayout.PropertyField(m_ReloadingUIProp);
                        EditorGUILayout.PropertyField(m_SpeedTextProp);
                        break;
                    case 3:
                        DrawInteractGUI();

                        EditorGUILayout.PropertyField(m_AudioParamsProp, true);
                        EditorGUILayout.PropertyField(m_TankTurretProp);

                        break;
                }
            }
            void DoStatistics()
            {
                EditorGUILayout.PropertyField(m_HealthProp);
                EditorGUILayout.PropertyField(m_MotorTourqeProp);
                EditorGUILayout.PropertyField(m_BrakePowerProp);
                EditorGUILayout.PropertyField(m_MaxSpeedProp);
                EditorGUILayout.PropertyField(m_TurnSpeedProp);
                EditorGUILayout.PropertyField(m_TurretRotSpeedProp);
                EditorGUILayout.PropertyField(m_GunRotSpeedProp);
                EditorGUILayout.PropertyField(m_ElevationProp);
                EditorGUILayout.PropertyField(m_DepressionProp);
                EditorGUILayout.PropertyField(m_CenterOfMassProp);
            }
        }
    }
}