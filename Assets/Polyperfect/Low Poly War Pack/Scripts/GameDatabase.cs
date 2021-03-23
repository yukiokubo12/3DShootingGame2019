using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Application = UnityEngine.Application;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PolyPerfect
{
    namespace War
    {
        public class GameDatabase : ScriptableObject
        {
            public static GameDatabase Instance
            {
                get
                {

                    if (s_Instance == null)
                    {
                        var db = Resources.Load<GameDatabase>("GameDatabase");
                        if (db == null)
                        {
#if UNITY_EDITOR
                            if (!Application.isPlaying)
                            {
                                db = CreateInstance<GameDatabase>();

                                if (!System.IO.Directory.Exists(Application.dataPath + "/Polyperfect/LowPolyWarPack/Resources"))
                                    AssetDatabase.CreateFolder("Assets/Polyperfect/LowPolyWarPack", "Resources");

                                AssetDatabase.CreateAsset(db, "Assets/Polyperfect/LowPolyWarPack/Resources/GameDatabase.asset");
                                AssetDatabase.Refresh();
                            }
                            else
                            {
                                Debug.LogError("Game Database couldn't be found.");
                                return null;
                            }

#endif
                        }
                        s_Instance = db;
                    }
                    return s_Instance;
                }

            }

            public int maxID = 0;
            public int maxWeaponID = 0;
            static GameDatabase s_Instance;
            public List<Ammo> ammoDatabase = new List<Ammo>();
            public List<GameObject> weaponDatabase = new List<GameObject>();
            public Queue<int> freeAmmoID = new Queue<int>();
            public Queue<int> freeWeaponID = new Queue<int>();
#if UNITY_EDITOR
            public void AddAmmoType(string name, ProjectileType type)
            {
                if (freeAmmoID.Count > 0)
                {
                    ammoDatabase.Add(new Ammo(freeAmmoID.Dequeue(), name, type));
                }
                else
                {
                    ammoDatabase.Add(new Ammo(maxID, name, type));
                    maxID++;
                }
                EditorUtility.SetDirty(this);
            }
            public void DeleteAmmoType(Ammo ammo)
            {
                if (ammoDatabase.Contains(ammo))
                {
                    ammoDatabase.Remove(ammo);
                    freeAmmoID.Enqueue(ammo.ammoType);
                }
                EditorUtility.SetDirty(this);
            }
#endif
            public string[] GetAmmoAllNames()
            {
                string[] s = new string[ammoDatabase.Count];
                int i = 0;
                foreach (Ammo ammo in ammoDatabase)
                {
                    s[i] = ammo.ammoName;
                    i++;
                }
                return s;
            }

            public int GetIdFromName(string name)
            {
                foreach (Ammo ammo in ammoDatabase)
                {
                    if (ammo.ammoName == name)
                        return ammo.ammoType;
                }
                return -1;
            }

            public string[] GetAllWeaponNames()
            {
                string[] s = new string[weaponDatabase.Count];
                int i = 0;
                foreach (GameObject weapon in weaponDatabase)
                {
                    s[i] = weapon.name;
                    i++;
                }
                return s;
            }
            public void AddWeapon(GameObject weapon)
            {
                if (weapon != null)
                    weaponDatabase.Add(weapon);
            }
            public void DeleteWeaponType(GameObject weapon)
            {
                if (weaponDatabase.Contains(weapon))
                    weaponDatabase.Remove(weapon);
            }
        }
#if UNITY_EDITOR
        class WeaponAssetDeleter : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (deletedAssets.Length > 0)
                {
                    foreach (GameObject obj in GameDatabase.Instance.weaponDatabase)
                    {
                        if (obj == null)
                            GameDatabase.Instance.weaponDatabase.Remove(obj);
                    }
                }
            }
        }
#endif
    }
}