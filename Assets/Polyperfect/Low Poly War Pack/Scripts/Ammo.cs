using UnityEngine;

namespace PolyPerfect
{
    namespace War
    {
        [System.Serializable]
        public class Ammo
        {
            [Tooltip("Name of ammo")]
            public string ammoName;
            public ProjectileType projectileType;
            public int ammoType;
            public Ammo(int id, string name, ProjectileType _projectileType)
            {
                ammoType = id;
                ammoName = name;
                projectileType = _projectileType;
            }

        }
        [System.Serializable]
        public class StartingAmmo
        {
            [HideInInspector]
            public int ammoType;
            public int count;
        }
    }
}
