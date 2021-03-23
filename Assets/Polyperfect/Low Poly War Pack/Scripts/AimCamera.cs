using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyPerfect
{
    namespace War
    {
        public class AimCamera : MonoBehaviour
        {
            // Virtual camera as aim camera
            VirtualCamera VirtualCameraBase;
            // Key with wich will switch to this camera
            public KeyCode Aim;
            // Ammout of priority that will be added on key press
            public int priorityBoost = 10;
            // True if camera is active
            bool boosted = false;
            public UnityEngine.UI.Image image;
            private void Start()
            {
                VirtualCameraBase = GetComponent<VirtualCamera>();
            }
            private void Update()
            {
                if (VirtualCameraBase != null)
                {
                    if (Input.GetKey(Aim))
                    {
                        if (!boosted)
                        {
                            VirtualCameraBase.priority += priorityBoost;
                            boosted = true;
                        }
                    }
                    else if (boosted)
                    {
                        VirtualCameraBase.priority -= priorityBoost;
                        boosted = false;
                    }
                }
                if (image != null)
                    image.gameObject.SetActive(boosted);
            }
        }
    }
}