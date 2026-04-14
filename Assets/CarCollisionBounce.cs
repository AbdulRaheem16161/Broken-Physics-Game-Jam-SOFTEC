using UnityEngine;

namespace ArcadeVP
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarCollisionBounce : MonoBehaviour
    {
        public Rigidbody carBody;

        public float impactThreshold = 4f;

        public float forceMultiplier = 1.5f;
        public float upwardBoost = 2f;

        public float torqueMultiplier = 1f;

        public float maxForce = 20f;

        private void Reset()
        {
            carBody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            float impact = collision.relativeVelocity.magnitude;

            if (impact < impactThreshold)
                return;

            ContactPoint contact = collision.contacts[0];

            Vector3 hitDir = collision.relativeVelocity.normalized;

            // -----------------------------------------
            // 🔥 KEY FIX: OFF-CENTER FORCE CREATION
            // -----------------------------------------

            Vector3 offsetFromCenter = contact.point - carBody.worldCenterOfMass;

            // remove vertical influence so we control lift ourselves
            offsetFromCenter.y = 0f;

            // base directional force (push direction of impact)
            Vector3 directionalForce = hitDir * impact * forceMultiplier;

            // upward lift
            Vector3 liftForce = Vector3.up * impact * upwardBoost;

            // combine forces
            Vector3 finalForce = directionalForce + liftForce;

            finalForce = Vector3.ClampMagnitude(finalForce, maxForce);

            // APPLY FORCE AT OFFSET POSITION → THIS CREATES REAL TORQUE
            carBody.AddForceAtPosition(finalForce, contact.point + offsetFromCenter, ForceMode.Impulse);

            // EXTRA SPIN FOR FLIPS
            Vector3 torqueAxis = Vector3.Cross(offsetFromCenter.normalized, hitDir);
            carBody.AddTorque(torqueAxis * impact * torqueMultiplier, ForceMode.Impulse);
        }
    }
}