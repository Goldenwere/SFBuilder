using UnityEngine;
using UnityEngine.UI;

namespace SFBuilder.UI
{
    /// <summary>
    /// Functions similarly to StyleableText, in that it applies updates to settings related to UI scaling
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ScaleableUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private ScaleSettings  scaleSettings;
#pragma warning restore 0649
        /**************/ private RectTransform  rect;

        /// <summary>
        /// Subscribe to events OnEnable
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.SettingsUpdated += OnSettingsUpdated;
            OnSettingsUpdated();
        }

        /// <summary>
        /// Unsubscribe from events OnDisable
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.SettingsUpdated -= OnSettingsUpdated;
        }

        /// <summary>
        /// Handle GameEventSystem.SettingsUpdated by applying UI settings
        /// </summary>
        private void OnSettingsUpdated()
        {
            if (rect == null)
                rect = GetComponent<RectTransform>();

            switch (GameSettings.Instance.Settings.accessibility_UIScale)
            {
                case UIScale.Large:  rect.localScale = scaleSettings.large; break;
                case UIScale.Small:  rect.localScale = scaleSettings.small; break;
                case UIScale.Medium:
                default:             rect.localScale = scaleSettings.medium; break;
            }
        }
    }

    /// <summary>
    /// Defines scale settings depending on UI scale setting
    /// </summary>
    [System.Serializable]
    public struct ScaleSettings
    {
        public Vector3  small;
        public Vector3  medium;
        public Vector3  large;
    }
}