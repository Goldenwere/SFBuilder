using Goldenwere.Unity.Controller;
using UnityEngine;

namespace SFBuilder.Util
{
    public class ControllerSettings : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private ManagementCamera[] cameras;
#pragma warning restore 0649

        /// <summary>
        /// Get settings on Start
        /// </summary>
        private void Start()
        {
            OnSettingsUpdated();
        }

        /// <summary>
        /// Subscribe to events OnEnable
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.SettingsUpdated += OnSettingsUpdated;
        }

        /// <summary>
        /// Unsubscribe from events OnDisable
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.SettingsUpdated -= OnSettingsUpdated;
        }

        /// <summary>
        /// Updates camera settings
        /// </summary>
        private void OnSettingsUpdated()
        {
            foreach(ManagementCamera cam in cameras)
            {
                cam.settingInputInversion.mouse.movementHorizontal = GameSettings.Instance.Settings.controlSetting_InvertHorizontal;
                cam.settingInputInversion.mouse.movementVertical = GameSettings.Instance.Settings.controlSetting_InvertVertical;
                cam.settingInputInversion.mouse.rotationHorizontal = GameSettings.Instance.Settings.controlSetting_InvertHorizontal;
                cam.settingInputInversion.mouse.rotationVertical = GameSettings.Instance.Settings.controlSetting_InvertVertical;
                cam.settingInputInversion.mouse.zoom = GameSettings.Instance.Settings.controlSetting_InvertScroll;
                cam.settingMouseMotionIsToggled = !GameSettings.Instance.Settings.controlSetting_HoldModifiers;
                cam.settingMovementSensitivity = GameSettings.Instance.Settings.controlSetting_SensitivityMovement;
                cam.settingRotationSensitivity = GameSettings.Instance.Settings.controlSetting_SensitivityRotation;
                cam.settingZoomSensitivity = GameSettings.Instance.Settings.controlSetting_SensitivityZoom;
                cam.useCameraSmoothing = GameSettings.Instance.Settings.accessibility_CameraSmoothing;
                
            }

            cameras[0].GetComponentInChildren<Camera>().fieldOfView = GameSettings.Instance.Settings.display_FOV;
        }
    }
}