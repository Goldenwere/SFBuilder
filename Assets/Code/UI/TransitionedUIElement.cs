using UnityEngine;
using System.Collections;

namespace SFBuilder.UI
{
    /// <summary>
    /// Represents an element that has some sort of transition whenever it is enabled
    /// </summary>
    public class TransitionedUIElement : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private PartToAnimate  partToAnimate;
        [SerializeField] private Vector4        valueStart;
        [SerializeField] private Vector4        valueStop;
#pragma warning restore 0649
        public bool IsComplete { get; private set; }
        #endregion
        #region Methods
        /// <summary>
        /// On Enable, animate the element
        /// </summary>
        private void OnEnable()
        {
            IsComplete = false;
            switch (partToAnimate)
            {
                case PartToAnimate.HorizontalStretch:
                    GetComponent<RectTransform>().offsetMin = new Vector2(valueStart.x, valueStart.y);
                    GetComponent<RectTransform>().offsetMax = new Vector2(valueStart.z, valueStart.w);
                    break;
                case PartToAnimate.Scale:
                default:
                    GetComponent<RectTransform>().localScale = valueStart;
                    break;
            }
            StartCoroutine(TransitionElement(UITransitionSystem.Instance.AddElement(this)));
        }

        /// <summary>
        /// Transitions a UI element
        /// </summary>
        /// <param name="delayMultiplier">How much the delay should be</param>
        private IEnumerator TransitionElement(int delayMultiplier)
        {
            yield return new WaitForSeconds(delayMultiplier * GameConstants.UITransitionDelay);
            float t = 0;
            while (t <= GameConstants.UITransitionDuration)
            {
                switch (partToAnimate)
                {
                    case PartToAnimate.HorizontalStretch:
                        GetComponent<RectTransform>().offsetMin = Vector2.LerpUnclamped(
                            new Vector2(valueStart.x, valueStart.y),
                            new Vector2(valueStop.x, valueStop.y),
                            UITransitionSystem.Instance.AnimCurve.Evaluate(t / GameConstants.UITransitionDuration));
                        GetComponent<RectTransform>().offsetMax = Vector2.LerpUnclamped(
                            new Vector2(valueStart.z, valueStart.w),
                            new Vector2(valueStop.z, valueStop.w),
                            UITransitionSystem.Instance.AnimCurve.Evaluate(t / GameConstants.UITransitionDuration));
                        break;
                    case PartToAnimate.Scale:
                    default:
                        GetComponent<RectTransform>().localScale = Vector3.LerpUnclamped(valueStart, valueStop, UITransitionSystem.Instance.AnimCurve.Evaluate(t / GameConstants.UITransitionDuration));
                        break;
                }
                t += Time.deltaTime;
                yield return null;
            }
            switch (partToAnimate)
            {
                case PartToAnimate.HorizontalStretch:
                    GetComponent<RectTransform>().offsetMin = new Vector2(valueStop.x, valueStop.y);
                    GetComponent<RectTransform>().offsetMax = new Vector2(valueStop.z, valueStop.w);
                    break;
                case PartToAnimate.Scale:
                default:
                    GetComponent<RectTransform>().localScale = valueStop;
                    break;
            }
            IsComplete = true;
        }
        #endregion
    }

    /// <summary>
    /// Definition of what part of a TransitionedUIElement should be animated
    /// </summary>
    public enum PartToAnimate
    {
        Scale,
        HorizontalStretch
    }
}