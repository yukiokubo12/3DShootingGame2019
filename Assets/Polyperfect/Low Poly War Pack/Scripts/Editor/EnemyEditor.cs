using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace PolyPerfect
{
    namespace War
    {
        [CustomEditor(typeof(Enemy))]
        [CanEditMultipleObjects]
        public class CustomEnemyEditor : Editor
        {
            int id = -1;
            int weaponId = -1;
            bool foldoutWeapon = false;
            bool foldoutAmmo = false;
            SerializedProperty m_WeaponsProp;
            SerializedProperty m_StartingAmmoProp;
            SerializedProperty m_HandProp;
            private void OnEnable()
            {
                m_StartingAmmoProp = serializedObject.FindProperty("startingAmmo");
                m_WeaponsProp = serializedObject.FindProperty("weapons");
                m_HandProp = serializedObject.FindProperty("hand");
            }
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                List<string> names = new List<string>();
                GameDatabase db;
                db = GameDatabase.Instance;
                serializedObject.Update();

                foldoutWeapon = EditorGUILayout.Foldout(foldoutWeapon, "Weapons", true);
                if (foldoutWeapon)
                {
                    GUILayout.BeginVertical("GroupBox");
                    EditorGUILayout.PropertyField(m_HandProp);
                    for (int i = 0; i < m_WeaponsProp.arraySize; i++)
                    {
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        EditorGUILayout.PropertyField(m_WeaponsProp.GetArrayElementAtIndex(i), GUIContent.none);
                        //  EditorGUILayout.LabelField(m_WeaponsProp.GetArrayElementAtIndex(i).FindPropertyRelative("weaponName").stringValue);
                        if (GUILayout.Button("Remove", GUILayout.MaxWidth(100)))
                        {
                            m_WeaponsProp.DeleteArrayElementAtIndex(i);
                            m_WeaponsProp.DeleteArrayElementAtIndex(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (m_WeaponsProp.arraySize == 0)
                        EditorGUILayout.LabelField("List is empty!");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    weaponId = EditorGUILayout.Popup(-1, db.GetAllWeaponNames(), "Button", GUILayout.MaxWidth(150));
                    Rect rect = GUILayoutUtility.GetLastRect();
                    rect.x += 60;
                    GUI.Label(rect, "Add");
                    if (weaponId != -1)
                    {
                        GameObject weapon = db.weaponDatabase[weaponId];
                        //   m_WeaponsProp.InsertArrayElementAtIndex(index);
                        //  m_WeaponsProp.GetArrayElementAtIndex(index).FindPropertyRelative("weaponData").objectReferenceValue = db.weaponDatabase[weaponId];
                        Enemy playerController = target as Enemy;
                        playerController.weapons.Add(weapon);
                        EditorUtility.SetDirty(playerController);
                        Debug.Log(playerController.weapons.Count);
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                foldoutAmmo = EditorGUILayout.Foldout(foldoutAmmo, "Ammo", true);
                if (foldoutAmmo)
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
                    GUILayout.EndVertical();
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}