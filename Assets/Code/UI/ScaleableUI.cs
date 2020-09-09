﻿using UnityEngine;
using UnityEngine.UI;

namespace SFBuilder.UI
{
    /// <summary>
    /// Functions similarly to StyleableText, in that it applies updates to settings related to UI scaling
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    public class ScaleableUI : MonoBehaviour
    {
        /**************/ private Graphic    graphic;

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
            if (graphic == null)
                graphic = GetComponent<Graphic>();

            switch (GameSettings.Instance.Settings.accessibility_FontSize)
            {
                case FontSize.Large:  graphic.rectTransform.localScale = new Vector3(1.25f, 1.25f, 1.25f); break;
                case FontSize.Small:  graphic.rectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f); break;
                case FontSize.Medium:
                default:              graphic.rectTransform.localScale = new Vector3(1.00f, 1.00f, 1.00f); break;
            }
        }
    }
}