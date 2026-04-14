using UnityEngine;

namespace ArcadeVP
{
    public class AICarBrain : MonoBehaviour
    {
        [Header("References")]
        public ArcadeVehicleController car;
        public Transform target;

        [Header("AI Tuning")]
        public float steerStrength = 1f;
        public float maxSteerAngle = 45f;

        [Header("Distance Control")]
        public float idealDistance = 8f;
        public float slowDistance = 4f;

        private float currentSteer;
        private float currentAccel;
        private float currentBrake;

        private void Update()
        {
            if (!car || !target) return;

            Vector3 toTarget = target.position - car.transform.position;

            // Convert target direction into local space
            Vector3 localPos = car.transform.InverseTransformDirection(toTarget.normalized);

            // -----------------------------
            // STEERING (smooth + no 180° bug)
            // -----------------------------
            float angle = Vector3.SignedAngle(
                car.transform.forward,
                toTarget,
                Vector3.up
            );

            float steer = Mathf.Clamp(angle / maxSteerAngle, -1f, 1f);

            // -----------------------------
            // DISTANCE CONTROL (smooth speed)
            // -----------------------------
            float distance = Vector3.Distance(car.transform.position, target.position);

            float accel = 1f;
            float brake = 0f;

            if (distance < slowDistance)
            {
                // slow down smoothly
                accel = 0.3f;
                brake = 0.2f;
            }
            else if (distance < idealDistance)
            {
                accel = 0.6f;
                brake = 0f;
            }

            // -----------------------------
            // NEVER FREEZE WHEN TARGET IS BEHIND
            // -----------------------------
            if (localPos.z < 0f)
            {
                // target is behind → just turn harder, DO NOT brake
                accel = 0.8f;
                brake = 0f;
                steer *= 1.2f;
            }

            // -----------------------------
            // SMOOTH INPUTS (prevents stutter/freeze feeling)
            // -----------------------------
            currentSteer = Mathf.Lerp(currentSteer, steer * steerStrength, Time.deltaTime * 5f);
            currentAccel = Mathf.Lerp(currentAccel, accel, Time.deltaTime * 3f);
            currentBrake = Mathf.Lerp(currentBrake, brake, Time.deltaTime * 5f);

            // -----------------------------
            // SEND TO CONTROLLER
            // -----------------------------
            car.ProvideInputs(currentSteer, currentAccel, currentBrake);
        }
    }
}