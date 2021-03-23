using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PolyPerfect
{
    namespace War
    {
        public class NavPathGameObj : Editor
        {
            [MenuItem("GameObject/Create Other/Nav Path")]
            public static void CreateNavPath()
            {
                GameObject obj = new GameObject("NavPath", typeof(NavPath));
                obj.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
                RaycastHit hit;
                if (Physics.Raycast(obj.transform.position, SceneView.lastActiveSceneView.camera.transform.forward, out hit))
                {
                    obj.transform.position = hit.point;
                }

                obj.GetComponent<NavPath>().pathPositions.Add(obj.transform.position);
                obj.GetComponent<NavPath>().pathPositions.Add(obj.transform.position + (Vector3.forward * 10));
                Selection.activeObject = obj;
            }
        }
    }
}
