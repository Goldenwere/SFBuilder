using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Goldenwere.Unity.Controller
{
    public class FreeFlyCamera : MonoBehaviour
    {
        #region Fields
        [SerializeField]    private PlayerInput attachedControls;
        [SerializeField]    private GameObject  pointCamera;
        [SerializeField]    private GameObject  pointPivot;
        [SerializeField]    private float       settingMoveSpeed = 2f;
        /**************/    public  float       settingRotationSensitivity = 3f;
        #endregion

        #region Methods
        /// <summary>
        /// Manipulate camera on MonoBehaviour.Update() as long as the camera is not locked
        /// </summary>
        private void Update()
        {
            /*
            if (!workingIsLocked)
            {
                if (workingDoHorizontal)
                {
                    Vector2 value = attachedControls.actions["Horizontal"].ReadValue<Vector2>().normalized;
                    Vector3 dir = pointCamera.transform.forward * value.y + pointCamera.transform.right * value.x;
                    transform.Translate(dir * Time.deltaTime * settingMoveSpeed);
                }

                if (workingDoRotation)
                {
                    Vector2 value = attachedControls.actions["Rotation"].ReadValue<Vector2>();
                    pointCamera.transform.localRotation *= Quaternion.Euler(-value.y * Time.deltaTime * settingRotationSensitivity, 0, 0);
                    pointPivot.transform.localRotation *= Quaternion.Euler(0, value.x * Time.deltaTime * settingRotationSensitivity, 0);
                }

                if (workingDoVertical)
                {
                    float value = attachedControls.actions["Vertical"].ReadValue<float>();
                    Vector3 dir = pointCamera.transform.up * value;
                    transform.Translate(dir * Time.deltaTime * settingMoveSpeed);
                }
            }
            */
        }

        public void OnMovement(InputAction.CallbackContext context)
        {

        }

        public void OnMovementMouse(InputAction.CallbackContext context)
        {

        }

        public void OnMovementMouseToggle(InputAction.CallbackContext context)
        {

        }

        public void OnObjectRotation(InputAction.CallbackContext context)
        {

        }

        public void OnPlacement(InputAction.CallbackContext context)
        {

        }

        public void OnRotation(InputAction.CallbackContext context)
        {

        }

        public void OnRotationMouse(InputAction.CallbackContext context)
        {

        }

        public void OnRotationMouseToggle(InputAction.CallbackContext context)
        {

        }

        public void OnUndo(InputAction.CallbackContext context)
        {

        }

        public void OnZoom(InputAction.CallbackContext context)
        {

        }

        public void OnZoomMouse(InputAction.CallbackContext context)
        {

        }

        public void OnZoomMouseToggle(InputAction.CallbackContext context)
        {

        }
        #endregion
    }
}