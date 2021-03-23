using UnityEngine;
using UnityEditor;

namespace PolyPerfect
{
    namespace War
    {
        // Editor window for ammution and weapons creation/database 
        public class AssetEditor : EditorWindow
        {
            //Tools toolbar tabs names and open tab id 
            int selectedTool = 0;
            string[] tools = new string[] { "Asset Creator", "Asset Database" };
            //Creator toolbar tabs names and open tab id 
            int selectedCreatorTool = 0;
            string[] creatorTools = new string[] { "Ammo", "Weapon" };

            GameObject preview;
            GUIStyle bgColor;

            Vector2 scrollPos;

            string ammoName;
            ProjectileType projectileType;

            bool toggle = false;

            GameDatabase db;

            public AmmoBox ammoBox;
            public GameObject newWeapon;

            [MenuItem("Tools/Asset Editor")]
            public static void ShowWindow()
            {
                GetWindow<AssetEditor>("Asset Editor");
            }
            private void OnEnable()
            {
                preview = Resources.Load<GameObject>("Preview");
                if (preview == null)
                {
                    Debug.LogError("Preview prefab is missing in Resources folder");
                }
                db = GameDatabase.Instance;
                bgColor = new GUIStyle();
                bgColor.normal.background = EditorGUIUtility.whiteTexture;
            }
            Rect buttonRect;
            private void OnGUI()
            {

                selectedTool = GUILayout.Toolbar(selectedTool, tools);
                switch (selectedTool)
                {
                    case 0:
                        AssetCreator();
                        break;
                    case 1:
                        AssetDatabase();
                        break;
                }

            }
            void AssetCreator()
            {
                selectedCreatorTool = GUILayout.Toolbar(selectedCreatorTool, creatorTools);
                switch (selectedCreatorTool)
                {
                    case 0:
                        GUILayout.Label("Creation system for new ammunition types", EditorStyles.boldLabel);
                        ammoName = EditorGUILayout.TextField("Ammunition name", ammoName);
                        projectileType = (ProjectileType)EditorGUILayout.EnumPopup("Projectile type", projectileType);

                        if (GUILayout.Button("Add"))
                        {
                            db.AddAmmoType(ammoName, projectileType);
                            Undo.RecordObject(db, "Add ammo");
                            EditorUtility.SetDirty(db);
                        }
                        break;
       
                    case 1:
                        GUILayout.Label("Creation system for new weapons", EditorStyles.boldLabel);
                        newWeapon = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Weapon", "Weapon prefab that you want to add to the database"), newWeapon, typeof(GameObject), true);
                        if (GUILayout.Button("Add"))
                        {
                            if (newWeapon != null)
                            {
                                if (newWeapon.GetComponent<Weapon>() != null)
                                {
                                    db.AddWeapon(newWeapon);
                                    newWeapon = null;
                                    Undo.RecordObject(db, "Add weapon");
                                    EditorUtility.SetDirty(db);
                                }
                                else
                                    EditorUtility.DisplayDialog("Wrong prefab!", "Selected prefab does not contain Weapon script!", "Ok");
                            }
                        }
                        break;

                }
            }
            void AssetDatabase()
            {

                var ammoDB = db.ammoDatabase;
                var weaponDB = db.weaponDatabase;

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxWidth(2000), GUILayout.MaxHeight(1080));
                GUILayout.BeginVertical("GroupBox");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Ammunition database", EditorStyles.boldLabel);
                toggle = GUILayout.Toggle(toggle, "Edit", "Button", GUILayout.MinWidth(64));
                EditorGUILayout.EndHorizontal();


                foreach (Ammo ammo in ammoDB)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (toggle)
                    {
                        ammo.ammoName = EditorGUILayout.TextField(ammo.ammoName, GUILayout.MinHeight(20));
                        ammo.projectileType = (ProjectileType)EditorGUILayout.EnumPopup(ammo.projectileType, GUILayout.MinHeight(20), GUILayout.Height(20));
                        if (GUILayout.Button("Delete", GUILayout.MinWidth(64)))
                        {
                            if (EditorUtility.DisplayDialog("Delete ammunition?", "Are you sure you want remove ammunition from database?", "Delete", "Cancel"))
                            {
                                db.DeleteAmmoType(ammo);
                                Undo.RecordObject(db, "Delete ammo");
                                EditorUtility.SetDirty(db);
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Box(ammo.ammoName, GUILayout.MaxWidth(1000));
                        GUILayout.Box(ammo.projectileType.ToString(), GUILayout.MaxWidth(1000), GUILayout.MinWidth(75));
                    }

                    EditorGUILayout.EndHorizontal();

                }
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical("GroupBox");
                GUILayout.Label("Weapon database", EditorStyles.boldLabel);

                foreach (GameObject weapon in weaponDB)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Box(weapon.name, GUILayout.MaxWidth(1000));
                    buttonRect = GUILayoutUtility.GetLastRect();
                    if (GUILayout.Button("Edit", GUILayout.MinWidth(100), GUILayout.MinHeight(20)))
                    {
                        PopupWindow.Show(buttonRect, new PopupExample(weapon.GetComponent<Weapon>()));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }


        }
        public class PopupExample : PopupWindowContent
        {
            Weapon weapon;
            GameDatabase db;
            public PopupExample(Weapon _weapon)
            {
                weapon = _weapon;
                db = GameDatabase.Instance;
            }
            public override Vector2 GetWindowSize()
            {
                return new Vector2(500,400);
            }

            public override void OnGUI(Rect rect)
            {
                EditorGUILayout.LabelField("Weapon characteristics", EditorStyles.boldLabel);
                weapon.weaponName = EditorGUILayout.TextField(new GUIContent("Weapon name", "Name of the gun"), weapon.weaponName);
                weapon.shootingType = (ShootingType)EditorGUILayout.EnumPopup(new GUIContent("Shooting type", "How fast this weapon fires"), weapon.shootingType);
                weapon.weaponType = (WeaponType)EditorGUILayout.EnumPopup(new GUIContent("Weapon type", "How fast this weapon fires"), weapon.weaponType);
                weapon.ammoType = EditorGUILayout.Popup(new GUIContent("Ammo type", "Select ammunition type that will weapon use"), weapon.ammoType, db.GetAmmoAllNames());
                EditorGUILayout.LabelField("Weapon statistics", EditorStyles.boldLabel);
                weapon.fireRate = EditorGUILayout.Slider(new GUIContent("Firerate", "How many shots will weapon fire in one second"), weapon.fireRate, 0, 60);
                weapon.reloadTime = EditorGUILayout.Slider(new GUIContent("Reload time", "Time that takes weapon to reload"), weapon.reloadTime, 0, 10);
                weapon.magazine = EditorGUILayout.IntSlider(new GUIContent("Magazine size", "Number of rounds in one magazine"), weapon.magazine, 0, 1000);
                weapon.weaponDamage = EditorGUILayout.Slider(new GUIContent("Damage", "Amount of health weapon dealt"), weapon.weaponDamage, 0, 1000);
                weapon.weaponSwitchTime = EditorGUILayout.Slider(new GUIContent("Weapon switch time", "Time in sec to switch to this weapon"), weapon.weaponSwitchTime, 0, 5);
                EditorGUILayout.LabelField("Weapon prefabs", EditorStyles.boldLabel);
                weapon.PrefabRayTrail = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Ray trail", "Trail after bullet"), weapon.PrefabRayTrail, typeof(GameObject), true);
                if (weapon.shootingType == ShootingType.Projectile)
                    weapon.Projectile = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Projectile", "Projectile that will fire"), weapon.Projectile, typeof(GameObject), true);
                weapon.weaponFlash = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Weapon flash", "Partical effect that will show on weapon fire"), weapon.weaponFlash, typeof(GameObject), true);
                EditorGUILayout.LabelField("Weapon setup", EditorStyles.boldLabel);
                weapon.weaponFireSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Weapon sound", "Weapon fire sound"), weapon.weaponFireSound, typeof(AudioClip), true);
                weapon.weaponReloadSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Weapon reload sound", "Weapon reload sound"), weapon.weaponReloadSound, typeof(AudioClip), true);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OK", GUILayout.MinWidth(64)))
                {
                    editorWindow.Close();
                }
                if (GUILayout.Button("Delete", GUILayout.MinWidth(64)))
                {

                    Undo.RecordObject(db, "Delete ammo");
                    EditorUtility.SetDirty(db);
                    if (EditorUtility.DisplayDialog("Delete ammunition?", "Are you sure you want remove ammunition from database?", "Delete", "Cancel"))
                        db.DeleteWeaponType(weapon.gameObject);
                }
            }
        }

    }
}