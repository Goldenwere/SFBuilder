using UnityEngine;
using UnityEngine.UI;
using SFBuilder.Gameplay;
using System.Collections;

namespace SFBuilder.UI
{
    /// <summary>
    /// Displays value for a stat in a radial element
    /// </summary>
    /// <remarks>Currently, only viability is a </remarks>
    public class RadialStatIndicator : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private ScoreType  associatedType;
        [SerializeField] private Image      associatedImageElement;
#pragma warning restore 0649

        /// <summary>
        /// On Start, ensure the UI displays proper values by calling the OnScoreWasChanged handler
        /// </summary>
        private void Start()
        {
            switch (associatedType)
            {
                case ScoreType.TotalViability:
                    OnScoreWasChanged(ScoreType.TotalViability, GameSave.Instance.currentHappiness + GameSave.Instance.currentPower + GameSave.Instance.currentSustenance);
                    break;
                case ScoreType.PotentialViability:
                    OnScoreWasChanged(ScoreType.PotentialViability, 0);
                    break;
            }
        }

        /// <summary>
        /// On Enable, subscribe to the ScoreWasChanged event
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.ScoreWasChanged += OnScoreWasChanged;
            GameEventSystem.GoalChanged += OnGoalChanged;
        }

        /// <summary>
        /// On Disable, unsubscribe from the ScoreWasChanged event
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.ScoreWasChanged -= OnScoreWasChanged;
            GameEventSystem.GoalChanged -= OnGoalChanged;
        }

        /// <summary>
        /// On the GoalChanged event, update viability radial stat indicator
        /// </summary>
        private void OnGoalChanged()
        {
            if (associatedType == ScoreType.TotalViability)
                OnScoreWasChanged(ScoreType.TotalViability, GameScoring.Instance.TotalViability);
            else if (associatedType == ScoreType.PotentialViability)
                associatedImageElement.fillAmount = 0;
        }

        /// <summary>
        /// On the ScoreWasChanged event, update UI if the proper type matches
        /// </summary>
        /// <param name="type">The type that was updated</param>
        /// <param name="val">The value at the passed type</param>
        private void OnScoreWasChanged(ScoreType type, int val)
        {
            if (type == associatedType)
            {
                switch (associatedType)
                {
                    case ScoreType.PotentialViability:
                        if (val == 0)
                            associatedImageElement.fillAmount = 0;

                        else
                        {
                            if (associatedImageElement.fillAmount == 0)
                            {
                                StartCoroutine(TransitionFill(
                                    GameScoring.Instance.TotalViability,
                                    Mathf.Clamp(val + GameScoring.Instance.TotalViability - GoalSystem.Instance.PreviousGoalViability, 0, float.MaxValue) / (GoalSystem.Instance.CurrentGoalWorkingSet.goalViability - GoalSystem.Instance.PreviousGoalViability)
                                ));
                            }
                            else
                            {
                                StartCoroutine(TransitionFill(
                                    associatedImageElement.fillAmount,
                                    Mathf.Clamp(val + GameScoring.Instance.TotalViability - GoalSystem.Instance.PreviousGoalViability, 0, float.MaxValue) / (GoalSystem.Instance.CurrentGoalWorkingSet.goalViability - GoalSystem.Instance.PreviousGoalViability)
                                ));
                            }
                        }
                        break;
                    case ScoreType.TotalViability:
                        StartCoroutine(TransitionFill(
                            associatedImageElement.fillAmount,
                            Mathf.Clamp(val - GoalSystem.Instance.PreviousGoalViability, 0, float.MaxValue) / (GoalSystem.Instance.CurrentGoalWorkingSet.goalViability - GoalSystem.Instance.PreviousGoalViability)
                        ));
                        break;
                }
            }
        }

        /// <summary>
        /// Coroutine for animating radial elements
        /// </summary>
        /// <param name="start">The starting value (usually current, unless specifically targetting Potential when its current is 0, in which it should start from Total)</param>
        /// <param name="end">The ending value</param>
        private IEnumerator TransitionFill(float start, float end)
        {
            float t = 0;
            associatedImageElement.fillAmount = start;
            while (t <= GameConstants.UITransitionRadialDuration)
            {
                associatedImageElement.fillAmount = AnimationCurve.EaseInOut(0, start, 1, end).Evaluate(t / GameConstants.UITransitionRadialDuration);
                yield return null;
                t += Time.deltaTime;
            }
            associatedImageElement.fillAmount = end;
        }
    }
}