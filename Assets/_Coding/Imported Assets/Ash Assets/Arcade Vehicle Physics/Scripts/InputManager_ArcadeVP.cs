using UnityEngine;

#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
using UnityEditor;
#endif

namespace ArcadeVP
{
    public class InputManager_ArcadeVP : MonoBehaviour
    {
        public ArcadeVehicleController arcadeVehicleController;

        [Header("Nitro Reference")]
        public Nitro nitroSystem;

        [HideInInspector] public float Horizontal;
        [HideInInspector] public float Vertical;
        [HideInInspector] public float Jump;

        [HideInInspector] public bool nitroBeingPressed;

        #region Reverse Controls

        [Header("Reverse Controls")]
        public bool reverseControls = false;
        public bool reverseRightLeft = false;
        public bool reverseFrontBack = false;

        #endregion

        private void Update()
        {
            #region Read Input

            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
            Jump = Input.GetAxis("Jump");

            #endregion

            #region Nitro State Check

            nitroBeingPressed = false;

            if (nitroSystem != null)
            {
                // Nitro is active ONLY while key is held AND nitro has fuel
                nitroBeingPressed = Input.GetKey(nitroSystem.nitroKey) && nitroSystem.nitro > 0;
            }

            #endregion

            #region Apply Reverse Lock (NEW)

            if (nitroBeingPressed)
            {
                // 🚫 Block reverse completely
                if (Vertical < 0f)
                {
                    Vertical = 0f;
                }
            }

            #endregion

            #region Apply Reverse Logic (existing system)

            if (reverseControls)
            {
                if (reverseRightLeft)
                {
                    Horizontal = -Horizontal;
                }

                if (reverseFrontBack)
                {
                    Vertical = -Vertical;
                }
            }

            #endregion

            #region Send Input To Vehicle

            arcadeVehicleController.ProvideInputs(Horizontal, Vertical, Jump);

            #endregion
        }
    }

#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
    [CustomEditor(typeof(InputManager_ArcadeVP))]
    public class InputManager_ArcadeVP_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            InputManager_ArcadeVP script = (InputManager_ArcadeVP)target;

            if (GUILayout.Button("Upgrade to New Input System"))
            {
                GameObject obj = script.gameObject;
                ArcadeVehicleController avc = script.arcadeVehicleController;

                Undo.RegisterCompleteObjectUndo(obj, "Upgrade Input Manager");

                New_InputManager_ArcadeVP newManager = Undo.AddComponent<New_InputManager_ArcadeVP>(obj);
                newManager.arcadeVehicleController = avc;
                newManager.AddDefaultBindings();

                Undo.DestroyObjectImmediate(script);
            }
        }
    }
#endif
}