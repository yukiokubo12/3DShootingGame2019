using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace PolyPerfect
{
    namespace War
    {
        public class Projectile : MonoBehaviour, IPoolSystem
        {

            [Tooltip("If you put a gameobject in this slot, it will be spawned at the position of the hit")]
            public GameObject hitEffect;

            [Tooltip("How fast the Projectile will travel")]
            public float projectileSpeed;

            [Tooltip("Will the weapon damage around the point of impact e.g. Bazzoka")]
            public bool areaDamage;

            [Tooltip("How far away from the point of impact will the damage effect")]
            public float areaRadius;

            public float destroyTime;

            public string bulletName;

            [HideInInspector]
            public float weaponDamage;

            public Rigidbody bulletrigidbody;

            PoolSystem poolSystem;

            public void OnPoolSpawn()
            {
                StartCoroutine(DestroyTimer(destroyTime));
                bulletrigidbody.AddForce(transform.forward * projectileSpeed);
            }
            private void Awake()
            {
                poolSystem = PoolSystem.Instance;
            }

            private void OnCollisionEnter(Collision other)
            {

                if (hitEffect != null)
                {
                    if (other.gameObject.GetComponent<Enemy>() != null)
                        hitEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial.color = Color.red;
                    else
                        hitEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial.color = new Color(1, 0.99f, 0.8f);
                    poolSystem.Spawn(hitEffect, other.GetContact(0).point, Quaternion.LookRotation(other.GetContact(0).normal));
                    if (areaDamage)
                        StaticHelperClass.DamageArea(other.GetContact(0).point, areaRadius, weaponDamage);
                }
                ResetVelocity();
                poolSystem.poolDictionary[bulletName].Enqueue(gameObject);
                gameObject.SetActive(false);

            }


            IEnumerator DestroyTimer(float seconds)
            {
                yield return new WaitForSeconds(seconds);
                ResetVelocity();
                poolSystem.poolDictionary[bulletName].Enqueue(gameObject);
                gameObject.SetActive(false);

            }
            void ResetVelocity()
            {
                bulletrigidbody.velocity = Vector3.zero;
                bulletrigidbody.angularVelocity = Vector3.zero;
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Projectile))]
        public class CustomProjectileEditor : Editor
        {
            SerializedProperty m_HitEffectProp;
            SerializedProperty m_ProjectileSpeedProp;
            SerializedProperty m_AreaDamageProp;
            SerializedProperty m_AreaRadiusProp;
            SerializedProperty m_DestroyTime;
            SerializedProperty m_RigidbodyProp;
            SerializedProperty m_NameProp;


            void OnEnable()
            {
                m_HitEffectProp = serializedObject.FindProperty("hitEffect");
                m_ProjectileSpeedProp = serializedObject.FindProperty("projectileSpeed");
                m_AreaDamageProp = serializedObject.FindProperty("areaDamage");
                m_AreaRadiusProp = serializedObject.FindProperty("areaRadius");
                m_DestroyTime = serializedObject.FindProperty("destroyTime");
                m_RigidbodyProp = serializedObject.FindProperty("bulletrigidbody");
                m_NameProp = serializedObject.FindProperty("bulletName");
            }
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(m_NameProp);
                EditorGUILayout.PropertyField(m_HitEffectProp);
                EditorGUILayout.PropertyField(m_ProjectileSpeedProp);
                EditorGUILayout.PropertyField(m_DestroyTime);
                EditorGUILayout.PropertyField(m_RigidbodyProp);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_AreaDamageProp);
                if (m_AreaDamageProp.boolValue == true)
                {
                    EditorGUILayout.PropertyField(m_AreaRadiusProp);
                }

                serializedObject.ApplyModifiedProperties();

            }
        }
#endif
    }
}