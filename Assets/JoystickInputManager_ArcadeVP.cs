using UnityEngine;

namespace ArcadeVP
{
    public class TouchInputManager_ArcadeVP : MonoBehaviour
    {
        public ArcadeVehicleController arcadeVehicleController;

        [Header("Debug")]
        public float Horizontal;
        public float Vertical;
        public float Jump;

        private void Update()
        {
            // Always accelerate
            Vertical = 1f;

            // No jump
            Jump = 0f;

            // No steering by default
            Horizontal = 0f;

#if UNITY_EDITOR
            // Mouse support for testing in editor
            //if (Input.GetMouseButton(0))    //////////////////
            //{
            //    if (Input.mousePosition.x < Screen.width * 0.5f)
            //        Horizontal = -1f; // A
            //    else
            //        Horizontal = 1f;  // D
            //}
#else
            // Touch support
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.position.x < Screen.width * 0.5f)
                    Horizontal = -1f; // A
                else
                    Horizontal = 1f; // D
            }
#endif

            arcadeVehicleController.ProvideInputs(
                Horizontal,
                Vertical,
                Jump);
        }
    }
}