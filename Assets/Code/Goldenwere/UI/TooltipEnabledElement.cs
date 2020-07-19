using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Goldenwere.Unity.UI
{
    public class TooltipEnabledElement : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private Camera         cameraThatRendersCanvas;
        [SerializeField] private Canvas         canvasToBeAttachedTo;
        [Tooltip         ("Prefab which the topmost gameobject can be resized based on text and contains a text element that can be set")]
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

        private void Update()
        {
            if (tooltipSpawnedElement.activeInHierarchy)
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasToBeAttachedTo.transform as RectTransform, Mouse.current.position.ReadValue(), cameraThatRendersCanvas, out Vector2 newPos))
                    tooltipSpawnedTransform.anchoredPosition = newPos;
        }

        public void OnPointerEnter()
        {
            tooltipSpawnedElement.SetActive(true);
        }

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
}