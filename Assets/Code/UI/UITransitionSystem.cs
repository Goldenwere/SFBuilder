using UnityEngine;
using System.Collections.Generic;

namespace SFBuilder.UI
{
    /// <summary>
    /// The UITransitionSystem keeps track of UI elements in order to properly implement delays to elements
    /// </summary>
    public class UITransitionSystem : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private AnimationCurve                 animCurve;
#pragma warning restore 0649
        /**************/ private List<TransitionedUIElement>    currentElements;
        #endregion
        #region Properties
        /// <summary>
        /// The animation curve the UI system uses for transitions
        /// </summary>
        public AnimationCurve AnimCurve { get { return animCurve; } }

        /// <summary>
        /// Singleton instance of UITransitionSystem in the base level scene
        /// </summary>
        public static UITransitionSystem Instance { get; private set; }
        #endregion
        #region Methods
        /// <summary>
        /// Set singleton instance on Awake
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            currentElements = new List<TransitionedUIElement>(16);
        }

        /// <summary>
        /// On Update, keep track of elements in the list to remove them when needed
        /// </summary>
        private void Update()
        {
            for (int i = currentElements.Count - 1; i >= 0; i--)
                if (currentElements[i].IsComplete)
                    currentElements.RemoveAt(i);
        }

        /// <summary>
        /// Adds an element to the currently animating elements list
        /// </summary>
        /// <param name="elem">The element to add</param>
        /// <returns>How many elements are in the list (excluding the one added)</returns>
        public int AddElement(TransitionedUIElement elem)
        {
            if (!currentElements.Contains(elem))
                currentElements.Add(elem);
            return currentElements.Count - 1;
        }
        #endregion
    }
}