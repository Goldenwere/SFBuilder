/**
*** Copyright (C) 2020 Goldenwere
*** Part of the Goldenwere Standard Unity repository
*** The Goldenwere Standard Unity Repository is licensed under the MIT license
***
*** File Info:
***     Description - Contains the TooltipEnabledElement class and associated structures AnchorMode, AnchorPosition, MiddlePosition, and TransitionMode
***     Pkg Name    - TooltipSystem
***     Pkg Ver     - 1.0.1
***     Pkg Req     - CoreAPI
**/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;

namespace Goldenwere.Unity.UI
{
    /// <summary>
    /// Describes how the tooltip should be anchored in terms of attachment
    /// </summary>
    public enum AnchorMode
    {
        /// <summary>
        /// Tooltip follows cursor
        /// </summary>
        AttachedToCursor,

        /// <summary>
        /// Tooltip stays fixed and positions based on element
        /// </summary>
        AttachedToElement
    }

    /// <summary>
    /// Defines how the tooltip should be anchored in terms of positioning (first half describes vertical positioning, second half describes horizontal positioning)
    /// </summary>
    public enum AnchorPosition
    {
        TopLeft,
        TopMiddle,
        TopRight,
        CenterLeft,
        CenterMiddle,
        CenterRight,
        BottomLeft,
        BottomMiddle,
        BottomRight
    }

    /// <summary>
    /// Defines where an arrow goes when the AnchorPosition is CenterMiddle
    /// </summary>
    public enum MiddlePosition
    {
        Top,
        Bottom
    }

    /// <summary>
    /// Defines how the tooltip should be transitioned
    /// </summary>
    public enum TransitionMode
    {
        None,
        Fade,
        ShiftUp,
        ShiftDown,
        Scale,
        ScaleHorizontal,
        ScaleVertical,
        RotateHorizontal,
        RotateHorizontalInverted,
        RotateVertical,
        RotateVerticalInverted
    }

    /// <summary>
    /// Adds a tooltip to a UI element
    /// </summary>
    public class TooltipEnabledElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
    {
        #region Fields
#pragma warning disable 0649
        [Header("Anchoring Properties")]
        [Tooltip                                ("Defines how the tooltip is attached")]
        [SerializeField] private AnchorMode     anchorMode;
        [Tooltip                                ("The default anchor position. If the tooltip text overflows with this anchor, will change to another one if needed")]
        [SerializeField] private AnchorPosition anchorPosition;
        [Tooltip                                ("Sets where the tooltip arrow goes when using the CenterMiddle setting in anchorPosition. " +
                                                "Has no effect for other settings or when there is no arrow available")]
        [SerializeField] private MiddlePosition arrowDefaultPositionAtMiddle;

        [Header("Required Utilities")]
        [Tooltip                                ("Needed in order to ensure proper tooltip positioning in AnchorMode.AttachedToCamera; otherwise not necessary")]
        [SerializeField] private Camera         cameraThatRendersCanvas;
        [Tooltip                                ("Optional string to provide if cannot attach camera in inspector (e.g. prefabbed UI elements instantiated at runtime)")]
        [SerializeField] private string         cameraThatRendersCanvasName;
        [Tooltip                                ("Needed in order to ensure proper tooltip positioning as well as attaching tooltip to canvas")]
        [SerializeField] private Canvas         canvasToBeAttachedTo;

        [Header("Tooltip Properties")]
        [Range(00f,10f)] [Tooltip               ("Delay (in seconds) between triggering the tooltip and transitioning it into existence")]
        [SerializeField] private float          tooltipDelay;
        [Range(0.01f,1)] [Tooltip               ("Multiplier that determines how much the tooltip anchors to the left/right when AnchorPosition is one of the " +
                                                "left/right settings (has no effect on Middle settings)")]
        [SerializeField] private float          tooltipHorizontalFactor = 1;
        [Tooltip                                ("Padding between the edges of the tooltip and text element, done in traditional CSS order: Top, Right, Bottom, Left")]
        [SerializeField] private Vector4        tooltipPadding;
        [Tooltip                                ("Prefab which contains a TooltipPrefab class. Only the width of the prefab and text size are a concern;\n" +
                                                "text padding is defined in TooltipEnabledElement, and height is determined dynamically based on text contents.")]
        [SerializeField] private GameObject     tooltipPrefab;
        [Tooltip                                ("The text to display in the tooltip")]
        [SerializeField] private string         tooltipText;
        [Tooltip                                ("Values used if defining a string that needs formatting. Leave blank if no formatting is done inside tooltipText")]
        [SerializeField] private double[]       tooltipValues;

        [Header("Transition Properties")]
        [Range(000,100)] [Tooltip               ("How long (in seconds) tooltip transitions last (only used if transitionMode isn't set to None")]
        [SerializeField] private float          transitionDuration;
        [Tooltip                                ("The curve for animating transitions when transitioning into existence")]
        [SerializeField] private AnimationCurve transitionCurveIn = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Tooltip                                ("The curve for animating transitions when transitioning out of existence")]
        [SerializeField] private AnimationCurve transitionCurveOut = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [Tooltip                                ("How the tooltip is transitioned/animated into/out of existence")]
        [SerializeField] private TransitionMode transitionMode;
#pragma warning restore 0649
        /**************/ private bool           isActive;
        /**************/ private bool           isInitialized;
        /**************/ private bool           isTransitioning;
        /**************/ private TooltipPrefab  tooltipInstance;
        // Parent and position needed to ensure tooltip is set back to the same position when using ShiftUp/ShiftDown transitions
        /**************/ private Transform      tooltipInstanceParent;
        /**************/ private Vector3        tooltipInstancePosition;
        #endregion

        #region Methods

        #region Unity Methods
        /// <summary>
        /// Sets up the tooltip at start
        /// </summary>
        private void Start()
        {
            if (!isInitialized)
                Initialize();
            SetText();

            if (anchorMode == AnchorMode.AttachedToElement)
            {
                tooltipInstance.RTransform.anchoredPosition = PositionTooltipToElement();
                tooltipInstancePosition = tooltipInstance.RTransform.anchoredPosition;
            }
        }

        /// <summary>
        /// Set position of tooltip at Update
        /// </summary>
        private void Update()
        {
            if (isActive)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    SetActive(false);

                else if (anchorMode == AnchorMode.AttachedToCursor && PositionTooltipToCursor(out Vector2 newPos))
                    tooltipInstance.RTransform.anchoredPosition = newPos;
            }
        }

        /// <summary>
        /// Destroys the tooltip when the enabled element itself is destroyed
        /// </summary>
        private void OnDestroy()
        {
            Destroy(tooltipInstance);
        }
        #endregion

        /// <summary>
        /// Initializes the tooltip; this is separate from Start in case SetText is called externally before Start gets a chance to run
        /// </summary>
        private void Initialize()
        {
            if (canvasToBeAttachedTo == null)
                canvasToBeAttachedTo = gameObject.GetComponentInParents<Canvas>();

            if (anchorMode == AnchorMode.AttachedToCursor)
            {
                if (cameraThatRendersCanvas == null)
                    if (cameraThatRendersCanvasName != null && cameraThatRendersCanvasName != "")
                        cameraThatRendersCanvas = GameObject.Find(cameraThatRendersCanvasName).GetComponent<Camera>();
                    else
                        cameraThatRendersCanvas = Camera.main;

                tooltipInstance = Instantiate(tooltipPrefab, canvasToBeAttachedTo.transform).GetComponent<TooltipPrefab>();
                if (transitionMode == TransitionMode.ShiftDown || transitionMode == TransitionMode.ShiftUp)
                    transitionMode = TransitionMode.Fade;
            }

            else
                tooltipInstance = Instantiate(tooltipPrefab, GetComponent<RectTransform>()).GetComponent<TooltipPrefab>();

            tooltipInstanceParent = tooltipInstance.transform.parent;
            isActive = tooltipInstance.gameObject.activeSelf;
            SetActive(false, TransitionMode.None);
            isInitialized = true;

            // Override sizing/anchors in prefab in case they may conflict with tooltipping system
            // Padding is a serialized variable, the anchor for text must be vertical+horizontal stretch for it to work properly
            tooltipInstance.Text.rectTransform.offsetMin = new Vector2(tooltipPadding.w, tooltipPadding.z);
            tooltipInstance.Text.rectTransform.offsetMax = new Vector2(-tooltipPadding.y, -tooltipPadding.x);
            tooltipInstance.Text.rectTransform.anchorMin = new Vector2(0, 0);
            tooltipInstance.Text.rectTransform.anchorMax = new Vector2(1, 1);

            // Ensure arrow and container are set to center+center so that positioning is correct
            tooltipInstance.RTransform.anchorMin = new Vector2(0.5f, 0.5f);
            tooltipInstance.RTransform.anchorMax = new Vector3(0.5f, 0.5f);

            if (tooltipInstance.ArrowEnabled)
            {
                tooltipInstance.Arrow.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                tooltipInstance.Arrow.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            }
        }

        #region Handlers
        /// <summary>
        /// OnDeselect, disable the tooltip
        /// </summary>
        public void OnDeselect(BaseEventData data)
        {
            if (transitionMode == TransitionMode.ShiftDown || transitionMode == TransitionMode.ShiftUp)
            {
                if (isTransitioning)
                {
                    Transform parent = tooltipInstance.transform.parent;
                    tooltipInstance.transform.SetParent(tooltipInstanceParent, true);
                    Destroy(parent.gameObject);
                    tooltipInstance.RTransform.anchoredPosition = tooltipInstancePosition;
                }
            }
            StopAllCoroutines();
            SetActive(false);
        }

        /// <summary>
        /// OnPointerClick, disable the tooltip
        /// </summary>
        public void OnPointerClick(PointerEventData data)
        {
            if (transitionMode == TransitionMode.ShiftDown || transitionMode == TransitionMode.ShiftUp)
            {
                if (isTransitioning)
                {
                    Transform parent = tooltipInstance.transform.parent;
                    tooltipInstance.transform.SetParent(tooltipInstanceParent, true);
                    Destroy(parent.gameObject);
                    tooltipInstance.RTransform.anchoredPosition = tooltipInstancePosition;
                }
            }
            StopAllCoroutines();
            SetActive(false);
        }

        /// <summary>
        /// OnPointerEnter, enable the tooltip
        /// </summary>
        public void OnPointerEnter(PointerEventData data)
        {
            StopAllCoroutines();
            if (tooltipDelay > 0)
                StartCoroutine(DelayOpening());
            else
                SetActive(true);
        }

        /// <summary>
        /// OnPointerExit, disable the tooltip
        /// </summary>
        public void OnPointerExit(PointerEventData data)
        {
            if (transitionMode == TransitionMode.ShiftDown || transitionMode == TransitionMode.ShiftUp)
            {
                if (isTransitioning)
                {
                    Transform parent = tooltipInstance.transform.parent;
                    tooltipInstance.transform.SetParent(tooltipInstanceParent, true);
                    Destroy(parent.gameObject);
                    tooltipInstance.RTransform.anchoredPosition = tooltipInstancePosition;
                }
            }
            StopAllCoroutines();
            SetActive(false);
        }

        /// <summary>
        /// OnSelect, enable the tooltip
        /// </summary>
        public void OnSelect(BaseEventData data)
        {
            StopAllCoroutines();
            if (tooltipDelay > 0)
                StartCoroutine(DelayOpening());
            else
                SetActive(true);
        }

        /// <summary>
        /// OnSubmit, disable the tooltip
        /// </summary>
        public void OnSubmit(BaseEventData data)
        {
            if (transitionMode == TransitionMode.ShiftDown || transitionMode == TransitionMode.ShiftUp)
            {
                if (isTransitioning)
                {
                    Transform parent = tooltipInstance.transform.parent;
                    tooltipInstance.transform.SetParent(tooltipInstanceParent, true);
                    Destroy(parent.gameObject);
                    tooltipInstance.RTransform.anchoredPosition = tooltipInstancePosition;
                }
            }
            StopAllCoroutines();
            SetActive(false);
        }
        #endregion

        #region Appearance
        /// <summary>
        /// Set the tooltip's colors with this method
        /// </summary>
        /// <param name="background">Color applied to any non-text graphic</param>
        /// <param name="foreground">Color applied to any text graphic</param>
        public void SetColors(Color background, Color foreground)
        {
            if (!isInitialized)
                Initialize();

            Graphic[] graphics = tooltipInstance.GetComponentsInChildren<Graphic>();
            foreach(Graphic graphic in graphics)
            {
                if (graphic.GetType().Namespace == "TMPro")
                    graphic.color = foreground;
                else
                    graphic.color = background;
            }
        }

        /// <summary>
        /// Updates the tooltip text with a new tooltip and optional new values (required if the tooltip has formated values)
        /// </summary>
        /// <param name="newTooltip">The new tooltip string value</param>
        /// <param name="newValues">New values to display in the tooltip if the string requires formatting</param>
        public void UpdateText(string newTooltip, double[] newValues = null)
        {
            tooltipText = newTooltip;
            tooltipValues = newValues;
            SetText();
        }
        #endregion

        #region Positioning
        /// <summary>
        /// Positions the tooltip to the element for AnchorMode.AttachedToCursor
        /// </summary>
        /// <returns>The position of the tooltip</returns>
        private bool PositionTooltipToCursor(out Vector2 newPos)
        {
            bool didHit = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasToBeAttachedTo.transform as RectTransform, Mouse.current.position.ReadValue(), cameraThatRendersCanvas, out newPos);

            switch (anchorPosition)
            {
                case AnchorPosition.TopLeft:
                    newPos.x += tooltipInstance.RTransform.sizeDelta.x / 2 * tooltipHorizontalFactor;
                    newPos.y -= tooltipInstance.RTransform.sizeDelta.y / 2;
                    if (tooltipInstance.ArrowEnabled)
                        newPos.y -= tooltipInstance.Arrow.rectTransform.sizeDelta.y;
                    break;
                case AnchorPosition.TopMiddle:
                    newPos.y -= tooltipInstance.RTransform.sizeDelta.y / 2;
                    if (tooltipInstance.ArrowEnabled)
                        newPos.y -= tooltipInstance.Arrow.rectTransform.sizeDelta.y;
                    break;
                case AnchorPosition.TopRight:
                    newPos.x -= tooltipInstance.RTransform.sizeDelta.x / 2 * tooltipHorizontalFactor;
                    newPos.y -= tooltipInstance.RTransform.sizeDelta.y / 2;
                    if (tooltipInstance.ArrowEnabled)
                        newPos.y -= tooltipInstance.Arrow.rectTransform.sizeDelta.y;
                    break;
                case AnchorPosition.CenterLeft:
                    newPos.x += tooltipInstance.RTransform.sizeDelta.x / 2 * tooltipHorizontalFactor;
                    if (tooltipInstance.ArrowEnabled)
                        newPos.x += tooltipInstance.Arrow.rectTransform.sizeDelta.x;
                    break;
                case AnchorPosition.CenterRight:
                    newPos.x -= tooltipInstance.RTransform.sizeDelta.x / 2 * tooltipHorizontalFactor;
                    if (tooltipInstance.ArrowEnabled)
                        newPos.x -= tooltipInstance.Arrow.rectTransform.sizeDelta.x;
                    break;
                case AnchorPosition.BottomLeft:
                    newPos.x += tooltipInstance.RTransform.sizeDelta.x / 2 * tooltipHorizontalFactor;
                    newPos.y += tooltipInstance.RTransform.sizeDelta.y / 2;
                    if (tooltipInstance.ArrowEnabled)
                        newPos.y += tooltipInstance.Arrow.rectTransform.sizeDelta.y;
                    break;
                case AnchorPosition.BottomMiddle:
                    newPos.y += tooltipInstance.RTransform.sizeDelta.y / 2;
                    if (tooltipInstance.ArrowEnabled)
                        newPos.y += tooltipInstance.Arrow.rectTransform.sizeDelta.y;
                    break;
                case AnchorPosition.BottomRight:
                    newPos.x -= tooltipInstance.RTransform.sizeDelta.x / 2 * tooltipHorizontalFactor;
                    newPos.y += tooltipInstance.RTransform.sizeDelta.y / 2;
                    if (tooltipInstance.ArrowEnabled)
                        newPos.y += tooltipInstance.Arrow.rectTransform.sizeDelta.y;
                    break;

                case AnchorPosition.CenterMiddle:
                default:
                    // Do nothing in this case - newPos should already be centered if the notes for tooltipPrefab are followed
                    break;
            }

            #region Position clamp-to-screen
            Rect canvasRect = (canvasToBeAttachedTo.transform as RectTransform).rect;
            if (newPos.x < canvasRect.xMin + tooltipInstance.RTransform.sizeDelta.x / 2)
                newPos.x = canvasRect.xMin + tooltipInstance.RTransform.sizeDelta.x / 2;
            if (newPos.x + tooltipInstance.RTransform.sizeDelta.x / 2 > canvasRect.xMax)
                newPos.x = canvasRect.xMax - tooltipInstance.RTransform.sizeDelta.x / 2;
            if (newPos.y < canvasRect.yMin + tooltipInstance.RTransform.sizeDelta.y / 2)
                newPos.y = canvasRect.yMin + tooltipInstance.RTransform.sizeDelta.y / 2;
            if (newPos.y + tooltipInstance.RTransform.sizeDelta.y / 2 > canvasRect.yMax)
                newPos.y = canvasRect.yMax - tooltipInstance.RTransform.sizeDelta.y / 2;
            #endregion

            if (tooltipInstance.ArrowEnabled)
            {
                switch (anchorPosition)
                {
                    case AnchorPosition.TopLeft:
                    case AnchorPosition.TopMiddle:
                    case AnchorPosition.TopRight:
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                            (tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2));
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                        break;
                    case AnchorPosition.CenterLeft:
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(
                            -((tooltipInstance.RTransform.sizeDelta.x / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.x / 2)), 0);
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, -90);
                        break;
                    case AnchorPosition.CenterMiddle:
                        if (arrowDefaultPositionAtMiddle == MiddlePosition.Top)
                        {
                            tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                                (tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2));
                            tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        else
                        {
                            tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                                -(tooltipInstance.RTransform.sizeDelta.y / 2) - (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2));
                            tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                        }
                        break;
                    case AnchorPosition.CenterRight:
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(
                            ((tooltipInstance.RTransform.sizeDelta.x / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.x / 2)), 0);
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                        break;
                    case AnchorPosition.BottomLeft:
                    case AnchorPosition.BottomMiddle:
                    case AnchorPosition.BottomRight:
                    default:
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                            -((tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)));
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                        break;
                }
            }

            return didHit;
        }

        /// <summary>
        /// Positions the tooltip to the element for AnchorMode.AttachedToElement
        /// </summary>
        /// <returns>The position of the tooltip</returns>
        private Vector2 PositionTooltipToElement()
        {
            RectTransform thisRect = GetComponent<RectTransform>();
            Vector2 newPos = Vector2.zero;

            switch (anchorPosition)
            {
                case AnchorPosition.TopLeft:
                    newPos.x -= ((tooltipInstance.RTransform.sizeDelta.x / 2) + (thisRect.sizeDelta.x / 2)) * tooltipHorizontalFactor;
                    newPos.y += (tooltipInstance.RTransform.sizeDelta.y / 2) + (thisRect.sizeDelta.y / 2);

                    if (tooltipInstance.ArrowEnabled)
                    {
                        newPos.y += (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                            -((tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)));
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    break;
                case AnchorPosition.TopMiddle:
                    newPos.y += (tooltipInstance.RTransform.sizeDelta.y / 2) + (thisRect.sizeDelta.y / 2);

                    if (tooltipInstance.ArrowEnabled)
                    {
                        newPos.y += (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                            -((tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)));
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    break;
                case AnchorPosition.TopRight:
                    newPos.x += ((tooltipInstance.RTransform.sizeDelta.x / 2) + (thisRect.sizeDelta.x / 2)) * tooltipHorizontalFactor;
                    newPos.y += (tooltipInstance.RTransform.sizeDelta.y / 2) + (thisRect.sizeDelta.y / 2);

                    if (tooltipInstance.ArrowEnabled)
                    {
                        newPos.y += (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                            -((tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)));
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    break;
                case AnchorPosition.CenterLeft:
                    newPos.x -= ((tooltipInstance.RTransform.sizeDelta.x / 2) + (thisRect.sizeDelta.x / 2)) * tooltipHorizontalFactor;

                    if (tooltipInstance.ArrowEnabled)
                    {
                        newPos.x -= (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(
                            ((tooltipInstance.RTransform.sizeDelta.x / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)), 0);
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    break;
                case AnchorPosition.CenterRight:
                    newPos.x += ((tooltipInstance.RTransform.sizeDelta.x / 2) + (thisRect.sizeDelta.x / 2)) * tooltipHorizontalFactor;

                    if (tooltipInstance.ArrowEnabled)
                    {
                        newPos.x += (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(
                            -((tooltipInstance.RTransform.sizeDelta.x / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)), 0);
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, -90);
                    }
                    break;
                case AnchorPosition.BottomLeft:
                    newPos.x -= ((tooltipInstance.RTransform.sizeDelta.x / 2) + (thisRect.sizeDelta.x / 2)) * tooltipHorizontalFactor;
                    newPos.y -= (tooltipInstance.RTransform.sizeDelta.y / 2) + (thisRect.sizeDelta.y / 2);

                    if (tooltipInstance.ArrowEnabled)
                    {
                        newPos.y -= (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                            ((tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)));
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                    }
                    break;
                case AnchorPosition.BottomMiddle:
                    newPos.y -= (tooltipInstance.RTransform.sizeDelta.y / 2) + (thisRect.sizeDelta.y / 2);

                    if (tooltipInstance.ArrowEnabled)
                    {
                        newPos.y -= (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                            ((tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)));
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                    }
                    break;
                case AnchorPosition.BottomRight:
                    newPos.x += ((tooltipInstance.RTransform.sizeDelta.x / 2) + (thisRect.sizeDelta.x / 2)) * tooltipHorizontalFactor;
                    newPos.y -= (tooltipInstance.RTransform.sizeDelta.y / 2) + (thisRect.sizeDelta.y / 2);

                    if (tooltipInstance.ArrowEnabled)
                    {
                        newPos.y -= (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                        tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                            ((tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)));
                        tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                    }
                    break;

                case AnchorPosition.CenterMiddle:
                default:
                    if (tooltipInstance.ArrowEnabled)
                    {
                        if (arrowDefaultPositionAtMiddle == MiddlePosition.Bottom)
                        {
                            newPos.y += (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                            tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                                -((tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)));
                            tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                        }

                        else
                        {
                            newPos.y -= (tooltipInstance.Arrow.rectTransform.sizeDelta.y);
                            tooltipInstance.Arrow.rectTransform.anchoredPosition = new Vector2(0,
                                ((tooltipInstance.RTransform.sizeDelta.y / 2) + (tooltipInstance.Arrow.rectTransform.sizeDelta.y / 2)));
                            tooltipInstance.Arrow.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                    }
                    break;
            }

            return newPos;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Activates/deactivates the tooltip, which engages in transitions if the tooltip's active state is different from the new state
        /// </summary>
        /// <param name="_isActive">Whether to activate or deactivate the tooltip</param>
        private void SetActive(bool _isActive)
        {
            SetActive(_isActive, transitionMode);
        }

        /// <summary>
        /// Activates/deactivates the tooltip, which engages in transitions if the tooltip's active state is different from the new state
        /// <para>This overload overrides the mode defined for the element</para>
        /// </summary>
        /// <param name="_isActive">Whether to activate or deactivate the tooltip</param>
        /// <param name="mode">The mode of transition to use for animation</param>
        private void SetActive(bool _isActive, TransitionMode mode)
        {
            if (isActive != _isActive || !isActive)
            {
                isActive = _isActive;
                switch (mode)
                {
                    case TransitionMode.RotateHorizontal:
                    case TransitionMode.RotateHorizontalInverted:
                    case TransitionMode.RotateVertical:
                    case TransitionMode.RotateVerticalInverted:
                        if (!tooltipInstance.gameObject.activeSelf)
                            tooltipInstance.gameObject.SetActive(true);
                        if (!isActive && tooltipInstance.CGroup.alpha > 0 || isActive)
                        {
                            StartCoroutine(TransitionFade(isActive));
                            StartCoroutine(TransitionRotate(isActive, mode));
                        }
                        break;
                    case TransitionMode.Scale:
                    case TransitionMode.ScaleHorizontal:
                    case TransitionMode.ScaleVertical:
                        if (!tooltipInstance.gameObject.activeSelf)
                            tooltipInstance.gameObject.SetActive(true);
                        if (!isActive && tooltipInstance.CGroup.alpha > 0 || isActive)
                        {
                            StartCoroutine(TransitionFade(isActive));
                            StartCoroutine(TransitionScale(isActive, mode));
                        }
                        break;
                    case TransitionMode.ShiftUp:
                        if (!tooltipInstance.gameObject.activeSelf)
                            tooltipInstance.gameObject.SetActive(true);
                        if (!isActive && tooltipInstance.CGroup.alpha > 0 || isActive)
                        {
                            StartCoroutine(TransitionFade(isActive));
                            StartCoroutine(TransitionShift(isActive, false));
                        }
                        break;
                    case TransitionMode.ShiftDown:
                        if (!tooltipInstance.gameObject.activeSelf)
                            tooltipInstance.gameObject.SetActive(true);
                        if (!isActive && tooltipInstance.CGroup.alpha > 0 || isActive)
                        {
                            StartCoroutine(TransitionFade(isActive));
                            StartCoroutine(TransitionShift(isActive, true));
                        }
                        break;
                    case TransitionMode.Fade:
                        if (!tooltipInstance.gameObject.activeSelf)
                            tooltipInstance.gameObject.SetActive(true);
                        if (!isActive && tooltipInstance.CGroup.alpha > 0 || isActive)
                            StartCoroutine(TransitionFade(isActive));
                        break;
                    case TransitionMode.None:
                    default:
                        tooltipInstance.gameObject.SetActive(isActive);
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the text element text to the stored tooltipText and resizes the container
        /// </summary>
        private void SetText()
        {
            bool needsEnabled = false;
            if (!gameObject.activeInHierarchy)
                needsEnabled = true;

            if (!isInitialized)
                Initialize();

            if (tooltipValues != null && tooltipValues.Length > 0)
                tooltipInstance.Text.text = string.Format(tooltipText, tooltipValues.Cast<object>().ToArray()).RepairSerializedEscaping();
            else
                tooltipInstance.Text.text = tooltipText.RepairSerializedEscaping();

            if (needsEnabled)
            {
                Transform parent = tooltipInstance.transform.parent;
                tooltipInstance.transform.SetParent(null, true);
                bool activeSelf = tooltipInstance.gameObject.activeSelf;
                tooltipInstance.gameObject.SetActive(true);
                tooltipInstance.Text.ForceMeshUpdate();
                tooltipInstance.RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        tooltipInstance.Text.preferredHeight + tooltipInstance.Text.rectTransform.offsetMin.y * 2);
                tooltipInstance.transform.SetParent(parent, true);
                tooltipInstance.gameObject.SetActive(activeSelf);
            }

            else
                tooltipInstance.RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    tooltipInstance.Text.preferredHeight + tooltipInstance.Text.rectTransform.offsetMin.y * 2);
        }

        /// <summary>
        /// Coroutine for delaying the opening of the tooltip
        /// </summary>
        private IEnumerator DelayOpening()
        {
            yield return new WaitForSeconds(tooltipDelay);
            SetActive(true);
        }
        #endregion

        #region Transitions
        /// <summary>
        /// Coroutine for the Fade transition
        /// </summary>
        /// <param name="_isActive">Determines whether to fade in or out</param>
        private IEnumerator TransitionFade(bool _isActive)
        {
            isTransitioning = true;
            float t = 0;

            while (t <= transitionDuration)
            {
                if (_isActive)
                    tooltipInstance.CGroup.alpha = transitionCurveIn.Evaluate(t / transitionDuration);
                else
                    tooltipInstance.CGroup.alpha = transitionCurveOut.Evaluate(t / transitionDuration);

                yield return null;
                t += Time.deltaTime;
            }

            if (_isActive)
                tooltipInstance.CGroup.alpha = 1;
            else
                tooltipInstance.CGroup.alpha = 0;

            isTransitioning = false;
        }

        /// <summary>
        /// Coroutine for the ScaleHorizontal/ScaleVertical/Scale transitions
        /// </summary>
        /// <param name="_isActive">Determines whether to shift in or out</param>
        /// <param name="_scaleMode">One of the three scale modes; others passed through will simply default to Scale</param>
        private IEnumerator TransitionScale(bool _isActive, TransitionMode _scaleMode)
        {
            isTransitioning = true;
            float t = 0;
            Vector3 scaleStart;
            Vector3 scaleEnd;

            if (_isActive)
            {
                scaleEnd = Vector3.one;
                switch (_scaleMode)
                {
                    case TransitionMode.ScaleHorizontal:
                        scaleStart = new Vector3(0, 1, 1);
                        break;
                    case TransitionMode.ScaleVertical:
                        scaleStart = new Vector3(1, 0, 1);
                        break;
                    case TransitionMode.Scale:
                    default:
                        scaleStart = Vector3.zero;
                        break;
                }

                while (t <= transitionDuration)
                {
                    tooltipInstance.RTransform.localScale = Vector3.LerpUnclamped(scaleStart, scaleEnd, transitionCurveIn.Evaluate(t / transitionDuration));
                    yield return null;
                    t += Time.deltaTime;
                }
            }

            else
            {
                scaleStart = Vector3.one;
                switch (_scaleMode)
                {
                    case TransitionMode.ScaleHorizontal:
                        scaleEnd = new Vector3(0, 1, 1);
                        break;
                    case TransitionMode.ScaleVertical:
                        scaleEnd = new Vector3(1, 0, 1);
                        break;
                    case TransitionMode.Scale:
                    default:
                        scaleEnd = Vector3.zero;
                        break;
                }

                while (t <= transitionDuration)
                {
                    tooltipInstance.RTransform.localScale = Vector3.LerpUnclamped(scaleEnd, scaleStart, transitionCurveOut.Evaluate(t / transitionDuration));
                    yield return null;
                    t += Time.deltaTime;
                }
            }

            tooltipInstance.RTransform.localScale = scaleEnd;
            isTransitioning = false;
        }

        /// <summary>
        /// Coroutine for the ShiftUp/ShiftDown transitions
        /// </summary>
        /// <param name="_isActive">Determines whether to shift in or out</param>
        /// <param name="_shiftDown">Determines whether to shift up or down</param>
        private IEnumerator TransitionShift(bool _isActive, bool _shiftDown)
        {
            isTransitioning = true;
            float t = 0;
            GameObject parent = new GameObject();
            parent.transform.localPosition = tooltipInstance.transform.localPosition;
            parent.transform.SetParent(tooltipInstance.transform.parent, true);
            tooltipInstance.transform.SetParent(parent.transform, true);
            Vector3 posStart;
            Vector3 posEnd;

            if (_isActive)
            {
                if (_shiftDown)
                {
                    posStart = new Vector3(parent.transform.localPosition.x, parent.transform.localPosition.y + tooltipInstance.RTransform.sizeDelta.y / 2, parent.transform.localPosition.z);
                    posEnd = parent.transform.localPosition;
                }
                else
                {
                    posStart = new Vector3(parent.transform.localPosition.x, parent.transform.localPosition.y - tooltipInstance.RTransform.sizeDelta.y / 2, parent.transform.localPosition.z);
                    posEnd = parent.transform.localPosition;
                }

                while (t <= transitionDuration)
                {
                    parent.transform.localPosition = Vector3.Lerp(posStart, posEnd, transitionCurveIn.Evaluate(t / transitionDuration));
                    yield return null;
                    t += Time.deltaTime;
                }
            }

            else
            {
                if (_shiftDown)
                {
                    posEnd = new Vector3(parent.transform.localPosition.x, parent.transform.localPosition.y + tooltipInstance.RTransform.sizeDelta.y / 2, parent.transform.localPosition.z);
                    posStart = parent.transform.localPosition;
                }
                else
                {
                    posEnd = new Vector3(parent.transform.localPosition.x, parent.transform.localPosition.y - tooltipInstance.RTransform.sizeDelta.y / 2, parent.transform.localPosition.z);
                    posStart = parent.transform.localPosition;
                }

                while (t <= transitionDuration)
                {
                    parent.transform.localPosition = Vector3.LerpUnclamped(posEnd, posStart, transitionCurveOut.Evaluate(t / transitionDuration));
                    yield return null;
                    t += Time.deltaTime;
                }
            }

            tooltipInstance.transform.SetParent(tooltipInstanceParent, true);
            Destroy(parent.gameObject);
            if (!_isActive)
                tooltipInstance.RTransform.anchoredPosition = tooltipInstancePosition;
            isTransitioning = false;
        }

        /// <summary>
        /// Coroutine for the various Tilt transitions
        /// </summary>
        /// <param name="_isActive">Determines whether to shift in or out</param>
        /// <param name="_rotMode">One of the four tilt modes; others passed through will simply default to RotateVerticalInverted</param>
        private IEnumerator TransitionRotate(bool _isActive, TransitionMode _rotMode)
        {
            isTransitioning = true;
            float t = 0;
            Quaternion rotStart;
            Quaternion rotEnd;

            if (_isActive)
            {
                rotEnd = Quaternion.identity;
                switch(_rotMode)
                {
                    case TransitionMode.RotateHorizontal:
                        rotStart = Quaternion.Euler(new Vector3(90, 0, 0));
                        break;
                    case TransitionMode.RotateHorizontalInverted:
                        rotStart = Quaternion.Euler(new Vector3(-90, 0, 0));
                        break;
                    case TransitionMode.RotateVertical:
                        rotStart = Quaternion.Euler(new Vector3(0, 90, 0));
                        break;
                    case TransitionMode.RotateVerticalInverted:
                    default:
                        rotStart = Quaternion.Euler(new Vector3(0, -90, 0));
                        break;
                }

                while (t <= transitionDuration)
                {
                    tooltipInstance.RTransform.rotation = Quaternion.SlerpUnclamped(rotStart, rotEnd, transitionCurveIn.Evaluate(t / transitionDuration));
                    yield return null;
                    t += Time.deltaTime;
                }
            }

            else
            {
                rotStart = Quaternion.identity;
                switch (_rotMode)
                {
                    case TransitionMode.RotateHorizontal:
                        rotEnd = Quaternion.Euler(new Vector3(90, 0, 0));
                        break;
                    case TransitionMode.RotateHorizontalInverted:
                        rotEnd = Quaternion.Euler(new Vector3(-90, 0, 0));
                        break;
                    case TransitionMode.RotateVertical:
                        rotEnd = Quaternion.Euler(new Vector3(0, 90, 0));
                        break;
                    case TransitionMode.RotateVerticalInverted:
                    default:
                        rotEnd = Quaternion.Euler(new Vector3(0, -90, 0));
                        break;
                }

                while (t <= transitionDuration)
                {
                    tooltipInstance.RTransform.rotation = Quaternion.SlerpUnclamped(rotEnd, rotStart, transitionCurveOut.Evaluate(t / transitionDuration));
                    yield return null;
                    t += Time.deltaTime;
                }
            }

            tooltipInstance.RTransform.rotation = rotEnd;
            isTransitioning = false;
        }
        #endregion
        #endregion
    }
}
