using UnityEngine;
using UnityEngine.UI;
using SFBuilder.Obj;
using Goldenwere.Unity.UI;

namespace SFBuilder.UI
{
    /// <summary>
    /// An instance of BuilderButton is attached to a Button prefab to give it functionality related to object placement
    /// </summary>
    public class BuilderButton : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private Button                 button;
        [SerializeField] private TMPro.TMP_Text         indicatorCount;
        [SerializeField] private Image                  indicatorIcon;
        [SerializeField] private TMPro.TMP_Text         indicatorID;
        [SerializeField] private TooltipEnabledElement  tooltipElement;
#pragma warning restore 0649
        /**************/ private int                    associatedCount;
        /**************/ private int                    associatedID;
        /**************/ private bool                   initialized;
        /**************/ private bool                   isRequired;
        #endregion
        #region Methods
        /// <summary>
        /// When the button is first being created, call this to associate it with a BuilderObject
        /// </summary>
        /// <param name="id">The numerical id of the BuilderObject (casted to ObjectType)</param>
        /// <param name="count">The number of the specified BuilderObject the button can create</param>
        /// <param name="required">Whether or not the BuilderObject is required (determines which working set to edit)</param>
        public void Initialize(ButtonInfo info)
        {
            if (!initialized)
            {
                associatedCount = info.count;
                associatedID = info.id;
                isRequired = info.req;
                Sprite s = GameUI.Instance.GetIcon((ObjectType)info.id);
                if (s != null)
                {
                    indicatorIcon.sprite = s;
                    indicatorID.gameObject.SetActive(false);
                }
                else
                {
                    indicatorID.text = BuilderObject.NameOfType((ObjectType)info.id);
                    indicatorIcon.gameObject.SetActive(false);
                }
                indicatorCount.text = info.count.ToString();
                button.interactable = associatedCount > 0;
                BuilderObject.DescriptionOfType((ObjectType)info.id, out string desc);
                tooltipElement.UpdateText(desc);
                initialized = true;
            }
        }

        /// <summary>
        /// Handler for button's press event, which utilized the PlacementSystem to spawn a BuilderObject
        /// </summary>
        public void OnButtonPress()
        {
            if (associatedCount > 0)
            {
                BuilderObject spawned = PlacementSystem.Instance.OnObjectSelected(associatedID);
                if (spawned != null)
                {
                    spawned.objectPlaced += OnObjectPlaced;
                    spawned.objectRecalled += OnObjectRecalled;
                }
            }
        }

        /// <summary>
        /// Handler for a BuilderObject's objectPlaced event which decrements the associated count on the button and the current goal working set
        /// </summary>
        /// <param name="obj">The object that was placed</param>
        private void OnObjectPlaced(BuilderObject obj)
        {
            associatedCount--;
            indicatorCount.text = associatedCount.ToString();
            if (associatedCount <= 0)
                button.interactable = false;
            GameEventSystem.Instance.UpdateGoalAmount(false, associatedID, isRequired);
        }

        /// <summary>
        /// Handler for a BuilderObject's objectRecalled event (called by Undo) which increments the associated count on the button and the current goal working set
        /// </summary>
        /// <param name="obj">The object that was recalled</param>
        private void OnObjectRecalled(BuilderObject obj)
        {
            associatedCount++;
            indicatorCount.text = associatedCount.ToString();
            if (associatedCount > 0 && !button.interactable)
                button.interactable = true;
            GameEventSystem.Instance.UpdateGoalAmount(true, associatedID, isRequired);
        }
        #endregion
    }
}