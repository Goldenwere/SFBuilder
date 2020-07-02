using UnityEngine;
using UnityEngine.InputSystem;

namespace Goldenwere.Unity.Controller
{
    public class GodGameCamera : MonoBehaviour
    {
        #region Fields
        #region Player/UX settings              (these ideally should be exposed to a settings/controls menu, with sensitivity representing a percentage (1=100%))
        /**************/ public  float          settingMovementSensitivity = 1f;
        /**************/ public  float          settingRotationSensitivity = 1f;
        /**************/ public  float          settingZoomSensitivity = 1f;
        /**************/ public  bool           settingModifiersAreToggled;
        #endregion
        #region Inspector-set variables         (all of these are required, and speed/padding should be greater than 0)
#pragma warning disable 0649
        [Tooltip                                ("Anchor used for rotating around when settingRotateAroundAnchor is enabled")]   
        [SerializeField] private Transform      anchorRotationPivot;
        [Tooltip                                ("Input values are read from here")]
        [SerializeField] private PlayerInput    attachedControls;
        [Tooltip                                ("This pivot is used for tilting up and down")]
        [SerializeField] private GameObject     pointCamera;
        [Tooltip                                ("This pivot is used for rotating along the y axis (horizontal rotation)")]
        [SerializeField] private GameObject     pointPivot;
        [Tooltip                                ("The minimum distance the camera must be from objects in the scene")]
        [SerializeField] private float          settingCollisionPadding = 3f;
        [Tooltip                                ("How fast the smooth motion takes place")]
        [SerializeField] private float          settingMotionSpeed = 10f;
        [Tooltip                                ("Whether to rotate around an anchor point rotate around self")]
        [SerializeField] private bool           settingRotateAroundAnchor;
        [Tooltip                                ("How fast the smooth rotation takes place")]
        [SerializeField] private float          settingRotationSpeed = 10f;
        [Tooltip                                ("Min and max angles for camera's vertical rotation")]
        [SerializeField] private Vector2        settingRotationVerticalClamp = new Vector2(-50f, 50f);
#pragma warning restore 0649
        #endregion
        #region Game-style speed-scale settings (these can be tweaked depending on the style of game you are creating)
        /**************/ private const float    sensitivityScaleMovement = 0.35f;
        /**************/ private const float    sensitivityScaleMovementMouse = 0.005f;
        /**************/ private const float    sensitivityScaleRotation = 1f;
        /**************/ private const float    sensitivityScaleRotationMouse = 0.01f;
        /**************/ private const float    sensitivityScaleZoom = 1f;
        /**************/ private const float    sensitivityScaleZoomMouse = 3f;
        #endregion
        #region Internal working variables      (these shouldn't need changed unless modifying how the camera works)
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
                PerformMovement(attachedControls.actions["Movement"].ReadValue<Vector2>().normalized * sensitivityScaleMovement);

            if (workingInputRotation)
                PerformRotation(attachedControls.actions["Rotation"].ReadValue<Vector2>().normalized * sensitivityScaleRotation);

            if (workingInputZoom)
                PerformZoom(attachedControls.actions["Zoom"].ReadValue<float>() * sensitivityScaleZoom);

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
                PerformMovement(context.ReadValue<Vector2>() * sensitivityScaleMovementMouse);
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
                PerformRotation(context.ReadValue<Vector2>() * sensitivityScaleRotationMouse);
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
                PerformZoom(context.ReadValue<float>() * sensitivityScaleZoomMouse);
        }

        public void OnZoomMouseModifier(InputAction.CallbackContext context)
        {
            if (settingModifiersAreToggled)
                workingModifierMouseZoom = !workingModifierMouseZoom;
            else
                workingModifierMouseZoom = context.performed;
        }

        private void PerformMovement(Vector2 input)
        {
            Vector3 add = (pointPivot.transform.forward * input.y * settingMovementSensitivity) + (pointPivot.transform.right * input.x * settingMovementSensitivity);
            if (!WillCollideWithPosition(workingDesiredPosition + add))
                workingDesiredPosition += add;
        }

        private void PerformRotation(Vector2 input)
        {
            workingDesiredRotationHorizontal *= Quaternion.Euler(0, input.x * settingRotationSensitivity, 0);
            workingDesiredRotationVertical *= Quaternion.Euler(-input.y * settingRotationSensitivity, 0, 0);
            workingDesiredRotationVertical = workingDesiredRotationVertical.VerticalClampEuler(settingRotationVerticalClamp.x, settingRotationVerticalClamp.y);
        }

        private void PerformZoom(float input)
        {
            Vector3 add = pointCamera.transform.forward * input * settingZoomSensitivity;
            if (!WillCollideWithPosition(workingDesiredPosition + add))
                workingDesiredPosition += add;
        }

        private bool WillCollideWithPosition(Vector3 pos)
        {
            return Physics.OverlapSphere(pos, settingCollisionPadding).Length > 0;
        }
        #endregion
    }
}