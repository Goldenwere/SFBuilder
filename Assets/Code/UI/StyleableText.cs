using TMPro;
using UnityEngine;
using System.Linq;

namespace SFBuilder.UI
{
    /// <summary>
    /// Represents some form of styleable text
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class StyleableText : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private TextType   type;
#pragma warning restore 0649
        /**************/ private TMP_Text   text;

        private void Start()
        {
            text = GetComponent<TMP_Text>();
            OnSettingsUpdated();
        }

        private void OnEnable()
        {
            GameEventSystem.SettingsUpdated += OnSettingsUpdated;
        }

        private void OnDisable()
        {
            GameEventSystem.SettingsUpdated -= OnSettingsUpdated;
        }

        private void OnSettingsUpdated()
        {
            text.font = UIAssets.Instance
                .fonts.First(f => f.style == GameSettings.Instance.Settings.accessibility_FontStyle).font;
            text.fontSharedMaterial = UIAssets.Instance
                .fonts.First(f => f.style == GameSettings.Instance.Settings.accessibility_FontStyle)
                .presets.First(m => m.type == type).material;
        }
    }
}