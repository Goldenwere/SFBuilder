using UnityEngine;
using TMPro;

namespace SFBuilder.UI
{
    public class UIAssets : MonoBehaviour
    {
#pragma warning disable 0649
        public readonly FontCollection[] fonts;
#pragma warning restore 0649

        public static UIAssets Instance { get; private set; }

        /// <summary>
        /// Set singleton instance on Awake
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }
    }

    /// <summary>
    /// Structure for associating TextTypes with materials
    /// </summary>
    [System.Serializable]
    public readonly struct MaterialPresetsCollection
    {
        public readonly TextType type;
        public readonly Material material;
    }

    /// <summary>
    /// Structure for associating FontStyles with presets and font assets
    /// </summary>
    [System.Serializable]
    public readonly struct FontCollection
    {
        public readonly FontStyle style;
        public readonly TMP_FontAsset font;
        public readonly MaterialPresetsCollection[] presets;
    }
}