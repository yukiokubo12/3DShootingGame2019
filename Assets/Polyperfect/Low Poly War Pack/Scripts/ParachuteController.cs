using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PolyPerfect
{
    namespace War
    {
        [RequireComponent(typeof(Rigidbody))]
        public class ParachuteController : MonoBehaviour
        {
            private Rigidbody rBody;
            // Resistance of air, the higher value is the slower will parachute fall
            public float windResistance = 5;
            public GameObject player;
            // Position that player will be 
            public Transform playerSlot;
            // Area that is responsible for simulating air resistance
            public Wing parachute;
            // Virtual camera that will be active on using parachute
            public VirtualCamera virtualCamera;
            // Center of mass of rigidbody offseted from center
            public Vector3 centerOfMass;

            // Use this for initialization
            void OnEnable()
            {
                rBody = GetComponent<Rigidbody>();
                rBody.centerOfMass = centerOfMass;
            }
            private void Start()
            {
                virtualCamera.priority += 12;
            }
            // Update is called once per frame
            void Update()
            {
                // Simulate forces
                rBody.AddForceAtPosition(-rBody.velocity * windResistance * parachute.areaSurface, parachute.wingTransform.position);
                Debug.DrawRay(parachute.wingTransform.position, -rBody.velocity * windResistance * parachute.areaSurface);
            }

            void OnTriggerEnter(Collider other)
            {
                if (other.tag != "Player" && other.tag != "Plane" && other.tag != "PlayerPart")
                {
                    StartCoroutine(DestroyParachute());
                    player.transform.parent = null;
                    player.transform.rotation = new Quaternion(0, -transform.rotation.y, 0, 0);
                    virtualCamera.priority -= 12;
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    player.GetComponent<Animator>().enabled = true;
                    player.GetComponent<Rigidbody>().isKinematic = false;
                    player.GetComponent<Rigidbody>().useGravity = true;
                    playerController.StopInteracting();
                    playerController.canInteract = false;
                    playerController.interactingObject = null;
                }
            }

            IEnumerator DestroyParachute()
            {
                yield return new WaitForSeconds(0.5f);
                Destroy(this.gameObject);
            }

        }
    }
}