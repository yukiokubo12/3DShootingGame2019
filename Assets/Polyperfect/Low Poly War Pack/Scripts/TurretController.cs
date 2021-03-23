using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PolyPerfect
{
    namespace War
    {
        public class TurretController : InteractableObject
        {
            [Range(0, 360)]
            public float turretRotSpeed = 60f;
            [Range(0, 360)]
            public float gunRotSpeed = 60f;
            [Range(0, 90)]
            public float elevation = 40f;
            [Range(0, -90)]
            public float depression = -5f;

            public List<Weapon> weapons;
            public Transform weaponsCenter;

            public GameObject turretGun;

            public Slider reloadingUI;
            public Image crosshairGun;
            public bool autoFire;
            float time;
            bool realoading = false;
            private Vector3 targetAim;
            private AudioSource audioSource;
            private Animator anim;
            private Transform cam;
            private Vector2 uiOffset;
            private Vector3 gunTarget;

            // Use this for initialization
            void Start()
            {
                cam = Camera.main.transform;
                anim = GetComponent<Animator>();
                audioSource = GetComponent<AudioSource>();
                uiOffset = new Vector2((float)canvas.pixelRect.width / 2f, (float)canvas.pixelRect.height / 2f);
            }

            // Update is called once per frame
            void Update()
            {
                if (interactingWith)
                {

                    if (Input.GetKeyDown(KeyCode.Mouse0) && !realoading)
                    {
                        FireGun();
                        realoading = true;
                        time = 0;
                        StartCoroutine(WaitToRealod());
                    }

                    HideCursor();
                    RaycastHit[] raycastHits = Physics.RaycastAll(cam.position, cam.forward, 1000);
                    if (raycastHits.Length > 0)
                    {
                        foreach (RaycastHit raycast in raycastHits)
                        {
                            if (raycast.collider.attachedRigidbody != null)
                            {
                                if (raycast.collider.attachedRigidbody.gameObject != this.gameObject)
                                {
                                    targetAim = raycast.point;
                                    break;
                                }
                                else
                                    targetAim = cam.position + cam.forward * 1000;
                            }
                            else
                            {
                                targetAim = raycast.point;
                                break;
                            }
                        }
                    }
                    else
                        targetAim = cam.position + cam.forward * 1000;

                    TurnTurret();
                    RotateGun();
                    UpdateGunUI();
                    // TurretGun.transform.rotation = Quaternion.LookRotation(targetAim.position);
                    //  GunHead.transform.rotation = Quaternion.LookRotation(targetAim.position); 
                    if (Input.GetKeyDown(stopInteractingKey) && canGetOut)
                    {
                        StopInteracting();
                    }
                }
                else
                {
                    if (autoFire)
                    {
                        if (!realoading)
                        {
                            FireGun();
                            realoading = true;
                            time = 0;
                            StartCoroutine(WaitToRealod());
                        }
                    }
                }
            }
            void UpdateGunUI()
            {
                Transform gunTransform;
                if (weapons.Count>1)
                    gunTransform = weaponsCenter;
                else
                    gunTransform = weapons[0].weaponEnd;
                if (Physics.Raycast(gunTransform.position, gunTransform.forward, out RaycastHit hit, 1000))
                {
                    gunTarget = hit.point;
                }
                else
                    gunTarget = gunTransform.position + gunTransform.forward * 1000;
                // Get the position on the canvas
                Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(gunTarget);
                Vector2 proportionalPosition = new Vector2(ViewportPosition.x * canvas.pixelRect.width, ViewportPosition.y * canvas.pixelRect.height);

                // Set the position and remove the screen offset
                if (Vector3.Angle(gunTarget - Camera.main.transform.position, Camera.main.transform.forward) < 100)
                    crosshairGun.rectTransform.localPosition = proportionalPosition - uiOffset;
            }

            void TurnTurret()
            {
                float distanceToPlane = Vector3.Dot(transform.up, targetAim - transform.position);
                Vector3 planePoint = targetAim - transform.up * distanceToPlane;
                Quaternion targetQ = Quaternion.LookRotation(planePoint - transform.position, transform.up);/* Quaternion.Euler(0,lookAngle.y- transform.rotation.eulerAngles.y,0);*/
                float rotate = Time.deltaTime * turretRotSpeed;
                //Debug.DrawRay(tankHead.transform.position, targetQ.eulerAngles, Color.red);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQ, rotate);

            }
            void RotateGun()
            {
                float distanceToPlane = Vector3.Dot(transform.up, targetAim - turretGun.transform.position);
                Vector3 planePoint = targetAim - transform.up * distanceToPlane;
                float xRot = gunRotSpeed * Time.deltaTime;
                Quaternion q = Quaternion.LookRotation(new Vector3(0f, distanceToPlane, (planePoint - turretGun.transform.position).magnitude));
                Quaternion quaternion = Quaternion.RotateTowards(turretGun.transform.localRotation, q, xRot);
                float x = quaternion.eulerAngles.x;
                if (x > 180)
                    x = 360 - x;
                else
                    x = -x;

                if (x > elevation)
                {
                    quaternion = Quaternion.Euler(360 - elevation, 0, 0);
                    turretGun.transform.localRotation = Quaternion.RotateTowards(turretGun.transform.localRotation, quaternion, xRot);
                }
                else if (x < depression)
                {
                    quaternion = Quaternion.Euler(-depression, 0, 0);
                    turretGun.transform.localRotation = Quaternion.RotateTowards(turretGun.transform.localRotation, quaternion, xRot);
                }
                else
                    turretGun.transform.localRotation = quaternion;
            }

            void FireGun()
            {
                foreach (Weapon weapon in weapons)
                {
                    weapon.Shoot(Vector3.zero);
                    anim.SetTrigger("Fire");
                }
            }

            IEnumerator WaitToRealod()
            {
                while (time < weapons[0].reloadTime)
                {
                    time += Time.deltaTime;
                    reloadingUI.value = time / weapons[0].reloadTime;
                    yield return null;
                }
                realoading = false;
            }
        }
    }
}

