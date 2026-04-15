using UnityEngine;

namespace ArcadeVP
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarCollisionBounce : MonoBehaviour
    {
        public Rigidbody carBody;

        [Header("Only bounce on these layers")]
        public LayerMask bounceLayers;

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
            // ❌ ignore objects not in allowed layers
            if ((bounceLayers.value & (1 << collision.gameObject.layer)) == 0)
                return;

            float impact = collision.relativeVelocity.magnitude;

            if (impact < impactThreshold)
                return;

            ContactPoint contact = collision.contacts[0];

            Vector3 hitDir = collision.relativeVelocity.normalized;

            Vector3 offsetFromCenter = contact.point - carBody.worldCenterOfMass;

            offsetFromCenter.y = 0f;

            Vector3 directionalForce = hitDir * impact * forceMultiplier;

            Vector3 liftForce = Vector3.up * impact * upwardBoost;

            Vector3 finalForce = directionalForce + liftForce;

            finalForce = Vector3.ClampMagnitude(finalForce, maxForce);

            carBody.AddForceAtPosition(finalForce, contact.point + offsetFromCenter, ForceMode.Impulse);

            Vector3 torqueAxis = Vector3.Cross(offsetFromCenter.normalized, hitDir);
            carBody.AddTorque(torqueAxis * impact * torqueMultiplier, ForceMode.Impulse);
        }
    }
}