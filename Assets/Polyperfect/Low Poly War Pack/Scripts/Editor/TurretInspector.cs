using UnityEngine;
using UnityEditor;
namespace PolyPerfect
{
    namespace War
    {
        [CustomEditor(typeof(TurretController))]
        public class TurretInspector : CustomUIEditor
        {
            int selected = 0;
            int selectedMain = 0;
            GUIContent[] mainTabs = new GUIContent[2];
            GUIContent[] tabs = new GUIContent[3];
            SerializedProperty m_HealthProp;
            SerializedProperty m_TurretRotSpeedProp;
            SerializedProperty m_GunRotSpeedProp;
            SerializedProperty m_WeaponsProp;
            SerializedProperty m_ElevationProp;
            SerializedProperty m_DepressionProp;
            SerializedProperty m_TurretGunProp;
            SerializedProperty m_ReloadingUIProp;
            SerializedProperty m_CrosshairGunProp;
            SerializedProperty m_AutoFireProp;
            SerializedProperty m_WeaponsCenterProp;
            protected override void OnEnable()
            {
                base.OnEnable();
                tabs[0] = new GUIContent("", Resources.Load<Texture>("Armament"), "Turret setup");
                tabs[1] = new GUIContent("", Resources.Load<Texture>("UI"), "User Interface");
                tabs[2] = new GUIContent("", Resources.Load<Texture>("Interaction"), "Interaction setup");
                mainTabs[0] = new GUIContent(" Set Up", Resources.Load<Texture>("Setup"));
                mainTabs[1] = new GUIContent(" Statistics", Resources.Load<Texture>("Statistics"));
                SetUpProperties();
            }
            void SetUpProperties()
            {
                m_HealthProp = serializedObject.FindProperty("health");
                m_TurretRotSpeedProp = serializedObject.FindProperty("turretRotSpeed");
                m_GunRotSpeedProp = serializedObject.FindProperty("gunRotSpeed");
                m_WeaponsProp = serializedObject.FindProperty("weapons");
                m_ElevationProp = serializedObject.FindProperty("elevation");
                m_DepressionProp = serializedObject.FindProperty("depression");
                m_TurretGunProp = serializedObject.FindProperty("turretGun");
                m_ReloadingUIProp = serializedObject.FindProperty("reloadingUI");
                m_CrosshairGunProp = serializedObject.FindProperty("crosshairGun");
                m_AutoFireProp = serializedObject.FindProperty("autoFire");
                m_WeaponsCenterProp = serializedObject.FindProperty("weaponsCenter");
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
                        EditorGUILayout.PropertyField(m_TurretGunProp);
                        EditorGUILayout.PropertyField(m_WeaponsProp,true);
                        if (m_WeaponsProp.arraySize > 1)
                            EditorGUILayout.PropertyField(m_WeaponsCenterProp);
                        EditorGUILayout.PropertyField(m_AutoFireProp);
                        break;
                    case 1:
                        DrawHealthGUI();
                        EditorGUILayout.PropertyField(m_CrosshairGunProp);
                        EditorGUILayout.PropertyField(m_ReloadingUIProp);
                        break;
                    case 2:
                        DrawInteractGUI();
                        break;
                }
            }
            void DoStatistics()
            {
                EditorGUILayout.PropertyField(m_HealthProp);
                EditorGUILayout.PropertyField(m_TurretRotSpeedProp);
                EditorGUILayout.PropertyField(m_GunRotSpeedProp);
                EditorGUILayout.PropertyField(m_ElevationProp);
                EditorGUILayout.PropertyField(m_DepressionProp);
            }
        }
    }
}