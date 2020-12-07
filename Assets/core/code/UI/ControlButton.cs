using UnityEngine;
using UnityEngine.UI;

namespace SFBuilder.UI
{
    /// <summary>
    /// Extends the UnityEngine.UI.Button to allow for associating controls-related information to buttons
    /// </summary>
    public class ControlButton : Button
    {
        [Header("Controls")]
#pragma warning disable 0649
        [Tooltip                                ("Associate a control with this button")]
        [SerializeField] private GameControl    associatedControl;
        [Tooltip                                ("Define what input is expected.\n" +
                                                "Note that there can be multiple expected types for singular controls." +
                                                "There can only be a single type for actions with multiple bindings." +
                                                "This cannot be left empty")]
        [SerializeField] private InputType[]    expectedInput;
#pragma warning restore 0649

        /// <summary>
        /// The control associated with the button
        /// </summary>
        public GameControl  AssociatedControl   { get { return associatedControl; } }

        /// <summary>
        /// The expected inputs for the button
        /// </summary>
        public InputType[]  ExpectedInput       { get { return expectedInput; } }
    }
}