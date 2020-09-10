using UnityEngine;
using System.Linq;

namespace SFBuilder.Util
{
    /// <summary>
    /// Handler for graphics settings
    /// </summary>
    public class GraphicsHandler : MonoBehaviour
    {
        private Resolution nativeResolution;

        /// <summary>
        /// On Start, load settings
        /// </summary>
        private void Start()
        {
            nativeResolution = Screen.resolutions[Screen.resolutions.Length - 1];
            OnSettingsUpdated();
        }

        /// <summary>
        /// On Enable, subscribe to events
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.SettingsUpdated += OnSettingsUpdated;
        }

        /// <summary>
        /// On Disable, unsubscribe from events
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.SettingsUpdated -= OnSettingsUpdated;
        }

        /// <summary>
        /// Update graphical settings on SettingsUpdated
        /// </summary>
        private void OnSettingsUpdated()
        {
            FullScreenMode mode;
            int[] resolution;

            switch(GameSettings.Instance.Settings.display_Window)
            {
                case WindowMode.Fullscreen: mode = FullScreenMode.FullScreenWindow; break;
                case WindowMode.Windowed:
                default:                    mode = FullScreenMode.MaximizedWindow; break;
            }

            if (GameSettings.Instance.Settings.display_Resolution != ResolutionSetting._native)
                resolution = GameSettings.Instance.Settings.display_Resolution.ToString().Replace("_", "").Split('x').Cast<int>().ToArray();

            else
                resolution = new int[2] { nativeResolution.width, nativeResolution.height };

            Screen.SetResolution(resolution[0], resolution[1], mode, GameSettings.Instance.Settings.display_Framerate);
            if (GameSettings.Instance.Settings.display_Vsync)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;
        }
    }
}