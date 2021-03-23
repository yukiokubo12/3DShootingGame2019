using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PolyPerfect
{
    namespace War
    {
        [ExecuteInEditMode]
        public class CameraBrain : MonoBehaviour
        {
            // Curently using camera
            VirtualCamera currentCamera;
            // Time (in seconds) that takes transition from one camera to the other
            public float transitionTime;
            // List of active virtual cameras in the scene
            private List<VirtualCamera> virtualCameras;
            // Camera
            private Camera mainCamera;
            public static CameraBrain Instance;
            private void OnEnable()
            {
                Instance = this;
            }
            void Awake()
            {
                Instance = this;
                mainCamera = Camera.main;
                int cameraPriority = 0;
                virtualCameras = VirtualCamera.virtualCameras;
                // Sets the virtual camera with the highest priority as active 
                foreach (VirtualCamera virtualCamera in virtualCameras)
                {
                    if (virtualCamera.priority > cameraPriority)
                    {
                        cameraPriority = virtualCamera.priority;
                        currentCamera = virtualCamera;
                    }
                    virtualCamera.isActive = false;
                }
                currentCamera.isActive = true;
                if (Application.isPlaying)
                    transform.SetParent(currentCamera.cameraTransform);
                // Reset Position and rotation
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            // This function handles priority changes of virtual cameras
            public void OnPriorityChange()
            {
                int cameraPriority = currentCamera.priority;
                VirtualCamera previousCam = currentCamera;
                bool change = false;
                // Checks if there is a virtual camera with higher priority than the current one
                foreach (VirtualCamera virtualCamera in virtualCameras)
                {
                    if (virtualCamera.priority > cameraPriority)
                    {
                        cameraPriority = virtualCamera.priority;
                        currentCamera = virtualCamera;
                        change = true;
                    }
                }
                // If there is a change of the highest priority, the transition will be performed
                if (change)
                {
                    // Checks if the camera will inherit the position from previous
                    if (currentCamera.InheritPosition)
                    {
                        currentCamera.lastPos = previousCam.followTarget.position;
                        currentCamera.transform.position = previousCam.transform.position;
                        currentCamera.cameraTransform.position = previousCam.cameraTransform.position;
                    }
                    transform.parent = null;
                    previousCam.isActive = false;
                    currentCamera.isActive = true;
                    StopCoroutine("MoveToPosition");
                    StartCoroutine("MoveToPosition", transitionTime);
                }
            }
            // Performs a transition to new virtual camera
            public IEnumerator MoveToPosition(float timeToMove)
            {
                var t = 0f;
                while (t < 1)
                {
                    t += Time.deltaTime / timeToMove;
                    transform.position = Vector3.Lerp(transform.position, currentCamera.cameraTransform.position, t);
                    transform.rotation = Quaternion.Slerp(transform.rotation, currentCamera.cameraTransform.rotation, t);
                    mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentCamera.fOV, t);
                    yield return null;
                }
                transform.SetParent(currentCamera.cameraTransform);
            }
        }
    }
}
