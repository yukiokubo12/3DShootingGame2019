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
        public enum ShootingType
        {
            Raycast,
            Projectile
        }
        public enum WeaponType
        {
            Firearm,
            AntitankGun,
            AntiaircraftGun,
            TankGun,
            Aircraft

        }
        public enum ProjectileType
        {
            Bullet,
            Rocket,
            TankShell,
            Bomb
        }
        [RequireComponent(typeof(AudioSource))]
        public class Weapon : MonoBehaviour
        {
            public ShootingType shootingType = ShootingType.Projectile;
            [Tooltip("How fast this weapon fires")]
            public WeaponType weaponType = WeaponType.Firearm;
            [Range(1, 200)]
            [Tooltip("Number of rounds in one magazine")]
            public int magazine = 0;
            [Tooltip("Amount of health weapon deal")]
            [Range(0f, 1000f)]
            public float weaponDamage = 0f;
            [Tooltip("How many shots will weapon fire in one second")]
            [Range(0f, 30f)]
            public float fireRate = 0f;
            [Tooltip("Time that takes weapon to reload")]
            [Range(0f, 10f)]
            public float reloadTime = 1f;
            [Tooltip("Time in sec to switch to this weapon")]
            [Range(0f, 10f)]
            public float weaponSwitchTime = 0.8f;
            [Tooltip("Trail after bullet")]
            public GameObject PrefabRayTrail;
            [Tooltip("Projectile that will be fired")]
            public GameObject hitParticles;
            [Tooltip("Projectile that will be fired")]
            public GameObject Projectile;
            [AmmoType]
            [Tooltip("Select ammunition type that will weapon use")]
            public int ammoType = 0;
            [HideInInspector]
            public int weaponId;
            public string weaponName = "";
            [Tooltip("Partical effect that will show on weapon fire")]
            public GameObject weaponFlash;
            [Tooltip("Weapon fire sound")]
            public AudioClip weaponFireSound;

            public AudioClip weaponReloadSound;

            [Tooltip("This key you want to ")]
            public KeyCode weaponSwitchKey = KeyCode.Alpha1;

            [Tooltip("The tip of the weapon, make an empty gameobject and move it to the end of the weapon")]
            public Transform weaponEnd;

            [Tooltip("So that left hand can reach to the IK point, makes it look like it is holding the weapon")]
            public Transform leftHandPosition;

            public Sprite weaponUISprite;

            public Sprite weaponUISpriteDisable;
            public Sprite crosshair;
            public int currentMagazine;
            private AudioSource audioSource;
            private bool canFire = true;

            public GameObject modelMagazine;
            public Animator playerAnimator;

            private void Awake()
            {
                audioSource = GetComponent<AudioSource>();
                playerAnimator = GetComponentInParent<Animator>();
            }
            public void Shoot(Vector3 weaponEndVelocity)
            {
                if (canFire||magazine == 1)
                {
                    currentMagazine--;
                    if (weaponFlash != null)
                    {
                        //Move the weapon flash to the correct location
                        GameObject obj = PoolSystem.Instance.Spawn(weaponFlash, weaponEnd.transform.position, weaponEnd.transform.rotation);
                        obj.transform.SetParent(weaponEnd);
                        if (weaponType != WeaponType.Aircraft)
                        {
                            obj.GetComponent<Animator>().SetTrigger("Fire");
                            playerAnimator.SetTrigger("Fire");
                        }
                    }


                    if (shootingType == ShootingType.Projectile)
                    {
                        GameObject obj = PoolSystem.Instance.Spawn(Projectile, weaponEnd.position, weaponEnd.rotation);
                        obj.GetComponent<Projectile>().bulletrigidbody.velocity = weaponEndVelocity;
                        obj.GetComponent<Projectile>().weaponDamage = weaponDamage;
                    }
                    else
                    {
                        if (Physics.Raycast(weaponEnd.position, weaponEnd.forward, out RaycastHit hit, Mathf.Infinity))
                        {
                            if (hit.collider.attachedRigidbody != null)
                            {
                                IDamageable<float> target = hit.collider.attachedRigidbody.gameObject.GetComponent<IDamageable<float>>();
                                if (target != null)
                                    target.TakeDamage(weaponDamage);
                            }
                            PoolSystem.Instance.Spawn(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                        }

                    }
                    if (audioSource != null)
                    {
                        audioSource.PlayOneShot(weaponFireSound);
                    }
                    canFire = false;
                    StartCoroutine(WaitForFire());
                }
            }
            public void Shoot(float damageMultiplayer)
            {
                if (canFire||magazine==1)
                {
                    currentMagazine--;
                    if (weaponFlash != null)
                    {
                        //Move the weapon flash to the correct location
                        GameObject obj = PoolSystem.Instance.Spawn(weaponFlash, weaponEnd.transform.position, weaponEnd.transform.rotation);
                        obj.transform.SetParent(weaponEnd);
                        obj.GetComponent<Animator>().SetTrigger("Fire");
                    }


                    if (shootingType == ShootingType.Projectile)
                    {
                        GameObject obj = PoolSystem.Instance.Spawn(Projectile, weaponEnd.position, weaponEnd.rotation);
                        obj.GetComponent<Projectile>().weaponDamage = weaponDamage * damageMultiplayer;
                    }
                    else
                    {
                        if (Physics.Raycast(weaponEnd.position, weaponEnd.forward, out RaycastHit hit, Mathf.Infinity))
                        {
                            if (hit.collider.attachedRigidbody != null)
                            {
                                IDamageable<float> target = hit.collider.attachedRigidbody.gameObject.GetComponent<IDamageable<float>>();
                                if (target != null)
                                    target.TakeDamage(weaponDamage * damageMultiplayer);
                            }
                            PoolSystem.Instance.Spawn(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                        }

                    }
                    if (audioSource != null)
                    {
                        audioSource.PlayOneShot(weaponFireSound);
                    }
                    canFire = false;
                    StartCoroutine(WaitForFire());
                }
            }
            public int Reload(int inventoryAmmo)
            {
                int remainingAmmo = currentMagazine;
                audioSource.PlayOneShot(weaponReloadSound);
                if (inventoryAmmo >= magazine)
                    currentMagazine = magazine;
                else
                    currentMagazine = inventoryAmmo;
                return currentMagazine - remainingAmmo;
            }
            public bool IsMagazineFull()
            {
                return currentMagazine == magazine ? true : false; 
            }
            IEnumerator WaitForFire()
            {
                yield return new WaitForSeconds(60 / (fireRate * 60));
                canFire = true;
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Weapon)), CanEditMultipleObjects]
        public class CustomWeaponEditor : Editor
        {
            SerializedProperty m_WeaponNameProp;
            SerializedProperty m_WeaponTypeProp;
            SerializedProperty m_ShootingTypeProp;
            SerializedProperty m_WeaponSwitchTimeProp;
            SerializedProperty m_FireRateProp;
            SerializedProperty m_ReloadTimeProp;
            SerializedProperty m_ClipSizeProp;
            SerializedProperty m_DamageProp;
            SerializedProperty m_ProjectileProp;
            SerializedProperty m_AmmoProp;
            SerializedProperty m_EndPointProp;
            SerializedProperty m_LeftHandProp;
            SerializedProperty m_WeaponFlashProp;
            SerializedProperty m_FireAudioClipProp;
            SerializedProperty m_ReloadAudioClipProp;
            SerializedProperty m_PrefabRayTrailProp;
            SerializedProperty m_WeaponUIProp;
            SerializedProperty m_CrosshairProp;
            SerializedProperty m_WeaponUIDisableProp;
            SerializedProperty m_HitParticlesProp;
            SerializedProperty m_WeaponMag;

            void OnEnable()
            {
                m_WeaponNameProp = serializedObject.FindProperty("weaponName");
                m_ShootingTypeProp = serializedObject.FindProperty("shootingType");
                m_WeaponTypeProp = serializedObject.FindProperty("weaponType");
                m_WeaponSwitchTimeProp = serializedObject.FindProperty("weaponSwitchTime");
                m_FireRateProp = serializedObject.FindProperty("fireRate");
                m_ReloadTimeProp = serializedObject.FindProperty("reloadTime");
                m_ClipSizeProp = serializedObject.FindProperty("magazine");
                m_DamageProp = serializedObject.FindProperty("weaponDamage");
                m_EndPointProp = serializedObject.FindProperty("weaponEnd");
                m_LeftHandProp = serializedObject.FindProperty("leftHandPosition");
                m_PrefabRayTrailProp = serializedObject.FindProperty("PrefabRayTrail");
                m_WeaponFlashProp = serializedObject.FindProperty("weaponFlash");
                m_FireAudioClipProp = serializedObject.FindProperty("weaponFireSound");
                m_ReloadAudioClipProp = serializedObject.FindProperty("weaponReloadSound");
                m_ProjectileProp = serializedObject.FindProperty("Projectile");
                m_AmmoProp = serializedObject.FindProperty("ammoType");
                m_WeaponUIProp = serializedObject.FindProperty("weaponUISprite");
                m_WeaponUIDisableProp = serializedObject.FindProperty("weaponUISpriteDisable");
                m_CrosshairProp = serializedObject.FindProperty("crosshair");
                m_HitParticlesProp = serializedObject.FindProperty("hitParticles");
                m_WeaponMag = serializedObject.FindProperty("modelMagazine");
            }
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                EditorGUILayout.LabelField("Weapon characteristics", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_WeaponNameProp);
                EditorGUILayout.PropertyField(m_ShootingTypeProp);
                EditorGUILayout.PropertyField(m_WeaponTypeProp);
                if (m_WeaponTypeProp.intValue == (int)WeaponType.Firearm)
                {
                    EditorGUILayout.PropertyField(m_AmmoProp);
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Weapon prefabs", EditorStyles.boldLabel);
                if (m_ShootingTypeProp.intValue == (int)ShootingType.Projectile)
                {
                    EditorGUILayout.PropertyField(m_ProjectileProp);
                }
                else
                    EditorGUILayout.PropertyField(m_HitParticlesProp);

                EditorGUILayout.PropertyField(m_PrefabRayTrailProp);
                EditorGUILayout.PropertyField(m_WeaponFlashProp);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Weapon setup", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_EndPointProp);
                EditorGUILayout.PropertyField(m_WeaponMag);

                if (m_WeaponTypeProp.intValue == (int)WeaponType.Firearm)
                {
                    EditorGUILayout.PropertyField(m_LeftHandProp);
                }


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Weapon statistics", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_DamageProp);
                if (m_WeaponTypeProp.intValue != (int)WeaponType.Aircraft)
                    EditorGUILayout.PropertyField(m_ClipSizeProp);
                if (m_ClipSizeProp.intValue != 1|| m_WeaponTypeProp.intValue == (int)WeaponType.Aircraft)
                    EditorGUILayout.PropertyField(m_FireRateProp);
                if(m_WeaponTypeProp.intValue != (int)WeaponType.Aircraft)
                    EditorGUILayout.PropertyField(m_ReloadTimeProp);
                if (m_WeaponTypeProp.intValue == (int)WeaponType.Firearm)
                {
                    EditorGUILayout.PropertyField(m_WeaponSwitchTimeProp);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("UI sprites", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(m_WeaponUIProp);
                    EditorGUILayout.PropertyField(m_WeaponUIDisableProp);
                    EditorGUILayout.PropertyField(m_CrosshairProp);
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_FireAudioClipProp);
                EditorGUILayout.PropertyField(m_ReloadAudioClipProp);

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}