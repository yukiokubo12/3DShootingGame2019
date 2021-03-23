using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PolyPerfect
{
    namespace War
    {
        [RequireComponent(typeof(SphereCollider), typeof(AudioSource))]
        public class PickableObject : MonoBehaviour
        {
            public GameObject pickUpAudio;
            public float pickUpRange;
            private PoolSystem poolSystem;
            protected SphereCollider sphereCollider;
            private void Reset()
            {
                sphereCollider = GetComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
            }
            private void Start()
            {
                poolSystem = PoolSystem.Instance;
                sphereCollider = GetComponent<SphereCollider>();
                sphereCollider.radius = pickUpRange;
            }
            private void OnTriggerEnter(Collider other)
            {
                IPickable pickingPerson = other.GetComponentInParent<IPickable>();
                if (pickingPerson != null && !other.isTrigger)
                {
                    PickUp(pickingPerson);
                }
            }
            protected virtual void PickUp(IPickable pickingPerson)
            {
                poolSystem.Spawn(pickUpAudio,transform.position,Quaternion.identity);
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}