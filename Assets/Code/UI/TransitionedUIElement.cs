using UnityEngine;
using System.Collections;

namespace SFBuilder.UI
{
    public class TransitionedUIElement : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private PartToAnimate  partToAnimate;
        [SerializeField] private Vector3        valueStart;
        [SerializeField] private Vector3        valueStop;
#pragma warning restore 0649
        public bool IsComplete { get; private set; }

        /// <summary>
        /// On Enable, animate the element
        /// </summary>
        private void OnEnable()
        {
            IsComplete = false;
            switch (partToAnimate)
            {
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
                case PartToAnimate.Scale:
                default:
                    GetComponent<RectTransform>().localScale = valueStop;
                    break;
            }
            IsComplete = true;
        }
    }

    /// <summary>
    /// Definition of what part of a TransitionedUIElement should be animated
    /// </summary>
    public enum PartToAnimate
    {
        Scale
    }
}