using UnityEngine;
using TMPro;

namespace SFBuilder.UI
{
    public class UIAssets : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private FontCollection[]   fonts;
#pragma warning restore 0649

        public FontCollection[] Fonts       { get { return fonts; } }
        public static UIAssets  Instance    { get; private set; }

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
    public struct MaterialPresetsCollection
    {
        public TextType type;
        public Material material;
    }

    /// <summary>
    /// Structure for associating FontStyles with presets and font assets
    /// </summary>
    [System.Serializable]
    public struct FontCollection
    {
        public FontStyle                    style;
        public TMP_FontAsset                font;
        public MaterialPresetsCollection[]  presets;
    }
}