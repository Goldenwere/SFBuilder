/**
*** Copyright (C) 2020 Goldenwere
*** Part of the Goldenwere Standard Unity repository
*** The Goldenwere Standard Unity Repository is licensed under the MIT license
***
*** File Info:
***     Description - Contains the GamepadCursorModule class
***     Pkg Name    - ManagementCamera
***     Pkg Ver     - 1.0.0
***     Pkg Req     - CoreAPI
**/

using UnityEngine;
using UnityEngine.InputSystem;

namespace Goldenwere.Unity.Controller
{
    /// <summary>
    /// Recommended (optional and separate in case of a custom implementation or use of UI navigation) module
    /// for cursor movement via gamepad for use with the ManagementCamera
    /// </summary>
    public class GamepadCursorModule : MonoBehaviour
    {
        #region Fields
        /**************/ public float           settingCursorSensitivity = 1.0f;
#pragma warning disable 0649
        [SerializeField] private PlayerInput    attachedInput;
#pragma warning restore 0649
        /**************/ private const float    sensitivityScaleCursor = 1000f;
        /**************/ private bool           workingInputActionMoveCursor;
        #endregion

        #region Methods
        /// <summary>
        /// Perform cursor movement on Update
        /// </summary>
        private void Update()
        {
            if (workingInputActionMoveCursor)
            {
                if (Cursor.lockState != CursorLockMode.None)
                    Cursor.lockState = CursorLockMode.None;

                Vector2 currPos = Mouse.current.position.ReadValue();
                Vector2 currInput = attachedInput.actions["MoveCursor"].ReadValue<Vector2>();
                Vector2 newPos = currInput * Time.deltaTime * sensitivityScaleCursor + currPos;
                Mouse.current.WarpCursorPosition(newPos);
                InputSystem.QueueDeltaStateEvent(Mouse.current["position"], newPos);
            }
        }

        /// <summary>
        /// Handler for MoveCursor from PlayerInput
        /// </summary>
        /// <param name="context">Holds bool regarding performed or cancelled</param>
        public void OnInput_MoveCursor(InputAction.CallbackContext context)
        {
            workingInputActionMoveCursor = context.performed;
        }
        #endregion
    }
}