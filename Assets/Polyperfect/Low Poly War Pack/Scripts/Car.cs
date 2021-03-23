using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PolyPerfect
{
    namespace War
    {
        [System.Serializable]
        public class Axle
        {
            // Left WheelCollider of axle 
            public WheelCollider leftWheel;
            // Right WheelCollider of axle 
            public WheelCollider rightWheel;

            // If true this axle will be powered by engine
            public bool motor;
            // If true this axle will be capable of steering
            public bool steering;
            // If true this axle will have brakes
            public bool brake;
            // If true this axle will be conected to handbrake
            public bool handBrake;
        }
        [System.Serializable]
        public class Lights
        {
            // List of front lights
            public List<Light> frontLights;
            // Renderer of front lights model
            public Renderer frontLightsRenderer;
            // Material that will be on the lights renderer when are front lights on
            public Material frontLightsOnMaterial;
            // Material that will be on the lights renderer when are front lights off
            public Material frontLightsOffMaterial;
            [Space(10)]
            // List of rear lights
            public List<Light> backLights;
            // Renderer of rear lights model
            public Renderer backLightsRenderer;
            // Material that will be on the lights renderer when are rear lights on
            public Material backLightsOnMaterial;
            // Material that will be on the lights renderer when are rear lights off
            public Material backLightsOffMaterial;
        }
        [RequireComponent(typeof(Rigidbody))]
        public class Car : InteractableObject
        {
            // Tourqe of motor in Nm
            [Range(100f, 1000f)]
            public float motorTourqe = 1;
            // Brake power Nm
            [Range(100f, 1000f)]
            public float brakePower = 1;
            // Maximum speed in km/h that car can reach
            [Range(10f, 300f)]
            public float maxSpeed = 100;
            // Maximum angle of cars turning wheels
            [Range(0f, 90f)]
            public float maxSteeringAngle = 30;
            // List of car axles
            public List<Axle> axles;
            // Front and rear lights
            public Lights lights;
            // Center of mass offseted from center
            public Vector3 centerOfMass;
            // Curve that limits steering angle dependent on speed (where 1 is maxspeed)
            public AnimationCurve steeringCurve;

            // UI text that shows speed of the car im km/h
            public TextMeshProUGUI speedText;
            private Rigidbody rb;
            bool updateSpeed = true;
            bool lightsOn = false;
            void Start()
            {
                // Set rigidbody and turn off lights
                rb = GetComponent<Rigidbody>();
                rb.centerOfMass = centerOfMass;
                foreach (Light light in lights.backLights)
                {
                    light.enabled = false;
                }
                foreach (Light light in lights.frontLights)
                {
                    light.enabled = false;
                }
            }

            void Update()
            {
                // Checks if player is driving the car
                if (interactingWith)
                {
                    // Hides the cursor
                    HideCursor();

                    // Checks if player pressed (stop interacting) key 
                    if (Input.GetKeyDown(stopInteractingKey) && canGetOut)
                    {
                        StopInteracting();
                    }
                    // Handles turning lights on/off
                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        foreach (Light light in lights.frontLights)
                        {
                            light.enabled = !lightsOn;
                        }
                        lightsOn = !lightsOn;
                        if (lightsOn)
                            lights.frontLightsRenderer.material = lights.frontLightsOnMaterial;
                        else
                            lights.frontLightsRenderer.material = lights.frontLightsOffMaterial;
                    }
                }
            }
            // Applies rotation and position of WheelCollider to the visual model witch must be set as first child of WheelCollider
            public void ApplyLocalPositionToVisuals(WheelCollider collider)
            {
                if (collider.transform.childCount == 0)
                {
                    return;
                }

                Transform visualWheel = collider.transform.GetChild(0);

                Vector3 position;
                Quaternion rotation;
                collider.GetWorldPose(out position, out rotation);

                visualWheel.transform.position = position;
                visualWheel.transform.rotation = rotation;
            }
            public void FixedUpdate()
            {
                // Checks if player is driving the car
                if (interactingWith)
                {
                    // Sets variables accordingly to axis input
                    float motor = motorTourqe * Input.GetAxis("Vertical");
                    float brake = brakePower * Input.GetAxis("Vertical");
                    float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
                    float speed = rb.velocity.magnitude * 3.6f;
                    steering *= steeringCurve.Evaluate(speed / maxSpeed);
                    // If true ui text of speed will be updated
                    if (updateSpeed)
                    {
                        speedText.text = Mathf.RoundToInt(speed).ToString();
                        updateSpeed = false;
                        StartCoroutine(UpdateCooldown());
                    }
                    if (lightsOn)
                    {
                        // This takes care of rear lights when breaking
                        if ((brake < 0 && speed > 3 && axles[0].rightWheel.rpm > 0) || (brake > 0 && speed > 3 && axles[0].rightWheel.rpm < 0) || Input.GetKey(KeyCode.Space))
                        {
                            foreach (Light light in lights.backLights)
                            {
                                light.enabled = true;
                            }
                            lights.backLightsRenderer.material = lights.backLightsOnMaterial;
                        }
                        else
                        {
                            foreach (Light light in lights.backLights)
                            {
                                light.enabled = false;
                            }
                            lights.backLightsRenderer.material = lights.backLightsOffMaterial;
                        }

                    }
                    // Updates all wheels
                    foreach (Axle axle in axles)
                    {
                        // Debug.Log("RMP :" + axle.leftWheel.rpm + " Motor :" + motor + " Brake :" + brake);
                        // Add torque to the powered axles if player commad to
                        if (axle.motor)
                        {
                            if (speed <= maxSpeed)
                            {
                                axle.rightWheel.motorTorque = motor;
                                axle.leftWheel.motorTorque = motor;
                            }
                            else
                            {
                                axle.rightWheel.motorTorque = 10;
                                axle.leftWheel.motorTorque = 10;
                            }
                        }
                        // Add power to brakes on axle if player commads to
                        if (axle.brake)
                        {
                            if ((brake < 0 && (axle.rightWheel.rpm + axle.leftWheel.rpm) * 0.5 > 10) || (brake > 0 && (axle.rightWheel.rpm + axle.leftWheel.rpm) * 0.5 < -10))
                            {
                                if (axle.leftWheel.isGrounded || axle.rightWheel.isGrounded)
                                    rb.AddForce(transform.GetChild(0).forward * brake * 10, ForceMode.Force);
                                brake = Mathf.Abs(brake);
                                axle.rightWheel.brakeTorque = brake;
                                axle.leftWheel.brakeTorque = brake;

                            }
                            else
                            {
                                axle.rightWheel.brakeTorque = 0;
                                axle.leftWheel.brakeTorque = 0;
                            }
                        }
                        // Changes angle of steering wheels
                        if (axle.steering)
                        {
                            axle.rightWheel.steerAngle = steering;
                            axle.leftWheel.steerAngle = steering;
                        }

                        if (axle.handBrake)
                            if (Input.GetKey(KeyCode.Space))
                            {
                                axle.leftWheel.brakeTorque = Mathf.Infinity;
                                axle.rightWheel.brakeTorque = Mathf.Infinity;
                            }
                        // Applies changes to bouth wheels on axle
                        ApplyLocalPositionToVisuals(axle.rightWheel);
                        ApplyLocalPositionToVisuals(axle.leftWheel);
                    }
                }
                else
                {
                    // Applies changes to visual wheels if player is not controling the car
                    foreach (Axle axle in axles)
                    {
                        ApplyLocalPositionToVisuals(axle.rightWheel);
                        ApplyLocalPositionToVisuals(axle.leftWheel);
                    }
                }
            }
            // Takes care of player exiting car
            public override void StopInteracting()
            {

                base.StopInteracting();
                // Stars breaking the car when player exits
                axles[1].rightWheel.brakeTorque = brakePower;
                axles[1].leftWheel.brakeTorque = brakePower;
                foreach (Axle axles in axles)
                {
                    axles.leftWheel.motorTorque = 0;
                    axles.rightWheel.motorTorque = 0;
                }
            }
            public override void BlowUp()
            {
                speedText.text = "0";
                base.BlowUp();
            }
            IEnumerator UpdateCooldown()
            {
                // Sets intervals in wich will speed UI text updates
                yield return new WaitForSeconds(0.1f);
                updateSpeed = true;
            }
        }
    }
}
