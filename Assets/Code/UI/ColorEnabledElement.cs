using UnityEngine;
using UnityEngine.UI;

namespace SFBuilder.UI
{
    /// <summary>
    /// Used for setting up UI colors in GameUI
    /// </summary>
    public class ColorEnabledElement : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private float          alphaBackground = 1f;
        [SerializeField] private float          alphaForeground = 1f;
        [SerializeField] private Graphic[]      backgroundElements;
        [SerializeField] private Graphic[]      foregroundElements;
        [SerializeField] private ElementType    type;
#pragma warning restore 0649

        /// <summary>
        /// Method for color initialization
        /// </summary>
        /// <param name="palette">The palette to use colors from</param>
        public void SetupColors(ColorPalette palette)
        {
            switch (type)
            {
                case ElementType.accent:
                    foreach(Graphic backgroundElement in backgroundElements)
                        backgroundElement.color = palette.BackgroundAccentColor;
                    foreach(Graphic foregroundElement in foregroundElements)
                        foregroundElement.color = palette.ForegroundAccentColor;
                    break;
                case ElementType.secondary:
                    foreach (Graphic backgroundElement in backgroundElements)
                        backgroundElement.color = palette.BackgroundSecondaryColor;
                    foreach (Graphic foregroundElement in foregroundElements)
                        foregroundElement.color = palette.ForegroundSecondaryColor;
                    break;
                case ElementType.primary:
                default:
                    foreach (Graphic backgroundElement in backgroundElements)
                        backgroundElement.color = palette.BackgroundPrimaryColor;
                    foreach (Graphic foregroundElement in foregroundElements)
                        foregroundElement.color = palette.ForegroundPrimaryColor;
                    break;
            }

            foreach (Graphic backgroundElement in backgroundElements)
                backgroundElement.color = new Color(backgroundElement.color.r, backgroundElement.color.g, backgroundElement.color.b, alphaBackground);
            foreach (Graphic foregroundElement in foregroundElements)
                foregroundElement.color = new Color(foregroundElement.color.r, foregroundElement.color.g, foregroundElement.color.b, alphaForeground);
        }
    }

    /// <summary>
    /// Used for defining what type of element a ColorEnabledElement is
    /// </summary>
    public enum ElementType
    {
        primary,
        secondary,
        accent
    }
}