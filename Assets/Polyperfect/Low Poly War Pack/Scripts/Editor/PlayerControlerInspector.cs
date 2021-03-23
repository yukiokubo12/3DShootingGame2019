using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace PolyPerfect
{
    namespace War
    {
        [CustomEditor(typeof(PlayerController))]
        public class CustomControlerEditor : Editor
        {
            int id = -1;
            int weaponId = -1;
            SerializedProperty m_SpeedProp;
            SerializedProperty m_RunningSpeedProp;
            SerializedProperty m_MoveSmoothingProp;
            SerializedProperty m_TurnSmoothingProp;
            SerializedProperty m_HealthProp;
            SerializedProperty m_WeaponsProp;
            SerializedProperty m_InteractingImageProp;
            SerializedProperty m_MaxAmmoProp;
            SerializedProperty m_CurrentAmmoProp;
            SerializedProperty m_StartingAmmoProp;
            SerializedProperty m_WeaponUIsProp;
            SerializedProperty m_HealthTextProp;
            SerializedProperty m_HandProp;
            SerializedProperty m_HealthUIsProp;
            SerializedProperty m_HealthImageProp;
            SerializedProperty m_CanvasProp;
            SerializedProperty m_GrenadeProp;
            SerializedProperty m_IKProp;
            SerializedProperty m_GrenadePositionProp;
            SerializedProperty m_CrosshairProp;
            SerializedProperty m_DeadScreenProp;
            SerializedProperty m_SpawnPointProp;
            static bool foldout = false;
            public void OnEnable()
            {
                m_SpeedProp = serializedObject.FindProperty("speed");
                m_RunningSpeedProp = serializedObject.FindProperty("runningSpeed");
                m_WeaponsProp = serializedObject.FindProperty("weapons");
                m_MoveSmoothingProp = serializedObject.FindProperty("moveSmoothing");
                m_TurnSmoothingProp = serializedObject.FindProperty("turnSmoothing");
                m_StartingAmmoProp = serializedObject.FindProperty("startingAmmo");
                m_HealthProp = serializedObject.FindProperty("_health");
                m_WeaponUIsProp = serializedObject.FindProperty("weaponsUI");
                m_InteractingImageProp = serializedObject.FindProperty("interactImage");
                m_MaxAmmoProp = serializedObject.FindProperty("maxAmmo");
                m_CurrentAmmoProp = serializedObject.FindProperty("currentAmmo");
                m_HealthTextProp = serializedObject.FindProperty("healthText");
                m_HandProp = serializedObject.FindProperty("hand");
                m_HealthUIsProp = serializedObject.FindProperty("healthUis");
                m_HealthImageProp = serializedObject.FindProperty("healthImage");
                m_CanvasProp = serializedObject.FindProperty("canvas");
                m_IKProp = serializedObject.FindProperty("IK");
                m_GrenadeProp = serializedObject.FindProperty("grenade");
                m_GrenadePositionProp = serializedObject.FindProperty("grenadePosition");
                m_CrosshairProp = serializedObject.FindProperty("crosshair");
                m_DeadScreenProp = serializedObject.FindProperty("deathScreen");
                m_SpawnPointProp = serializedObject.FindProperty("spawnPoint");
            }
            public override void OnInspectorGUI()
            {
                List<string> names = new List<string>();
                GameDatabase db;
                db = GameDatabase.Instance;
                serializedObject.Update();
                EditorGUILayout.PropertyField(m_HealthProp);
                EditorGUILayout.PropertyField(m_SpeedProp);
                EditorGUILayout.PropertyField(m_RunningSpeedProp);
                EditorGUILayout.PropertyField(m_MoveSmoothingProp);
                EditorGUILayout.PropertyField(m_TurnSmoothingProp);
                EditorGUILayout.PropertyField(m_SpawnPointProp);

                foldout = EditorGUILayout.Foldout(foldout, "UI", true);
                if (foldout)
                {
                    EditorGUILayout.PropertyField(m_CanvasProp);
                    EditorGUILayout.PropertyField(m_InteractingImageProp);
                    EditorGUILayout.PropertyField(m_CurrentAmmoProp);
                    EditorGUILayout.PropertyField(m_MaxAmmoProp);
                    EditorGUILayout.PropertyField(m_HealthTextProp);
                    EditorGUILayout.PropertyField(m_HealthImageProp);
                    EditorGUILayout.PropertyField(m_CrosshairProp);
                    EditorGUILayout.PropertyField(m_WeaponUIsProp, true);
                    EditorGUILayout.PropertyField(m_HealthUIsProp, true);
                    EditorGUILayout.PropertyField(m_DeadScreenProp);
                    EditorGUILayout.Space();
                }



                EditorGUILayout.PropertyField(m_WeaponsProp);
                if (m_WeaponsProp.isExpanded)
                {

                    EditorGUILayout.PropertyField(m_HandProp);
                    EditorGUILayout.PropertyField(m_IKProp);
                    EditorGUILayout.PropertyField(m_GrenadeProp);
                    EditorGUILayout.PropertyField(m_GrenadePositionProp);
                    GUILayout.BeginVertical("GroupBox");
                    EditorGUILayout.LabelField("Slots", EditorStyles.boldLabel);
                    List<string> slotWeaponNames = new List<string>(db.GetAllWeaponNames());
                    slotWeaponNames.Insert(0, "Empty slot");
                    GUIStyle style = EditorStyles.popup;
                    style.fixedHeight = 18;
                    style.alignment = TextAnchor.MiddleCenter;

                    for (int i = 0; i < m_WeaponsProp.arraySize; i++)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinHeight(25));
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField("Slot " + (i + 1), EditorStyles.boldLabel, GUILayout.MaxWidth(45));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(5);
                        EditorGUILayout.BeginHorizontal();
                        m_WeaponsProp.GetArrayElementAtIndex(i).intValue = EditorGUILayout.Popup(m_WeaponsProp.GetArrayElementAtIndex(i).intValue, slotWeaponNames.ToArray(), style, GUILayout.MinHeight(20));

                        //  EditorGUILayout.LabelField(m_WeaponsProp.GetArrayElementAtIndex(i).FindPropertyRelative("weaponName").stringValue);
                        if (GUILayout.Button("Remove", GUILayout.MaxWidth(100)))
                        {
                            m_WeaponsProp.DeleteArrayElementAtIndex(i);
                        }

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(5);
                        EditorGUILayout.EndVertical();
                    }
                    style.fixedHeight = 0;
                    style.alignment = TextAnchor.MiddleLeft;
                    if (m_WeaponsProp.arraySize == 0)
                        EditorGUILayout.LabelField("List is empty!");
                    if (m_WeaponsProp.arraySize < 7)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        weaponId = EditorGUILayout.Popup(-1, slotWeaponNames.ToArray(), "Button", GUILayout.MaxWidth(150));
                        Rect rect = GUILayoutUtility.GetLastRect();
                        rect.x += 50;
                        GUI.Label(rect, "Add Slot");
                        if (weaponId != -1)
                        {
                            m_WeaponsProp.InsertArrayElementAtIndex(m_WeaponsProp.arraySize);
                            m_WeaponsProp.GetArrayElementAtIndex(m_WeaponsProp.arraySize - 1).intValue = weaponId;
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }

                EditorGUILayout.PropertyField(m_StartingAmmoProp);
                if (m_StartingAmmoProp.isExpanded)
                {
                    GUILayout.BeginVertical("GroupBox");
                    for (int i = 0; i < m_StartingAmmoProp.arraySize; i++)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField(db.ammoDatabase[m_StartingAmmoProp.GetArrayElementAtIndex(i).FindPropertyRelative("ammoType").intValue].ammoName, EditorStyles.boldLabel);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(m_StartingAmmoProp.GetArrayElementAtIndex(i).FindPropertyRelative("count"));
                        if (GUILayout.Button("Remove", GUILayout.MaxWidth(100)))
                        {
                            m_StartingAmmoProp.DeleteArrayElementAtIndex(i);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }

                    if (m_StartingAmmoProp.arraySize == 0)
                        EditorGUILayout.LabelField("List is empty!");
                    if (m_StartingAmmoProp.arraySize < db.ammoDatabase.Count)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        bool exist = false;
                        foreach (Ammo ammo in db.ammoDatabase)
                        {

                            for (int i = 0; i < m_StartingAmmoProp.arraySize; i++)
                            {
                                if (ammo.ammoType == m_StartingAmmoProp.GetArrayElementAtIndex(i).FindPropertyRelative("ammoType").intValue)
                                {
                                    exist = true;
                                }
                            }
                            if (!exist)
                            {
                                names.Add(ammo.ammoName);
                            }
                            exist = false;
                        }
                        id = EditorGUILayout.Popup(-1, names.ToArray(), "Button", GUILayout.MaxWidth(150));
                        Rect rect = GUILayoutUtility.GetLastRect();
                        rect.x += 60;
                        GUI.Label(rect, "Add");
                        if (id != -1)
                        {
                            int arrsize = m_StartingAmmoProp.arraySize;
                            m_StartingAmmoProp.InsertArrayElementAtIndex(arrsize);
                            m_StartingAmmoProp.GetArrayElementAtIndex(arrsize).FindPropertyRelative("ammoType").intValue = db.GetIdFromName(names[id]);
                            serializedObject.ApplyModifiedProperties();
                            id = -1;
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }

                //    EditorGUILayout.PropertyField(m_CameraSetUpProp, true);
                //  EditorGUILayout.PropertyField(m_IKSetUpProp, true);

                serializedObject.ApplyModifiedProperties();
            }

        }
    }
}
