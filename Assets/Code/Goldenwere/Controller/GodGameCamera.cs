using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Goldenwere.Unity.Controller
{
    /// <summary>
    /// A GodGameCamera is a type of free-flying camera with behaviour typically associated with god-games (3D, typically involving resource/character management),
    /// such as a horizontal plane of motion, rotating around a point, and zoom
    /// </summary>
    public class GodGameCamera : MonoBehaviour
    {
        #region Fields
        /**************/ public  bool           cameraMotionIsFrozen;
        /**************/ public  CursorLockMode cursorNormalLockModeState;
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
        [Tooltip                                ("The rotation mode: free-look style, around an anchor (can be moved/updated programatically if needed), or around the point the camera is looking at")]
        [SerializeField] private RotationMode   settingRotationMode;
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
        /**************/ private bool           cameraModifiersAreEnabled;
        /**************/ private Vector3        workingDesiredPosition;
        /**************/ private Quaternion     workingDesiredRotationHorizontal;
        /**************/ private Quaternion     workingDesiredRotationVertical;
        /**************/ private bool           workingInputMovement;
        /**************/ private Vector3        workingInputMousePositionOnRotate;
        /**************/ private bool           workingInputMousePositionSet;
        /**************/ private bool           workingInputRotation;
        /**************/ private bool           workingInputZoom;
        /**************/ private bool           workingModifierMouseMovement;
        /**************/ private bool           workingModifierMouseRotation;
        /**************/ private bool           workingModifierMouseZoom;
        #endregion
        #endregion
        #region Properties
        /// <summary>
        /// Toggles whether mouse modifiers are enabled (useful if other systems need to use mouse buttons without camera movement interfering)
        /// </summary>
        public bool CameraModifiersAreEnabled
        {
            get { return cameraModifiersAreEnabled; }
            set
            {
                cameraModifiersAreEnabled = value;
                if (!cameraModifiersAreEnabled)
                {
                    workingModifierMouseZoom = false;
                    workingModifierMouseRotation = false;
                    workingModifierMouseMovement = false;
                }
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// Sets up the working variables on MonoBehaviour.Start()
        /// </summary>
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
            if (!cameraMotionIsFrozen)
            {
                if (workingInputMovement || workingInputRotation || workingInputZoom)
                    SetRotationPoint();

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
        }

        /// <summary>
        /// On the Movement input event, notify camera movement is needed or no longer being done
        /// </summary>
        /// <param name="context">Input context associated with event</param>
        public void OnMovement(InputAction.CallbackContext context)
        {
            if (!cameraMotionIsFrozen)
                workingInputMovement = context.performed;
        }

        /// <summary>
        /// On the MovementMouse input event, notify camera movement via mouse is needed or no longer being done
        /// </summary>
        /// <param name="context">Input context associated with event</param>
        public void OnMovementMouse(InputAction.CallbackContext context)
        {
            if (!cameraMotionIsFrozen && workingModifierMouseMovement)
            {
                PerformMovement(context.ReadValue<Vector2>() * sensitivityScaleMovementMouse);
                if (workingModifierMouseMovement)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        /// <summary>
        /// On the MovementMouseModifier input event, notify camera movement via mouse is enabled/disabled
        /// </summary>
        /// <param name="context">Input context associated with event</param>
        public void OnMovementMouseModifier(InputAction.CallbackContext context)
        {
            if (!cameraMotionIsFrozen && cameraModifiersAreEnabled)
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null ||
                    EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject() ||
                    EventSystem.current == null)
                {
                    if (settingModifiersAreToggled)
                        workingModifierMouseMovement = !workingModifierMouseMovement;
                    else
                        workingModifierMouseMovement = context.performed;

                    if (!workingModifierMouseMovement)
                    {
                        Cursor.lockState = cursorNormalLockModeState;
                        Cursor.visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// On the Rotation input event, notify camera rotation is needed or no longer being done
        /// </summary>
        /// <param name="context">Input context associated with event</param>
        public void OnRotation(InputAction.CallbackContext context)
        {
            if (!cameraMotionIsFrozen)
                workingInputRotation = context.performed;
        }

        /// <summary>
        /// On the RotationMouse input event, notify camera movement via mouse is needed or no longer being done
        /// </summary>
        /// <param name="context">Input context associated with event</param>
        public void OnRotationMouse(InputAction.CallbackContext context)
        {
            if (!cameraMotionIsFrozen && workingModifierMouseRotation)
            {
                PerformRotation(context.ReadValue<Vector2>() * sensitivityScaleRotationMouse);
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        /// <summary>
        /// On the RotationMouseModifier input event, notify camera rotation via mouse is enabled/disabled
        /// </summary>
        /// <param name="context">Input context associated with event</param>
        public void OnRotationMouseModifier(InputAction.CallbackContext context)
        {
            if (!cameraMotionIsFrozen && cameraModifiersAreEnabled)
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null ||
                    EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject() ||
                    EventSystem.current == null)
                {
                    if (settingModifiersAreToggled)
                        workingModifierMouseRotation = !workingModifierMouseRotation;
                    else
                        workingModifierMouseRotation = context.performed;

                    if (!workingModifierMouseRotation)
                    {
                        Cursor.lockState = cursorNormalLockModeState;
                        Cursor.visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// On the Zoom input event, notify camera zoom is needed or no longer being done
        /// </summary>
        /// <param name="context">Input context associated with event</param>
        public void OnZoom(InputAction.CallbackContext context)
        {
            if (!cameraMotionIsFrozen)
                workingInputZoom = context.performed;
        }

        /// <summary>
        /// On the ZoomMouse input event, notify camera zoom via mouse is needed or no longer being done
        /// </summary>
        /// <param name="context">Input context associated with event</param>
        public void OnZoomMouse(InputAction.CallbackContext context)
        {
            if (!cameraMotionIsFrozen && workingModifierMouseZoom)
            {
                PerformZoom(context.ReadValue<float>() * sensitivityScaleZoomMouse);
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        /// <summary>
        /// On the ZoomMouseModifier input event, notify camera zoom via mouse is enabled/disabled
        /// </summary>
        /// <param name="context">Input context associated with event</param>
        public void OnZoomMouseModifier(InputAction.CallbackContext context)
        {
            if (!cameraMotionIsFrozen && cameraModifiersAreEnabled)
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null ||
                    EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject() ||
                    EventSystem.current == null)
                {
                    if (settingModifiersAreToggled)
                        workingModifierMouseZoom = !workingModifierMouseZoom;
                    else
                        workingModifierMouseZoom = context.performed;

                    if (!workingModifierMouseZoom)
                    {
                        Cursor.lockState = cursorNormalLockModeState;
                        Cursor.visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the rotation point for rotation modes that require it
        /// </summary>
        private void SetRotationPoint()
        {
            if (settingRotationMode == RotationMode.CursorRaycast)
            {
                if (!workingInputMousePositionSet)
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 1000f))
                        workingInputMousePositionOnRotate = hit.point;
                workingInputMousePositionSet = workingModifierMouseRotation;
            }

            else if (settingRotationMode == RotationMode.Raycast)
            {
                if (!workingInputMousePositionSet)
                    if (Physics.Raycast(new Ray(pointCamera.transform.position, pointCamera.transform.forward), out RaycastHit hit, 1000f))
                        workingInputMousePositionOnRotate = hit.point;
                workingInputMousePositionSet = workingModifierMouseRotation;
            }
        }

        /// <summary>
        /// Performs camera movement based on input
        /// </summary>
        /// <param name="input">The current magnitude/direction of movement</param>
        private void PerformMovement(Vector2 input)
        {
            Vector3 add = (pointPivot.transform.forward * input.y * settingMovementSensitivity) + (pointPivot.transform.right * input.x * settingMovementSensitivity);
            if (!WillCollideWithPosition(workingDesiredPosition + add, add))
                workingDesiredPosition += add;
        }

        /// <summary>
        /// Performs camera rotation based on input
        /// </summary>
        /// <param name="input">The current magnitude/direction of rotation</param>
        private void PerformRotation(Vector2 input)
        {
            workingDesiredRotationHorizontal *= Quaternion.Euler(0, input.x * settingRotationSensitivity, 0);
            workingDesiredRotationVertical *= Quaternion.Euler(-input.y * settingRotationSensitivity, 0, 0);
            workingDesiredRotationVertical = workingDesiredRotationVertical.VerticalClampEuler(settingRotationVerticalClamp.x, settingRotationVerticalClamp.y);

            if (settingRotationMode != RotationMode.Freeform)
            {
                Vector3 point;
                if (settingRotationMode == RotationMode.Anchor)
                    point = anchorRotationPivot.position;
                else if (settingRotationMode == RotationMode.Raycast)
                {
                    if (workingInputMousePositionSet)
                        point = workingInputMousePositionOnRotate;
                    else
                        point = pointCamera.transform.position + (pointCamera.transform.forward * pointCamera.transform.position.y);
                }
                else
                {
                    if (workingInputMousePositionSet)
                        point = workingInputMousePositionOnRotate;
                    else
                        point = pointCamera.transform.position + (pointCamera.transform.forward * pointCamera.transform.position.y);
                }

                workingDesiredPosition = workingDesiredPosition.RotateSelfAroundPoint(point, new Vector3(0, input.x, 0));
            }
        }

        /// <summary>
        /// Performs camera zoom based on input
        /// </summary>
        /// <param name="input">The current magnitude/direction of zoom</param>
        private void PerformZoom(float input)
        {
            Vector3 add = pointCamera.transform.forward * input * settingZoomSensitivity;
            if (!WillCollideWithPosition(workingDesiredPosition + add, add))
                workingDesiredPosition += add;
        }

        /// <summary>
        /// Determines whether or not the camera is about to collide with something at a future position
        /// </summary>
        /// <param name="pos">The new position</param>
        /// <param name="dir">The direction of motion from old position to new position</param>
        /// <returns>Whether or not the camera will collide with another object</returns>
        private bool WillCollideWithPosition(Vector3 pos, Vector3 dir)
        {
            Collider[] cols = Physics.OverlapSphere(pos, settingCollisionPadding);

            bool willCollide = false;
            foreach (Collider c in cols)
            {
                if (!willCollide && c.gameObject.layer != 2)
                {
                    Vector3 headingCurr = c.transform.position - pos - dir;
                    float dotCurr = Vector3.Dot(headingCurr, pos - dir);
                    Vector3 headingNew = c.transform.position - pos;
                    float dotNew = Vector3.Dot(headingNew, pos);
                    willCollide = Mathf.Abs(dotNew) - Mathf.Abs(dotCurr) > 0;
                }
            }

            return willCollide;
        }
        #endregion
    }

    /// <summary>
    /// Determines the rotational behaviour of a GodGameCamera
    /// </summary>
    public enum RotationMode
    {
        /// <summary>
        /// Rotate around self in a freelook-style manner
        /// </summary>
        Freeform,
        /// <summary>
        /// Rotate around a defined anchor point (can be handled/moved by some other class)
        /// </summary>
        Anchor,
        /// <summary>
        /// Rotate around the point at which the camera is looking at
        /// </summary>
        Raycast,
        /// <summary>
        /// rotate around the point at which the mouse is raycasted to
        /// </summary>
        CursorRaycast
    }
}