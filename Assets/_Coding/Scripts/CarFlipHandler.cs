using UnityEngine;

namespace ArcadeVP
{
    public class CarFlipHandler : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody carBody;

        [Header("Flip Settings")]
        public float impactThreshold = 5f;
        public float recoveryDelay = 0.5f;
        public float uprightDotThreshold = 0.98f;

        private bool isRecovering;
        private float lastHitTime;

        private void Reset()
        {
            carBody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            #region Enable Free Rotation On Impact

            if (collision.relativeVelocity.magnitude > impactThreshold)
            {
                // Remove X & Z rotation constraints
                carBody.constraints &= ~RigidbodyConstraints.FreezeRotationX;
                carBody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;

                isRecovering = true;
                lastHitTime = Time.time;
            }

            #endregion
        }

        private void FixedUpdate()
        {
            #region Recover Rotation Constraints

            if (!isRecovering) return;

            // wait a bit so physics can do its thing
            if (Time.time - lastHitTime < recoveryDelay) return;

            // check if car is upright using dot product (best method)
            float uprightDot = Vector3.Dot(carBody.transform.up, Vector3.up);

            if (uprightDot > uprightDotThreshold)
            {
                // lock rotation again
                carBody.constraints |= RigidbodyConstraints.FreezeRotationX;
                carBody.constraints |= RigidbodyConstraints.FreezeRotationZ;

                isRecovering = false;
            }

            #endregion
        }
    }
}