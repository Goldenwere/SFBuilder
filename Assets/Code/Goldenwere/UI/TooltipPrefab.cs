/**
*** Copyright (C) 2020 Goldenwere
*** Part of the Goldenwere Standard Unity repository
*** The Goldenwere Standard Unity Repository is licensed under the MIT license
***
*** File Info:
***     Description - Contains the TooltipPrefab class
***     Pkg Name    - TooltipSystem
***     Pkg Ver     - 1.0.0
***     Pkg Req     - CoreAPI
**/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goldenwere.Unity.UI
{
    /// <summary>
    /// Contains references to the attached elements for the tooltip prefab used by TooltipEnabledElement
    /// </summary>
    public class TooltipPrefab : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [Header("Elements")]
        [Tooltip                                ("Element that points to the element that the tooltip is associated with")]
        [SerializeField] private Image          arrow;
        [Tooltip                                ("Whether to enable the usage of an arrow with this prefab")]
        [SerializeField] private bool           arrowEnabled;
        [Tooltip                                ("Canvas group for animation opacity")]
        [SerializeField] private CanvasGroup    canvasGroup;
        [Tooltip                                ("The text that gets updated")]
        [SerializeField] private TMP_Text       text;
#pragma warning restore 0649
        #endregion

        #region Properties
        public Image            Arrow           { get { return arrow; } }
        public bool             ArrowEnabled    { get { return arrowEnabled; } }
        public CanvasGroup      CGroup          { get { return canvasGroup; } }
        public RectTransform    RTransform      { get { return GetComponent<RectTransform>(); } }
        public TMP_Text         Text            { get { return text; } }
        #endregion
    }
}