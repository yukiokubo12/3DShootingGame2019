using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif

namespace PolyPerfect
{
    namespace War
    {
        [ExecuteInEditMode]
        public class VirtualCamera : MonoBehaviour
        {
            // Priority of virtual camera
            public int priority;
            // priority from last frame 
            private int lastPriority;
            [Space]
            // Target position on wich will camera focus
            public Transform lookAtTarget;
            // Target with wich will camera travel
            public Transform followTarget;
            // Transform of camera
            public Transform cameraTransform;
            // Offset from camera focus
            public Vector2 offset;
            // Distance betwen camera and followTarget
            [Space]
            [Range(0, 50)]
            public float distance;

            private CameraBrain cameraBrain;
            // Speed of camera roration around followTarget
            [Range(0, 5)]
            public float rotateSpeed;
            // Field of view of camera
            [Range(0, 100)]
            public float fOV;
            // Maximum andle betwen camera and lookAtTarget
            [Range(0, 89)]
            public float maxAngle;
            // Minimum andle betwen camera and lookAtTarget
            [Range(0, -89)]
            public float minAngle;
            // Determines if the camera is active
            [HideInInspector]
            public bool isActive;
            // Position of followTarget from last frame
            [HideInInspector]
            public Vector3 lastPos;
            // Default rotation of camera around y axis
            [Range(0, 359)]
            public float rotation;
            // Radius of camera collider
            [Range(0f, 2f)]
            public float cameraColliderRadius;
            // If true camera will use FixedUpdate for moving 
            [Space]
            public bool useFixedUpdate;
            // If true camera will inherit position and rotation from last active camera
            public bool InheritPosition;

            // If true player can change distance from followTarget
            [Space]
            [HideInInspector]
            public bool allowZoom;
            // Speed of zooming
            [Range(0, 50)]
            [HideInInspector]
            public float zoomSpeed;
            // If true camera will center its position after certain seconds of no player input
            [HideInInspector]
            [Space]
            public bool centering;
            // Time is seconds in wich will camera center
            [HideInInspector]
            public float timeUntilCenter = 2f;
            // Speed of camera centering
            [HideInInspector]
            public float centeringSpeed = 2f;

            // List of virtual cameras
            public static List<VirtualCamera> virtualCameras = new List<VirtualCamera>();

            private Vector3 lookTarget;
            private bool triggered;
            private float timeSinceLastInput;

            private void Awake()
            {
                // Adds camera to the camera list
                if (!virtualCameras.Contains(this))
                    virtualCameras.Add(this);
            }
            // Start is called before the first frame update
            void Start()
            {
#if UNITY_EDITOR
                if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                {
#endif
                    lastPriority = priority;
                    cameraBrain = CameraBrain.Instance;
                    // cameraBrain.OnPriorityChange();
                    lastPos = followTarget.position;
                    transform.position = followTarget.position + followTarget.forward * -distance + transform.up.normalized * offset.y + transform.right.normalized * offset.x;
                    cameraTransform.position = transform.position;
                    cameraTransform.rotation = transform.rotation;
#if UNITY_EDITOR
                }
#endif
            }

            // Update is called once per frame
            void FixedUpdate()
            {
                if (useFixedUpdate)
                    UpdateCamera();
            }
            void Update()
            {
                if (!useFixedUpdate)
                    UpdateCamera();
            }
            // Update camera position and rotation
            private void UpdateCamera()
            {
#if UNITY_EDITOR
                if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                {
#endif
                    // Checks if priority changed from the last frame
                    if (lastPriority != priority)
                        cameraBrain.OnPriorityChange();
                    lastPriority = priority;
                    if (!Application.isPlaying)
                    {
                        // Change camera transform in edit mode
                        if (followTarget != null)
                        {
                            followTarget.localRotation = Quaternion.Euler(0, rotation, 0);
                            transform.position = followTarget.position + followTarget.forward * -distance;
                            lookTarget = lookAtTarget.position + (transform.up.normalized * offset.y + transform.right.normalized * offset.x) * (Vector3.Distance(followTarget.position, transform.position) / distance);
                            transform.rotation = Quaternion.LookRotation(lookTarget - transform.position);
                            cameraTransform.rotation = transform.rotation;
                        }
                    }
                    else
                    {
                        if (isActive)
                        {
                            // Modify camera position accordingly to followTarget position
                            transform.position += (followTarget.position - lastPos);
                            // If centering is on updates time since last user input
                            if (centering)
                            {
                                if (Input.GetAxis("Mouse Y") == 0 && Input.GetAxis("Mouse Y") == 0)
                                    timeSinceLastInput += Time.deltaTime;
                                else
                                    timeSinceLastInput = 0;
                            }
                            // Zooming
                            if (Input.mouseScrollDelta.y != 0 && (distance - Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed) > offset.magnitude * 2)
                            {
                                // Camera.main.fieldOfView -= Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed;
                                distance -= Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed;
                            }
                            if (timeSinceLastInput > timeUntilCenter && centering)
                            {
                                // Camera centering
                                transform.position = Vector3.LerpUnclamped(transform.position, followTarget.position + followTarget.forward * -distance + transform.up.normalized * offset.y + transform.right.normalized * offset.x, Time.deltaTime * centeringSpeed);
                                cameraTransform.position = transform.position;
                                lookTarget = lookAtTarget.position + (transform.up.normalized * offset.y + transform.right.normalized * offset.x) * (Vector3.Distance(followTarget.position, transform.position) / distance);
                                transform.rotation = Quaternion.LookRotation(lookTarget - transform.position);
                                cameraTransform.rotation = transform.rotation;
                            }
                            else
                            {
                                // Detection of camera collison whith surroundings
                                RaycastHit[] raycastHits = new RaycastHit[3];
                                int hitNumber = Physics.SphereCastNonAlloc(lookAtTarget.position, cameraColliderRadius, (transform.position - lookAtTarget.position).normalized, raycastHits, distance);
                                triggered = false;
                                if (hitNumber > 0)
                                {
                                    for (int i = 0; i < hitNumber; i++)
                                    {
                                        if (!raycastHits[i].transform.IsChildOf(followTarget.transform.parent) && raycastHits[i].distance > cameraColliderRadius)
                                        {
                                            cameraTransform.position = raycastHits[i].point + (raycastHits[i].normal.normalized * cameraColliderRadius);
                                            triggered = true;
                                            break;
                                        }
                                    }
                                }

                                // If camera collides whith surroundings
                                if (!triggered)
                                {
                                    
                                    transform.position = Vector3.MoveTowards(transform.position, followTarget.position + (transform.position - followTarget.position) * (distance / Vector3.Distance(transform.position, followTarget.position)), 0.4f);
                                    cameraTransform.position = transform.position;
                                }
                                lookTarget = lookAtTarget.position + (cameraTransform.up.normalized * offset.y + cameraTransform.right.normalized * offset.x) * (Vector3.Distance(followTarget.position, cameraTransform.position) / distance);
                                float xRot = Input.GetAxis("Mouse X") * rotateSpeed;
                                transform.RotateAround(followTarget.position, transform.up, xRot);

                                float angle = transform.rotation.eulerAngles.x;
                                if (angle > 180)
                                    angle = angle - 360;
                                float zRot = -Input.GetAxis("Mouse Y") * rotateSpeed;
                                // Ensure that camera styes whit in angle bounderies
                                if (angle + zRot < maxAngle && angle + zRot > minAngle)
                                {
                                    //Rotates around lookAtTarget
                                    transform.RotateAround(followTarget.position, transform.right, zRot);
                                }
                                else
                                {
                                    // Rotates around lookAtTarget
                                    if (angle > 0)
                                        transform.RotateAround(followTarget.position, transform.right, (maxAngle - angle));
                                    else
                                        transform.RotateAround(followTarget.position, transform.right, (minAngle - angle));
                                }
                                if (triggered)
                                    transform.rotation = Quaternion.LookRotation(lookTarget - cameraTransform.position);
                                else
                                {
                                    transform.rotation = Quaternion.LookRotation(lookTarget - transform.position);
                                    cameraTransform.position = transform.position;
                                }
                                cameraTransform.rotation = transform.rotation;
                            }
                            lastPos = followTarget.position;
                        }
                    }
#if UNITY_EDITOR
                }
#endif
            }
        }
    }
}
