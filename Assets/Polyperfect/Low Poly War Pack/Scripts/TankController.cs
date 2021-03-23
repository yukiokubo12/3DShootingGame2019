using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PolyPerfect
{
    namespace War
    {
        [System.Serializable]
        public class TankWheel
        {
            public WheelCollider wheelCollider;
            public Transform beltBone;
        }
        [System.Serializable]
        public class CosmeticWheel
        {
            public Transform wheel;
            public Transform beltBone;
        }


        [RequireComponent(typeof(Rigidbody), typeof(AudioSource), typeof(UnityEngine.AI.NavMeshObstacle))]
        public class TankController : InteractableObject
        {
            [Range(100f, 10000f)]
            public float motorTourqe = 1000;
            [Range(100f, 10000f)]
            public float brakePower = 1000;
            [Range(10f, 80f)]
            public float maxSpeed = 40;
            [Range(5f, 90f)]
            public float turnSpeed = 20;
            [Range(5f, 180f)]
            public float turretRotSpeed = 20f;
            [Range(5f, 180f)]
            public float gunRotSpeed = 20f;
            [Range(0, 90)]
            public float elevation = 40f;
            [Range(0, -90)]
            public float depression = -5f;

            public GameObject tankTurret;
            public GameObject tankGun;
            public SkinnedMeshRenderer rightBeltRenderer;
            public SkinnedMeshRenderer leftBeltRenderer;
            public Vector3 centerOfMass;
            public TankWheel[] rightBeltWheels;
            public TankWheel[] leftBeltWheels;
            public List<CosmeticWheel> cosmeticWheels;

            public Weapon weapon;

            public Image crosshairGun;
            public TextMeshProUGUI speedText;
            public ProgressBar reloadingUI;

            [System.Serializable]
            public class AudioParams
            {
                //Audio Parameters
                public float min = 1f;
                public float max = -1f;
                public float newMin = 0.8f;
                public float newMax = 1.3f;
            }
            public AudioParams audioParams;

            //Input Parameters
            float vertical;
            float horizontal;

            bool startedEngine = false;
            private bool realoading = false;
            bool updateSpeed = true;

            private AudioSource tankMovingSource;
            private Vector3 lastPos;
            private Rigidbody rb;
            private Vector3 targetAim;
            private Vector3 gunTarget;
            private Transform cam;
            private Vector2 uiOffset;
            // Use this for initialization
            void Start()
            {
                lastPos = transform.position;
                rb = GetComponent<Rigidbody>();
                tankMovingSource = GetComponent<AudioSource>();
                rb.centerOfMass = centerOfMass;
                cam = Camera.main.transform;
                uiOffset = new Vector2((float)canvas.pixelRect.width / 2f, (float)canvas.pixelRect.height / 2f);
                reloadingUI.max = weapon.reloadTime;
            }
            private void Update()
            {
                if (interactingWith)
                {
                    UpdateGunUI();
                    HideCursor();
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (!realoading)
                        {
                            FireGun();
                            realoading = true;
                            StartCoroutine(WaitToRealod());
                        }
                    }
                    if (Input.GetKeyDown(stopInteractingKey) && canGetOut)
                    {
                        GetOutOfTank();
                    }
                    Sound();
                }
            }
            void FixedUpdate()
            {
                if (interactingWith)
                {
                    vertical = -Input.GetAxis("Vertical");
                    horizontal = Input.GetAxis("Horizontal");

                    float motor = motorTourqe * Input.GetAxis("Vertical");

                    float brake = brakePower * Input.GetAxis("Vertical");
                    float speed = ((transform.position - lastPos).magnitude / Time.fixedDeltaTime) * 3.6f;
                    float vectorSpeed = speed;
                    if (Vector3.Angle(transform.forward, (transform.position - lastPos)) > 90)
                        vectorSpeed = -vectorSpeed;
                    if (rightBeltWheels[3].wheelCollider.rpm > 0)
                        rightBeltRenderer.material.mainTextureOffset = new Vector2(0, rightBeltRenderer.sharedMaterial.mainTextureOffset.y + (speed / maxSpeed * Time.fixedDeltaTime * 3.6f));
                    else
                        rightBeltRenderer.material.mainTextureOffset = new Vector2(0, rightBeltRenderer.sharedMaterial.mainTextureOffset.y - (speed / maxSpeed * Time.fixedDeltaTime * 3.6f));
                    if (leftBeltWheels[3].wheelCollider.rpm > 0)
                        leftBeltRenderer.material.mainTextureOffset = new Vector2(0, rightBeltRenderer.sharedMaterial.mainTextureOffset.y + (speed / maxSpeed * Time.fixedDeltaTime * 3.6f));
                    else
                        leftBeltRenderer.material.mainTextureOffset = new Vector2(0, rightBeltRenderer.sharedMaterial.mainTextureOffset.y - (speed / maxSpeed * Time.fixedDeltaTime * 3.6f));
                    lastPos = transform.position;

                    float turn = Input.GetAxis("Horizontal");
                    if (motor == 0)
                        transform.Rotate(Vector3.up, turn * turnSpeed * Time.fixedDeltaTime);
                    else
                        transform.Rotate(Vector3.up, turn * turnSpeed * Time.fixedDeltaTime * 0.75f);
                    if (updateSpeed)
                    {
                        speedText.text = Mathf.RoundToInt(speed).ToString();
                        updateSpeed = false;
                        StartCoroutine(UpdateCooldown());
                    }
                    foreach (TankWheel wheel in leftBeltWheels)
                    {
                        if (wheel.wheelCollider.isGrounded)
                        {
                            if (speed < maxSpeed)
                            {
                                if (motor == 0)
                                {
                                    wheel.wheelCollider.motorTorque = turn * motorTourqe;
                                }
                                else if (turn == 0)
                                {
                                    wheel.wheelCollider.motorTorque = motor;
                                }
                                else if (turn < 0)
                                {
                                    wheel.wheelCollider.motorTorque = 0;
                                }
                                else if (turn > 0)
                                {
                                    wheel.wheelCollider.motorTorque = motor;
                                }
                            }
                            else
                                wheel.wheelCollider.motorTorque = 0;
                            if ((brake < 0 && vectorSpeed > 5) || (brake > 0 && vectorSpeed < -5))
                            {
                                rb.AddForce(transform.forward * brake * 10, ForceMode.Force);
                                brake = Mathf.Abs(brake);
                                wheel.wheelCollider.brakeTorque = brake;
                            }
                            else if (Input.GetKey(KeyCode.Space))
                            {
                                wheel.wheelCollider.brakeTorque = brakePower * 10;
                            }
                            else
                            {
                                wheel.wheelCollider.brakeTorque = 0;
                            }

                        }
                        ApplyLocalPositionToVisuals(wheel);
                    }
                    foreach (TankWheel wheel in rightBeltWheels)
                    {
                        if (wheel.wheelCollider.isGrounded)
                        {
                            if (speed < maxSpeed)
                            {
                                if (motor == 0)
                                {
                                    wheel.wheelCollider.motorTorque = -turn * motorTourqe;
                                }
                                else if (turn == 0)
                                {
                                    if (speed < maxSpeed)
                                        wheel.wheelCollider.motorTorque = motor;
                                }
                                else if (turn < 0)
                                {
                                    //  wheel.wheelCollider.sidewaysFriction = wheelFrictionCurve;
                                    if (speed < maxSpeed)
                                        wheel.wheelCollider.motorTorque = motor;
                                }
                                else if (turn > 0)
                                {
                                    // wheel.wheelCollider.sidewaysFriction = wheelFrictionCurve;
                                    wheel.wheelCollider.motorTorque = 0;
                                }
                            }
                            else
                                wheel.wheelCollider.motorTorque = 0;

                            if ((brake < 0 && vectorSpeed > 5) || (brake > 0 && vectorSpeed < -5))
                            {
                                rb.AddForce(transform.forward * brake * 10, ForceMode.Force);
                                brake = Mathf.Abs(brake);
                                wheel.wheelCollider.brakeTorque = brake;
                            }
                            else if (Input.GetKey(KeyCode.Space))
                            {
                                wheel.wheelCollider.brakeTorque = brakePower * 10;
                            }
                            else
                            {
                                wheel.wheelCollider.brakeTorque = 0;
                            }
                        }
                        ApplyLocalPositionToVisuals(wheel);
                    }
                    foreach (CosmeticWheel cosmeticWheel in cosmeticWheels)
                    {
                        cosmeticWheel.wheel.Rotate(leftBeltWheels[0].wheelCollider.rpm * 5.75f * Time.fixedDeltaTime, 0, 0);
                    }
                    if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 100))
                    {
                        targetAim = hit.point;
                    }
                    else
                        targetAim = cam.position + cam.forward * 100;
                    if (!Input.GetMouseButton(1))
                    {
                        TurnTurret();
                        RotateGun();
                    }
                    UpdateGunUI();
                }
            }
            void GetOutOfTank()
            {
                foreach (TankWheel tankWheel in rightBeltWheels)
                {
                    tankWheel.wheelCollider.motorTorque = 0;
                    tankWheel.wheelCollider.brakeTorque = brakePower;
                }
                foreach (TankWheel tankWheel in leftBeltWheels)
                {
                    tankWheel.wheelCollider.motorTorque = 0;
                    tankWheel.wheelCollider.brakeTorque = brakePower;
                }
                tankMovingSource.Stop();
                startedEngine = false;
                StopInteracting();
            }

            void UpdateGunUI()
            {
                if (Physics.Raycast(weapon.weaponEnd.position, weapon.weaponEnd.forward, out RaycastHit hit, 100))
                {
                    gunTarget = hit.point;
                }
                else
                    gunTarget = weapon.weaponEnd.position + weapon.weaponEnd.forward * 100;
                // Get the position on the canvas
                Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(gunTarget);
                Vector2 proportionalPosition = new Vector2(ViewportPosition.x * canvas.pixelRect.width, ViewportPosition.y * canvas.pixelRect.height);
                // Set the position and remove the screen offset
                if (Vector3.Angle(gunTarget - Camera.main.transform.position, Camera.main.transform.forward) < 100)
                    crosshairGun.rectTransform.localPosition = proportionalPosition - uiOffset;
            }
            void FireGun()
            {
                weapon.Shoot(Vector3.zero);
                rb.AddForceAtPosition(-weapon.transform.forward * rb.mass * weapon.weaponDamage * 0.25f, weapon.transform.position);
            }
            public void ApplyLocalPositionToVisuals(TankWheel tankWheel)
            {
                if (tankWheel.wheelCollider.transform.childCount == 0)
                {
                    return;
                }

                Transform visualWheel = tankWheel.wheelCollider.transform.GetChild(0);

                tankWheel.wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
                tankWheel.beltBone.transform.position = position;
                visualWheel.transform.position = position;
                visualWheel.transform.rotation = rotation;
            }


            void Sound()
            {
                if (interactingWith)
                {
                    if (!startedEngine)
                    {
                        tankMovingSource.Play();
                        startedEngine = true;
                    }
                    else
                    {
                        var audioPitch = vertical - Mathf.Abs(horizontal);

                        tankMovingSource.pitch = audioPitch.Remap(audioParams.min, audioParams.max, audioParams.newMin, audioParams.newMax);
                    }
                }
            }

            void TurnTurret()
            {
                float distanceToPlane = Vector3.Dot(tankTurret.transform.up, targetAim - tankTurret.transform.position);
                Vector3 planePoint = targetAim - tankTurret.transform.up * distanceToPlane;
                Quaternion targetQ = Quaternion.LookRotation(planePoint - tankTurret.transform.position, tankTurret.transform.up);/* Quaternion.Euler(0,lookAngle.y- transform.rotation.eulerAngles.y,0);*/
                float rotate = Time.deltaTime * turretRotSpeed;
                tankTurret.transform.rotation = Quaternion.RotateTowards(tankTurret.transform.rotation, targetQ, rotate);

            }
            void RotateGun()
            {
                float distanceToPlane = Vector3.Dot(tankTurret.transform.up, targetAim - tankGun.transform.position);
                Vector3 planePoint = targetAim - tankTurret.transform.up * distanceToPlane;
                float xRot = gunRotSpeed * Time.deltaTime;
                Quaternion q = Quaternion.LookRotation(new Vector3(0f, distanceToPlane, (planePoint - tankGun.transform.position).magnitude));
                Quaternion quaternion = Quaternion.RotateTowards(tankGun.transform.localRotation, q, xRot);
                float x = quaternion.eulerAngles.x;
                if (x > 180)
                    x = 360 - x;
                else
                    x = -x;

                if (x > elevation)
                {
                    quaternion = Quaternion.Euler(360 - elevation, 0, 0);
                    tankGun.transform.localRotation = Quaternion.RotateTowards(tankGun.transform.localRotation, quaternion, xRot);
                }
                else if (x < depression)
                {
                    quaternion = Quaternion.Euler(-depression, 0, 0);
                    tankGun.transform.localRotation = Quaternion.RotateTowards(tankGun.transform.localRotation, quaternion, xRot);
                }
                else
                    tankGun.transform.localRotation = quaternion;
            }
            IEnumerator WaitToRealod()
            {
                float time = 0;
                while (time < weapon.reloadTime)
                {
                    time += Time.deltaTime;
                    reloadingUI.current = time;
                    yield return null;
                }
                realoading = false;
            }
            IEnumerator UpdateCooldown()
            {
                yield return new WaitForSeconds(0.1f);
                updateSpeed = true;
            }
        }
    }
}
