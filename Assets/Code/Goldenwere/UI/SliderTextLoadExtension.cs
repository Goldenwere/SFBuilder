/**
*** Copyright (C) 2020 Goldenwere
*** Part of the Goldenwere Standard Unity repository
*** The Goldenwere Standard Unity Repository is licensed under the MIT license
***
*** File Info:
***     Description - Contains the SliderTextLoadExtension class
***     Pkg Name    - SliderExtensions
***     Pkg Ver     - 1.0.0
***     Pkg Req     - None
**/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goldenwere.Unity.UI
{
    /// <summary>
    /// Extends sliders by adding text and loading values when data loads
    /// </summary>
    public class SliderTextLoadExtension : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private Slider         associatedSlider;
        [SerializeField] private TMP_Text       associatedTextElement;
#pragma warning restore 0649
        #endregion

        #region Properties
        /// <summary>
        /// Exposes the slider to allow direct manipulation without having to have a reference to both this class and the slider class
        /// </summary>
        public Slider   AssociatedSlider    { get { return associatedSlider; } }

        /// <summary>
        /// The text that displays the slider's current value (assuming UpdateText is called when the slider's value is changed)
        /// </summary>
        public TMP_Text AssociatedText      { get { return associatedTextElement; } }
        #endregion

        #region Methods
        /// <summary>
        /// Updates the text using the dynamic float OnValueChanged
        /// </summary>
        /// <param name="newVal">The new value to set .text to</param>
        public void UpdateText(float newVal)
        {
            associatedTextElement.text = newVal.ToString();
        }

        /// <summary>
        /// Updates the text with a new string value
        /// </summary>
        /// <param name="newVal">The new value to set .text to</param>
        public void UpdateText(string newVal)
        {
            associatedTextElement.text = newVal;
        }
        #endregion
    }
}
