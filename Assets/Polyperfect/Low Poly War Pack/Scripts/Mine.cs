using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace PolyPerfect
{
    namespace War
    {
        public class Mine : MonoBehaviour
        {
            // Range in meters in wich damage will be applied
            public float exposionRadius;
            // Maximum damage that mine will do
            public float damage;
            // Prefab that will be spawn on explosion
            public GameObject exposion;

            private void OnTriggerEnter(Collider other)
            {
                if (!other.isTrigger)
                {
                    StaticHelperClass.DamageArea(transform.position, exposionRadius, damage);
                    PoolSystem.Instance.Spawn(exposion, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
            private void OnCollisionEnter(Collision collision)
            {
                var projectile = collision.transform.GetComponent<Projectile>();
                if (projectile != null)
                {
                    PoolSystem.Instance.Spawn(exposion, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
        }
    }
}
