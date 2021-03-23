using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
namespace PolyPerfect
{
    namespace War
    {
        // Types of paths enemy could walk on
        public enum PathType
        {
            Rounded,
            OneWay,
            BothWays
        }

        public class NavPath : MonoBehaviour
        {

            // Types of paths enemy could walk on
            public PathType pathType;
            [HideInInspector]
            // List of paths checkpoins
            public List<Vector3> pathPositions = new List<Vector3>();

            private bool walkingBack = false;

            // This function returns next target chekpoint depedent on type of path
            public Vector3 GetNextTarget(ref int currentTarget)
            {
                if (currentTarget == pathPositions.Count - 1)
                {
                    if (pathType == PathType.Rounded)
                        currentTarget = 0;
                    if (pathType == PathType.BothWays)
                    {
                        walkingBack = true;
                        currentTarget--;
                    }
                }
                else if (walkingBack)
                {
                    if (currentTarget == 0)
                    {
                        walkingBack = false;
                        currentTarget++;
                    }
                    else
                        currentTarget--;
                }
                else
                    currentTarget++;

                return pathPositions[currentTarget];
            }
            // This function returns current target chekpoint
            public Vector3 GetCurrentTarget(int currentTarget)
            {
                return pathPositions[currentTarget];
            }
        }
        // Editor custom inspector
        // Handles visual rendering and editing of path
        #region Editor
#if UNITY_EDITOR
        [CustomEditor(typeof(NavPath))]
        public class CustomNavPathEditor : Editor
        {
            SerializedProperty m_PathPositionProp;
            Vector3 currPos;
            int editingID = 0;
            bool editing = true;
            ReorderableList reorderableList;
            NavPath navPath;
            private void OnDisable()
            {
                Tools.hidden = false;
            }
            private void OnEnable()
            {
                navPath = target as NavPath;
                m_PathPositionProp = serializedObject.FindProperty("pathPositions");
                reorderableList = new ReorderableList(serializedObject, m_PathPositionProp, true, true, true, true);
                reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFosused) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth - 50, EditorGUIUtility.singleLineHeight), (index + 1).ToString() + ". Checkpoint");
                    if (isFosused)
                    {
                        currPos = navPath.pathPositions[index];
                        editingID = index;
                    }
                };
                reorderableList.onAddCallback = (ReorderableList list) =>
                {
                    reorderableList.serializedProperty.InsertArrayElementAtIndex(reorderableList.serializedProperty.arraySize);
                };
                reorderableList.onRemoveCallback = (ReorderableList list) =>
                {
                    if (EditorUtility.DisplayDialog("Delete Checkpoint?", "Are you sure you want remove checkpoint from database?", "Delete", "Cancel"))
                    {
                        if (navPath.pathPositions[list.index] != null)
                        {
                            ReorderableList.defaultBehaviours.DoRemoveButton(list);
                        }
                    }
                };
                reorderableList.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "NavPath Checkpoints");
                    editing = GUI.Toggle(new Rect(rect.x + 150, rect.y + 2, EditorGUIUtility.currentViewWidth - 200, EditorGUIUtility.singleLineHeight - 2), editing, "Edit", "Button");
                };
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                serializedObject.Update();
                reorderableList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
            void OnSceneGUI()
            {
                if (editing)
                {

                    EditorGUI.BeginChangeCheck();
                    currPos = Handles.PositionHandle(navPath.pathPositions[editingID], Quaternion.LookRotation(Vector3.forward));
                    navPath.transform.position = currPos;
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Changed Look Target");
                        navPath.pathPositions[editingID] = currPos;
                    }
                }
                Tools.hidden = true;
                for (int i = 0; i < m_PathPositionProp.arraySize; i++)
                {
                    if (navPath.pathPositions[i] != null)
                    {
                        if (navPath.pathType == PathType.Rounded && i == m_PathPositionProp.arraySize - 1)
                        {
                            Handles.color = Color.white;
                            Handles.DrawLine(navPath.pathPositions[i], navPath.pathPositions[0]);
                            Handles.color = Color.blue;
                            Handles.ArrowHandleCap(0, navPath.pathPositions[i], Quaternion.LookRotation(navPath.pathPositions[0] - navPath.pathPositions[i]), 3f, EventType.Repaint);
                        }
                        else if (i < m_PathPositionProp.arraySize - 1)
                        {
                            Handles.color = Color.white;
                            Handles.DrawLine(navPath.pathPositions[i], navPath.pathPositions[i + 1]);
                            if (navPath.pathType == PathType.BothWays)
                            {
                                Handles.color = Color.red;
                                Handles.ArrowHandleCap(0, navPath.pathPositions[i + 1], Quaternion.LookRotation(navPath.pathPositions[i] - navPath.pathPositions[i + 1]), 3f, EventType.Repaint);
                            }
                            Handles.color = Color.blue;
                            Handles.ArrowHandleCap(0, navPath.pathPositions[i], Quaternion.LookRotation(navPath.pathPositions[i + 1] - navPath.pathPositions[i]), 3f, EventType.Repaint);
                        }
                    }
                    if (i == 0)
                        Handles.color = Color.blue;
                    else if (i == m_PathPositionProp.arraySize - 1)
                        Handles.color = Color.red;
                    else
                        Handles.color = Color.white;
                    Handles.SphereHandleCap(0, navPath.pathPositions[i], Quaternion.LookRotation(navPath.pathPositions[i]), 0.2f, EventType.Repaint);

                }
            }
        }
#endif
        #endregion
    }
}