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
        /**************/    private bool        workingDoHorizontal;
        /**************/    private bool        workingDoRotation;
        /**************/    private bool        workingDoVertical;
        /**************/    private bool        workingIsLocked;
        #endregion

        #region Methods
        /// <summary>
        /// Manipulate camera on MonoBehaviour.Update() as long as the camera is not locked
        /// </summary>
        private void Update()
        {
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
        }

        /// <summary>
        /// Handler for the Horizontal input event (defined in FreeFlyActions)
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnHorizontal(InputAction.CallbackContext context)
        {
            workingDoHorizontal = context.performed;
        }

        /// <summary>
        /// Handler for the Lock input event (defined in FreeFlyActions)
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnLock(InputAction.CallbackContext context)
        {
            if (context.performed)
                workingIsLocked = !workingIsLocked;
        }

        /// <summary>
        /// Handler for the Rotation input event (defined in FreeFlyActions)
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnRotation(InputAction.CallbackContext context)
        {
            workingDoRotation = context.performed;
        }

        /// <summary>
        /// Handler for the Vertical input event (defined in FreeFlyActions)
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnVertical(InputAction.CallbackContext context)
        {
            workingDoVertical = context.performed;
        }
        #endregion
    }
}