using UnityEngine;

namespace PolyPerfect
{
    namespace War
    {
        [RequireComponent(typeof(WheelCollider))]
        public class AntiStuckSystem : MonoBehaviour
        {
            [Space(15)]
            public bool useWheelAntiStuck = true;
            [Range(2, 360)]
            public int raysNumber = 36;
            [Range(0f, 360f)]
            public float raysMaxAngle = 180f;
            [Range(0f, 1f)]
            public float wheelWidth = .25f;
            [Space(15)]
            private Transform wheelModel;

            private WheelCollider _wheelCollider;
            public Transform rootGameObject;
            private float orgRadius;

            void Awake()
            {
                _wheelCollider = GetComponent<WheelCollider>();
                wheelModel = transform.GetChild(0).transform;
                orgRadius = _wheelCollider.radius;
            }
            void FixedUpdate()
            {
                if (useWheelAntiStuck)
                {
                    if (!wheelModel)
                        return;

                    float radiusOffset = 0f;

                    for (int i = 0; i <= raysNumber; i++)
                    {
                        Vector3 rayDirection = Quaternion.AngleAxis(i * (raysMaxAngle / raysNumber) + ((180f - raysMaxAngle) / 2), transform.right) * transform.forward;

                        if (Physics.Raycast(wheelModel.position, rayDirection, out RaycastHit hit, _wheelCollider.radius))
                        {
                            if (!hit.transform.IsChildOf(transform) && !hit.collider.isTrigger)
                            {
                                // Debug.DrawLine(wheelModel.position, hit.point, Color.red);

                                radiusOffset = Mathf.Max(radiusOffset, _wheelCollider.radius - hit.distance);
                            }
                        }
                        /*
                                       // Debug.DrawRay(wheelModel.position, rayDirection * orgRadius, Color.green);

                                        if (Physics.Raycast(wheelModel.position + wheelModel.right * wheelWidth * .5f, rayDirection, out RaycastHit rightHit, _wheelCollider.radius))
                                        {
                                            if (!rightHit.transform.IsChildOf(carController.transform) && !rightHit.collider.isTrigger)
                                            {
                                               // Debug.DrawLine(wheelModel.position + wheelModel.right * wheelWidth * .5f, rightHit.point, Color.red);

                                                radiusOffset = Mathf.Max(radiusOffset, _wheelCollider.radius - rightHit.distance);
                                            }
                                        }

                                      //  Debug.DrawRay(wheelModel.position + wheelModel.right * wheelWidth * .5f, rayDirection * orgRadius, Color.green);

                                        if (Physics.Raycast(wheelModel.position - wheelModel.right * wheelWidth * .5f, rayDirection, out RaycastHit leftHit, _wheelCollider.radius))
                                        {
                                            if (!leftHit.transform.IsChildOf(carController.transform) && !leftHit.collider.isTrigger)
                                            {
                                                //Debug.DrawLine(wheelModel.position - wheelModel.right * wheelWidth * .5f, leftHit.point, Color.red);

                                                radiusOffset = Mathf.Max(radiusOffset, _wheelCollider.radius - leftHit.distance);
                                            }
                                        }

                                       // Debug.DrawRay(wheelModel.position - wheelModel.right * wheelWidth * .5f, rayDirection * orgRadius, Color.green);*/
                    }
                    Debug.DrawRay(wheelModel.position, -_wheelCollider.transform.up * orgRadius, Color.magenta);
                    if (Physics.Raycast(wheelModel.position, -_wheelCollider.transform.up, orgRadius) && _wheelCollider.radius > orgRadius + 0.02f)
                    {

                        _wheelCollider.radius = orgRadius;
                    }
                    else
                        _wheelCollider.radius = Mathf.LerpUnclamped(_wheelCollider.radius, orgRadius + radiusOffset, Time.deltaTime * 5);
                }
                else
                {
                    _wheelCollider.radius = orgRadius;
                }
            }
        }
    }
}