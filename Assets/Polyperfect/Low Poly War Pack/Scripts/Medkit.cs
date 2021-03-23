#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
namespace PolyPerfect
{
    namespace War
    {
        public class Medkit : PickableObject
        {
            public static List<Medkit> medkits = new List<Medkit>();
            // Amount of health that will medkit heal
            public float healAmount;
            // Boolean that determints if medkit will add health immidiently (true) or over time (false)
            public bool instantHeal;
            // Time in seconds in wich medkit will aply all healing
            [HideInInspector]
            public float healDuration;
            // Function that handles picking up of medkit
            private void Awake()
            {
                medkits.Add(this);
            }
            protected override void PickUp(IPickable player)
            {
                if (player.currentHealth < player.health)
                {
                    base.PickUp(player);
                    player.PickUpMedkit(this);
                    medkits.Remove(this);
                }
            }
        }
        // Custom editor inspector
#if UNITY_EDITOR
        [CustomEditor(typeof(Medkit))]
        public class CustomMedkitEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (!serializedObject.FindProperty("instantHeal").boolValue)
                {
                    serializedObject.Update();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("healDuration"));
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif
    }

}