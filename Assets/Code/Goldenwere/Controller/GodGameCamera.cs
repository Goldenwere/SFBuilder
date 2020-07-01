using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Goldenwere.Unity.Controller
{
    public class GodGameCamera : MonoBehaviour
    {
        #region Fields
        /**************/ public  float          settingMovementSensitivity = 1f;
        /**************/ public  float          settingMovementSensitivityMouse = 10f;
        /**************/ public  float          settingRotationSensitivity = 5f;
        /**************/ public  float          settingRotationSensitivityMouse = 100f;
        /**************/ public  float          settingZoomSensitivity = 1f;
        /**************/ public  float          settingZoomSensitivityMouse = 3f;
        /**************/ public  bool           settingModifiersAreToggled;
#pragma warning disable 0649
        [SerializeField] private PlayerInput    attachedControls;
        [SerializeField] private GameObject     pointCamera;
        [SerializeField] private GameObject     pointPivot;
        [SerializeField] private float          settingCollisionPadding = 3f;
        [SerializeField] private float          settingMotionSpeed = 10f;
        [SerializeField] private float          settingRotationSpeed = 10f;
#pragma warning restore 0649
        /**************/ private Vector3        workingDesiredPosition;
        /**************/ private Quaternion     workingDesiredRotationHorizontal;
        /**************/ private Quaternion     workingDesiredRotationVertical;
        /**************/ private bool           workingInputMovement;
        /**************/ private bool           workingInputRotation;
        /**************/ private bool           workingInputZoom;
        /**************/ private bool           workingModifierMouseMovement;
        /**************/ private bool           workingModifierMouseRotation;
        /**************/ private bool           workingModifierMouseZoom;
        #endregion

        #region Methods
        private void Start()
        {
            workingDesiredPosition = transform.position;
            workingDesiredRotationHorizontal = pointPivot.transform.localRotation;
            workingDesiredRotationVertical = pointCamera.transform.localRotation;
        }

        /// <summary>
        /// Manipulate camera on MonoBehaviour.Update()
        /// </summary>
        private void Update()
        {
            if (workingInputMovement)
            {
                Vector2 val = attachedControls.actions["Movement"].ReadValue<Vector2>().normalized * settingMovementSensitivity;
                if (!WillCollideWithPosition(workingDesiredPosition + pointPivot.transform.forward * val.y + pointPivot.transform.right * val.x))
                    workingDesiredPosition += pointPivot.transform.forward * val.y + pointPivot.transform.right * val.x;
            }

            if (workingInputRotation)
            {
                Vector2 val = attachedControls.actions["Rotation"].ReadValue<Vector2>().normalized * settingRotationSpeed;
                workingDesiredRotationHorizontal *= Quaternion.Euler(0, val.x * settingRotationSensitivity / Screen.width, 0);
                workingDesiredRotationVertical *= Quaternion.Euler(-val.y * settingRotationSensitivity / Screen.height, 0, 0);
            }

            if (workingInputZoom)
            {
                float val = attachedControls.actions["Zoom"].ReadValue<float>() * settingZoomSensitivity;
                if (!WillCollideWithPosition(workingDesiredPosition + pointCamera.transform.forward * val))
                    workingDesiredPosition += pointCamera.transform.forward * val;
            }

            transform.position = Vector3.Lerp(transform.position, workingDesiredPosition, Time.deltaTime * settingMotionSpeed);
            pointPivot.transform.localRotation = Quaternion.Slerp(pointPivot.transform.localRotation, workingDesiredRotationHorizontal, Time.deltaTime * settingRotationSpeed);
            pointCamera.transform.localRotation = Quaternion.Slerp(pointCamera.transform.localRotation, workingDesiredRotationVertical, Time.deltaTime * settingRotationSpeed);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            workingInputMovement = context.performed;
        }

        public void OnMovementMouse(InputAction.CallbackContext context)
        {
            if (workingModifierMouseMovement)
            {
                Vector2 val = context.ReadValue<Vector2>() * settingMovementSensitivityMouse;
                val.x /= Screen.width;
                val.y /= Screen.height;
                if (!WillCollideWithPosition(workingDesiredPosition + pointPivot.transform.forward * val.y + pointPivot.transform.right * val.x))
                    workingDesiredPosition += pointPivot.transform.forward * val.y + pointPivot.transform.right * val.x;
            }
        }

        public void OnMovementMouseModifier(InputAction.CallbackContext context)
        {
            if (settingModifiersAreToggled)
                workingModifierMouseMovement = !workingModifierMouseMovement;
            else
                workingModifierMouseMovement = context.performed;
        }

        public void OnRotation(InputAction.CallbackContext context)
        {
            workingInputRotation = context.performed;
        }

        public void OnRotationMouse(InputAction.CallbackContext context)
        {
            if (workingModifierMouseRotation)
            {
                Vector2 val = context.ReadValue<Vector2>();
                workingDesiredRotationHorizontal *= Quaternion.Euler(0, val.x * settingRotationSensitivityMouse / Screen.width, 0);
                workingDesiredRotationVertical *= Quaternion.Euler(-val.y * settingRotationSensitivityMouse / Screen.height, 0, 0);
            }
        }

        public void OnRotationMouseModifier(InputAction.CallbackContext context)
        {
            if (settingModifiersAreToggled)
                workingModifierMouseRotation = !workingModifierMouseRotation;
            else
                workingModifierMouseRotation = context.performed;
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            workingInputZoom = context.performed;
        }

        public void OnZoomMouse(InputAction.CallbackContext context)
        {
            if (workingModifierMouseZoom)
            {
                float val = context.ReadValue<float>() * settingZoomSensitivityMouse;
                if (!WillCollideWithPosition(workingDesiredPosition + pointCamera.transform.forward * val))
                    workingDesiredPosition += pointCamera.transform.forward * val;
            }
        }

        public void OnZoomMouseModifier(InputAction.CallbackContext context)
        {
            if (settingModifiersAreToggled)
                workingModifierMouseZoom = !workingModifierMouseZoom;
            else
                workingModifierMouseZoom = context.performed;
        }

        private bool WillCollideWithPosition(Vector3 pos)
        {
            return Physics.OverlapSphere(pos, settingCollisionPadding).Length > 0;
        }
        #endregion
    }
}