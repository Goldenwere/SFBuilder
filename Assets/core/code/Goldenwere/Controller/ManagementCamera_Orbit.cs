/**
*** Copyright (C) 2020 Goldenwere
*** Part of the Goldenwere Standard Unity repository
*** The Goldenwere Standard Unity Repository is licensed under the MIT license
***
*** File Info:
***     Description - Contains the ManagementCamera_Orbit class
***     Pkg Name    - ManagementCamera
***     Pkg Ver     - 1.2.0
***     Pkg Req     - CoreAPI
**/

using UnityEngine;

namespace Goldenwere.Unity.Controller
{
    /// <summary>
    /// Variant of ManagementCamera which does orbit-style rotation using raycasting from the camera to a point in the scene
    /// </summary>
    public class ManagementCamera_Orbit : ManagementCamera
    {
        #region Fields
        [Header("Raycasted Settings")]
#pragma warning disable 0649
        [Range(0.1f,100f)][Tooltip          ("When the raycast doesn't find a collider, an invisible point will be used instead defaultPointDistance units away from the camera")]
        [SerializeField] private float      defaultPointDistance;
        [Tooltip                            ("Whether to raycast downward while rotating; only applies if downcastEnabled is also true; might produce odd behaviour if allowed to tilt")]
        [SerializeField] protected bool     downcastEnabledForRotation;
        [Range(0.1f,1000f)][Tooltip         ("The maximum distance to raycast")]
        [SerializeField] private float      maxDistance;
#pragma warning restore 0649

        /**************/ private Vector3    rotationPoint;
        /**************/ private bool       rotationPointSet;
        #endregion

        #region Methods
        /// <summary>
        /// Sets transform on Update
        /// </summary>
        protected override void Update()
        {
            if (!rotationPointSet)
            {
                if (workingInputActionMovement || workingInputActionRotation || workingInputMouseToggleMovement || workingInputMouseToggleRotation)
                    SetRotationPoint();
            }

            else
            {
                if (!workingInputActionMovement && !workingInputActionRotation && !workingInputMouseToggleMovement && !workingInputMouseToggleRotation)
                    rotationPointSet = false;
            }

            base.Update();
        }

        /// <summary>
        /// Performs camera rotation based on input
        /// </summary>
        /// <remarks>This method performs rotation around a raycasted or default point</remarks>
        /// <param name="input">The current input (modified to account for device sensitivity scaling)</param>
        protected override void PerformRotation(Vector2 input)
        {
            Quaternion horizontal = workingDesiredRotationHorizontal * Quaternion.Euler(0, input.x * settingRotationSensitivity, 0);
            Quaternion vertical = workingDesiredRotationVertical * Quaternion.Euler(-input.y * settingRotationSensitivity, 0, 0);
            Quaternion verticalClamped = vertical.VerticalClampEuler(verticalClamping.x, verticalClamping.y);

            Vector3 eulerAngles;
            if (verticalClamped.eulerAngles.x >= verticalClamping.y - settingRotationSensitivity * Time.deltaTime ||
                verticalClamped.eulerAngles.x <= verticalClamping.x + settingRotationSensitivity * Time.deltaTime)
                eulerAngles = new Vector3(0, input.x, 0);
            else
                eulerAngles = (transformTilt.right * -input.y) + (Vector3.up * input.x);

            Vector3 newPos = workingDesiredPosition.RotateSelfAroundPoint(rotationPoint, eulerAngles);

            if (!WillCollideAtNewPosition(newPos, workingDesiredPosition - newPos))
            {
                workingDesiredRotationHorizontal = horizontal;
                workingDesiredRotationVertical = verticalClamped;

                workingDesiredPosition = newPos;
            }

            if (downcastEnabled)
            {
                if (!downcastEnabledForRotation)
                {
                    workingLostHeight = !Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hitInfo, downcastMaxDistance);
                    if (!workingLostHeight)
                        workingLastHeight = Mathf.Abs(Vector3.Distance(transform.position, hitInfo.point));
                }

                else
                {
                    bool prevLostHeight = workingLostHeight;
                    workingLostHeight = !Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hitInfo, downcastMaxDistance);
                    if (!workingLostHeight)
                    {
                        float dist = Mathf.Abs(Vector3.Distance(transform.position, hitInfo.point));

                        if (prevLostHeight)
                            workingLastHeight = dist;

                        else if (Mathf.Abs(dist - workingLastHeight) >= float.Epsilon)
                        {
                            workingDesiredPosition.y += workingLastHeight - dist;
                            workingLastHeight = dist;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the point at which the camera rotates around
        /// </summary>
        private void SetRotationPoint()
        {
            if (Physics.Raycast(new Ray(transformTilt.position, transformTilt.forward), out RaycastHit hit, maxDistance))
                rotationPoint = hit.point;
            else
                rotationPoint = transform.position + transformTilt.forward * defaultPointDistance;

            rotationPointSet = true;
        }
        #endregion
    }
}