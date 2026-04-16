using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArcadeVP
{
    public class ArcadeVehicleController : MonoBehaviour
    {
        public enum GroundCheckType { RayCast, SphereCast };
        public enum MovementMode { Velocity, AngularVelocity };

        public MovementMode movementMode;
        public GroundCheckType groundCheck;
        public LayerMask drivableSurface;

        [Header("Base Stats")]
        public float maxSpeed;
        public float acceleration;
        public float turn;

        public float gravity = 7f;
        public float downforce = 5f;

        [Header("Runtime Smooth System")]
        public float statSmoothness = 5f;

        public float targetSpeedMultiplier = 1f;
        public float targetAccelerationMultiplier = 1f;
        public float targetTurnMultiplier = 1f;

        public float speedMultiplier = 1f;
        public float accelerationMultiplier = 1f;
        public float turnMultiplier = 1f;

        [Header("Legacy Compatibility")]
        [HideInInspector] public float skidWidth = 1f;   // ✅ FIX FOR SkidMarks.cs

        public bool airControl = false;
        public bool kartLike = false;
        public float driftMultiplier = 1.5f;

        public Rigidbody rb;
        public Rigidbody carBody;

        [HideInInspector] public RaycastHit hit;

        public AnimationCurve frictionCurve;
        public AnimationCurve turnCurve;
        public PhysicsMaterial frictionMaterial;

        [Header("Visuals")]
        public Transform bodyMesh;
        public Transform[] frontWheels = new Transform[2];
        public Transform[] rearWheels = new Transform[2];

        [HideInInspector] public Vector3 carVelocity;

        [Range(0, 10)] public float bodyTilt;

        [Header("Audio")]
        public AudioSource engineSound;

        [Range(0, 1)] public float minPitch;

        [Range(1, 3)] public float maxPitch;

        public AudioSource skidSound;

        private float radius;
        private float steeringInput;
        private float accelerationInput;
        private float brakeInput;

        private Vector3 origin;
        private bool isGrounded;

        private float EffectiveMaxSpeed => maxSpeed * speedMultiplier;
        private float EffectiveAcceleration => acceleration * accelerationMultiplier;
        private float EffectiveTurn => turn * turnMultiplier;

        private void Start()
        {
            radius = rb.GetComponent<SphereCollider>().radius;

            if (movementMode == MovementMode.AngularVelocity)
                Physics.defaultMaxAngularSpeed = 100;
        }

        private void Update()
        {
            SmoothStats();
            Visuals();
            AudioManager();
        }

        private void SmoothStats()
        {
            float t = Time.deltaTime * statSmoothness;

            speedMultiplier = Mathf.Lerp(speedMultiplier, targetSpeedMultiplier, t);
            accelerationMultiplier = Mathf.Lerp(accelerationMultiplier, targetAccelerationMultiplier, t);
            turnMultiplier = Mathf.Lerp(turnMultiplier, targetTurnMultiplier, t);
        }

        public void ProvideInputs(float _steeringInput, float _accelerationInput, float _brakeInput)
        {
            steeringInput = _steeringInput;
            accelerationInput = _accelerationInput;
            brakeInput = _brakeInput;
        }

        public void AudioManager()
        {
            engineSound.pitch = Mathf.Lerp(
                minPitch,
                maxPitch,
                Mathf.Abs(carVelocity.z) / Mathf.Max(1f, EffectiveMaxSpeed)
            );

            skidSound.mute = !(Mathf.Abs(carVelocity.x) > 10 && grounded());
        }

        private void FixedUpdate()
        {
            UpdateGroundedState();

            carVelocity = carBody.transform.InverseTransformDirection(carBody.linearVelocity);

            if (Mathf.Abs(carVelocity.x) > 0)
            {
                frictionMaterial.dynamicFriction =
                    frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100));
            }

            if (grounded())
            {
                float sign = Mathf.Sign(carVelocity.z);

                float TurnMultiplier =
                    turnCurve.Evaluate(carVelocity.magnitude / Mathf.Max(1f, EffectiveMaxSpeed));

                if (kartLike && brakeInput > 0.1f)
                    TurnMultiplier *= driftMultiplier;

                if (accelerationInput > 0.1f || carVelocity.z > 1)
                {
                    carBody.AddTorque(Vector3.up * steeringInput * sign * EffectiveTurn * 100 * TurnMultiplier);
                }
                else if (accelerationInput < -0.1f || carVelocity.z < -1)
                {
                    carBody.AddTorque(Vector3.up * steeringInput * sign * EffectiveTurn * 100 * TurnMultiplier);
                }

                if (movementMode == MovementMode.Velocity)
                {
                    rb.linearVelocity = Vector3.Lerp(
                        rb.linearVelocity,
                        carBody.transform.forward * accelerationInput * EffectiveMaxSpeed,
                        EffectiveAcceleration / 10 * Time.deltaTime
                    );
                }
                else
                {
                    rb.angularVelocity = Vector3.Lerp(
                        rb.angularVelocity,
                        carBody.transform.right * accelerationInput * EffectiveMaxSpeed / radius,
                        EffectiveAcceleration * Time.deltaTime
                    );
                }

                rb.AddForce(-transform.up * downforce * rb.mass);

                carBody.MoveRotation(
                    Quaternion.Slerp(
                        carBody.rotation,
                        Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation,
                        0.12f
                    )
                );
            }
        }

        public void Visuals()
        {
            foreach (Transform FW in frontWheels)
            {
                FW.localRotation = Quaternion.Slerp(
                    FW.localRotation,
                    Quaternion.Euler(FW.localRotation.eulerAngles.x, 30 * steeringInput, FW.localRotation.eulerAngles.z),
                    0.7f * Time.deltaTime / Time.fixedDeltaTime
                );
            }
        }

        private void UpdateGroundedState()
        {
            origin = rb.position + radius * Vector3.up;
            float maxdistance = radius + 0.2f;

            isGrounded =
                groundCheck == GroundCheckType.RayCast
                    ? Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface)
                    : Physics.SphereCast(origin, radius + 0.1f, -transform.up, out hit, maxdistance, drivableSurface);
        }

        public bool grounded() => isGrounded;
    }
}