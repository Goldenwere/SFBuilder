using UnityEngine;

namespace SFBuilder.UI
{
    /// <summary>
    /// A more controlled version of ScaleableUI (useful if a UI element already has StyleableText attached)
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ResizeableUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private ResizeSettings resizeSettings;
#pragma warning restore 0649
        /**************/ private RectTransform  rectToApplyTo;

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
            if (rectToApplyTo == null)
                rectToApplyTo = GetComponent<RectTransform>();

            switch (GameSettings.Instance.Settings.accessibility_UIScale)
            {
                case UIScale.Large:  rectToApplyTo.sizeDelta = resizeSettings.large; break;
                case UIScale.Small:  rectToApplyTo.sizeDelta = resizeSettings.small; break;
                case UIScale.Medium:
                default:             rectToApplyTo.sizeDelta = resizeSettings.medium; break;
            }
        }
    }

    /// <summary>
    /// Defines width/height settings depending on UI scale setting
    /// </summary>
    [System.Serializable]
    public struct ResizeSettings
    {
        public Vector2  small;
        public Vector2  medium;
        public Vector2  large;
    }
}