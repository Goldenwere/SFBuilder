using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Goldenwere.Unity.UI
{
    /// <summary>
    /// Adds a tooltip to a UI element
    /// </summary>
    public class TooltipEnabledElement : MonoBehaviour
    {
#pragma warning disable 0649
        [Tooltip         ("Needed in order to ensure proper tooltip positioning")]
        [SerializeField] private Camera         cameraThatRendersCanvas;
        [Tooltip         ("Needed in order to ensure proper tooltip positioning as well as attaching tooltip to canvas")]
        [SerializeField] private Canvas         canvasToBeAttachedTo;
        [Tooltip         ("The default anchor position. If the tooltip text overflows with this anchor, will change to another one if needed")]
        [SerializeField] private AnchorPosition tooltipAnchorPosition;
        [Tooltip         ("Prefab which the topmost gameobject can be resized based on text and contains a text element that can be set\n" +
                          "Note: Make sure that the text element has the horizontal+vertical stretch anchor preset and equivalent padding on all sides," +
                          "as this class depends on the left padding when determining container height + bottom padding\n" +
                          "Make sure that the container uses the center+center anchor preset, as this class needs to use its own anchor method due to depending on cursor position")]
        [SerializeField] private GameObject     tooltipPrefab;
        [Tooltip         ("The text to display in the tooltip")]
        [SerializeField] private string         tooltipText;
        [Tooltip         ("Values used if defining a string that needs formatting. Leave blank if no formatting is done inside tooltipText")]
        [SerializeField] private double[]       tooltipValues;
#pragma warning restore 0649
        /**************/ private GameObject     tooltipSpawnedElement;
        /**************/ private RectTransform  tooltipSpawnedTransform;
        /**************/ private TMP_Text       tooltipTextElement;

        /// <summary>
        /// Sets up the tooltip at start
        /// </summary>
        private void Start()
        {
            tooltipSpawnedElement = Instantiate(tooltipPrefab, canvasToBeAttachedTo.transform);
            tooltipTextElement = tooltipSpawnedElement.GetComponentInChildren<TMP_Text>();
            tooltipSpawnedTransform = tooltipSpawnedElement.GetComponent<RectTransform>();
            SetText();
            tooltipSpawnedElement.SetActive(false);
        }

        /// <summary>
        /// Set position of tooltip at Update
        /// </summary>
        private void Update()
        {
            if (tooltipSpawnedElement.activeInHierarchy)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasToBeAttachedTo.transform as RectTransform, Mouse.current.position.ReadValue(), cameraThatRendersCanvas, out Vector2 newPos))
                {
                    switch (tooltipAnchorPosition)
                    {
                        case AnchorPosition.TopLeft:
                            newPos.x += tooltipSpawnedTransform.sizeDelta.x / 2;
                            newPos.y -= tooltipSpawnedTransform.sizeDelta.y / 2;
                            break;
                        case AnchorPosition.TopMiddle:
                            newPos.y -= tooltipSpawnedTransform.sizeDelta.y / 2;
                            break;
                        case AnchorPosition.TopRight:
                            newPos.x -= tooltipSpawnedTransform.sizeDelta.x / 2;
                            newPos.y -= tooltipSpawnedTransform.sizeDelta.y / 2;
                            break;
                        case AnchorPosition.CenterLeft:
                            newPos.x += tooltipSpawnedTransform.sizeDelta.x / 2;
                            break;
                        case AnchorPosition.CenterRight:
                            newPos.x -= tooltipSpawnedTransform.sizeDelta.x / 2;
                            break;
                        case AnchorPosition.BottomLeft:
                            newPos.x += tooltipSpawnedTransform.sizeDelta.x / 2;
                            newPos.y += tooltipSpawnedTransform.sizeDelta.y / 2;
                            break;
                        case AnchorPosition.BottomMiddle:
                            newPos.y += tooltipSpawnedTransform.sizeDelta.y / 2;
                            break;
                        case AnchorPosition.BottomRight:
                            newPos.x -= tooltipSpawnedTransform.sizeDelta.x / 2;
                            newPos.y += tooltipSpawnedTransform.sizeDelta.y / 2;
                            break;

                        case AnchorPosition.CenterMiddle:
                        default:
                            // Do nothing in this case - newPos should already be centered if the notes for tooltipPrefab are followed
                            break;
                    }

                    Rect canvasRect = (canvasToBeAttachedTo.transform as RectTransform).rect;

                    if (newPos.x < canvasRect.xMin + tooltipSpawnedTransform.sizeDelta.x / 2)
                        newPos.x = canvasRect.xMin + tooltipSpawnedTransform.sizeDelta.x / 2;
                    if (newPos.x + tooltipSpawnedTransform.sizeDelta.x / 2 > canvasRect.xMax)
                        newPos.x = canvasRect.xMax - tooltipSpawnedTransform.sizeDelta.x / 2;
                    if (newPos.y < canvasRect.yMin + tooltipSpawnedTransform.sizeDelta.y / 2)
                        newPos.y = canvasRect.yMin + tooltipSpawnedTransform.sizeDelta.y / 2;
                    if (newPos.y + tooltipSpawnedTransform.sizeDelta.y / 2 > canvasRect.yMax)
                        newPos.y = canvasRect.yMax - tooltipSpawnedTransform.sizeDelta.y / 2;
                    tooltipSpawnedTransform.anchoredPosition = newPos;
                }
            }
        }

        /// <summary>
        /// OnPointerEnter, enable the tooltip
        /// </summary>
        public void OnPointerEnter()
        {
            tooltipSpawnedElement.SetActive(true);
        }

        /// <summary>
        /// OnPointerExit, disable the tooltip
        /// </summary>
        public void OnPointerExit()
        {
            tooltipSpawnedElement.SetActive(false);
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

        /// <summary>
        /// Sets the text element text to the stored tooltipText and resizes the container
        /// </summary>
        private void SetText()
        {
            if (tooltipValues != null && tooltipValues.Length > 0)
                tooltipTextElement.text = string.Format(tooltipText, tooltipValues).RepairSerializedEscaping();
            else
                tooltipTextElement.text = tooltipText.RepairSerializedEscaping();

            tooltipSpawnedTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
                tooltipTextElement.preferredHeight + tooltipTextElement.rectTransform.offsetMin.x);
        }
    }

    /// <summary>
    /// Defines how the tooltip should be anchored
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
}