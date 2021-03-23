using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyPerfect
{
    namespace War
    {
        public static class StaticHelperClass
        {
            public static float Remap(this float value, float from1, float to1, float from2, float to2)
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }
            public static void DamageArea(Vector3 originPoint, float radius, float damage)
            {
                var collidersInRange = Physics.OverlapSphere(originPoint, radius);
                List<int> damagedObjects = new List<int>();
                foreach (var item in collidersInRange)
                {
                    if (item.attachedRigidbody != null)
                    {
                        IDamageable<float> damageable = item.attachedRigidbody.gameObject.GetComponent<IDamageable<float>>();
                        if (damageable != null && !damagedObjects.Contains(item.attachedRigidbody.gameObject.GetInstanceID()))
                        {
                            damagedObjects.Add(item.attachedRigidbody.gameObject.GetInstanceID());
                            var distanceDivide = Vector3.Distance(item.transform.position, originPoint);
                            distanceDivide = Mathf.Clamp(distanceDivide, 1f, 1000f);
                            damageable.TakeDamage(damage / distanceDivide);
                        }
                        item.attachedRigidbody.AddExplosionForce(item.attachedRigidbody.mass/10, originPoint, radius, 1, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}