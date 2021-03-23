using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PolyPerfect
{
    namespace War
    {
        [RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
        public class Grenade : MonoBehaviour, IPoolSystem
        {
            public float exposionRadius;
            public float damage;
            public GameObject exposion;
            private PoolSystem poolSystem;
            public float projectileSpeed;
            private Rigidbody rb;
            public float detonateTime;
            public bool detonateOnImpact;
            // Start is called before the first frame update
            public void OnPoolSpawn()
            {
                StartCoroutine(DestroyTimer(detonateTime));
                rb.AddForce(transform.forward * projectileSpeed);
                rb.AddForce(transform.up * projectileSpeed / 4);
                rb.AddTorque(new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10)));
            }
            private void Awake()
            {
                rb = GetComponent<Rigidbody>();
                poolSystem = PoolSystem.Instance;
            }
            private void OnCollisionEnter(Collision collision)
            {
                if (exposion != null && detonateOnImpact)
                {
                    poolSystem.Spawn(exposion, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
                    StaticHelperClass.DamageArea(collision.GetContact(0).point, exposionRadius, damage);
                    ResetVelocity();
                    poolSystem.poolDictionary["grenade-mk2"].Enqueue(gameObject);
                    gameObject.SetActive(false);
                }

            }
            void Detonate()
            {
                poolSystem.Spawn(exposion, transform.position, transform.rotation);
                StaticHelperClass.DamageArea(transform.position, exposionRadius, damage);
                ResetVelocity();
                poolSystem.poolDictionary["grenade-mk2"].Enqueue(gameObject);
                gameObject.SetActive(false);
            }
            IEnumerator DestroyTimer(float seconds)
            {
                yield return new WaitForSeconds(seconds);
                Detonate();
            }
            void ResetVelocity()
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}