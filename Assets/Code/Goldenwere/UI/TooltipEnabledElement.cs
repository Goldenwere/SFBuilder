using TMPro;
using UnityEngine;

namespace Goldenwere.Unity.UI
{
    public class TooltipEnabledElement : MonoBehaviour
    {
        [Tooltip         ("Prefab which the topmost gameobject can be resized based on text and contains a text element that can be set")]
        [SerializeField] private GameObject tooltipPrefab;
        [Tooltip         ("The text to display in the tooltip")]
        [SerializeField] private string     tooltipText;
        [Tooltip         ("Values used if defining a string that needs formatting. Leave blank if no formatting is done inside tooltipText")]
        [SerializeField] private double[]   tooltipValues;
        /**************/ private GameObject tooltipSpawnedElement;
        /**************/ private TMP_Text   tooltipTextElement;

        /// <summary>
        /// Sets up the tooltip at start
        /// </summary>
        private void Start()
        {
            tooltipSpawnedElement = Instantiate(tooltipPrefab, transform);
            tooltipTextElement = tooltipSpawnedElement.GetComponentInChildren<TMP_Text>();
            SetText();
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
                tooltipTextElement.text = string.Format(tooltipText, tooltipValues);
            else
                tooltipTextElement.text = tooltipText;

            tooltipSpawnedElement.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
                tooltipTextElement.renderedHeight + tooltipTextElement.rectTransform.anchoredPosition.y);
        }
    }
}