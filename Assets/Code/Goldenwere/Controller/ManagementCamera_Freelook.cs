/**
*** Copyright (C) 2020 Goldenwere
*** Part of the Goldenwere Standard Unity repository
*** The Goldenwere Standard Unity Repository is licensed under the MIT license
***
*** File Info:
***     Description - Contains the ManagementCamera_Freelook class
***     Pkg Name    - ManagementCamera
***     Pkg Ver     - 1.2.0
***     Pkg Req     - CoreAPI
**/

using UnityEngine;

namespace Goldenwere.Unity.Controller
{
    /// <summary>
    /// Variant of ManagementCamera which does freelook-style rotation by rotating around itself
    /// </summary>
    public class ManagementCamera_Freelook : ManagementCamera
    {
        /// <summary>
        /// Performs camera rotation based on input
        /// </summary>
        /// <remarks>This method performs rotation around itself in a freelook manner</remarks>
        /// <param name="input">The current input (modified to account for device sensitivity scaling)</param>
        protected override void PerformRotation(Vector2 input)
        {
            Quaternion horizontal = workingDesiredRotationHorizontal * Quaternion.Euler(0, input.x * settingRotationSensitivity, 0);
            Quaternion vertical = workingDesiredRotationVertical * Quaternion.Euler(-input.y * settingRotationSensitivity, 0, 0);
            Quaternion verticalClamped = vertical.VerticalClampEuler(verticalClamping.x, verticalClamping.y);

            workingDesiredRotationHorizontal = horizontal;
            workingDesiredRotationVertical = verticalClamped;
        }
    }
}