/**
*** Copyright (C) 2020 Goldenwere
*** Part of the Goldenwere Standard Unity repository
*** The Goldenwere Standard Unity Repository is licensed under the MIT license
***
*** File Info:
***     Description - Contains the CameraModeModule class
***     Pkg Name    - ManagementCamera
***     Pkg Ver     - 1.2.0
***     Pkg Req     - CoreAPI
**/

using UnityEngine;
using UnityEngine.InputSystem;

namespace Goldenwere.Unity.Controller
{
    /// <summary>
    /// Optional module which allows for swapping between ManagementCamera modes
    /// </summary>
    public class CameraModeModule : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private ManagementCamera[] cameraModes;
#pragma warning restore 0649
        /**************/ private int                selectedCamera;
        #endregion

        #region Methods
        /// <summary>
        /// Setting selectedCamera in Start rather than inline allows this class to be enabled/disabled in inspector to disable event
        /// </summary>
        private void Start()
        {
            selectedCamera = 0;
        }

        /// <summary>
        /// Handler for SwapCamera from PlayerInput
        /// </summary>
        /// <param name="context">Holds bool regarding performed or cancelled</param>
        public void OnInput_SwapCamera(InputAction.CallbackContext context)
        {
            if (context.performed && cameraModes.Length > 0)
            {
                ManagementCamera curr = cameraModes[selectedCamera];

                selectedCamera++;
                if (selectedCamera >= cameraModes.Length)
                    selectedCamera = 0;

                ManagementCamera next = cameraModes[selectedCamera];

                next.TransferCameraTransforms(curr);
                curr.enabled = false;
                next.enabled = true;
            }
        }
        #endregion
    }
}