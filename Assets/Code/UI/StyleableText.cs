using TMPro;
using UnityEngine;
using System.Linq;
using Goldenwere.Unity.UI;

namespace SFBuilder.UI
{
    /// <summary>
    /// Represents some form of styleable text
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class StyleableText : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private FontFormat format;
        [SerializeField] private TextType   type;
#pragma warning restore 0649
        /**************/ private TMP_Text   text;

        /// <summary>
        /// Initialize text on Start
        /// </summary>
        private void Start()
        {
            text = GetComponent<TMP_Text>();
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
        /// Handle GameEventSystem.SettingsUpdated by applying font settings
        /// </summary>
        private void OnSettingsUpdated()
        {
            text.font = UIAssets.Instance
                .fonts.First(f => f.style == GameSettings.Instance.Settings.accessibility_FontStyle).font;
            text.fontSharedMaterial = UIAssets.Instance
                .fonts.First(f => f.style == GameSettings.Instance.Settings.accessibility_FontStyle)
                .presets.First(m => m.type == type).material;
            text.fontSize = GameConstants.FontSizeToFloat(GameSettings.Instance.Settings.accessibility_FontSize, format);

            if (format == FontFormat.Tooltip)
                GetComponentInParent<TooltipPrefab>().ApplyTextSettings();
        }
    }
}